# Reference Solution — Spoiler

## Symptoms
BK-17 v2 8.4°C không tạo alert trên A dù ingress ACK.

## Evidence
A yêu cầu `temperatureC`; v2 chỉ có `celsius`; thiếu field thì A ACK im lặng. BK-18 v1 7.2°C là control; BK-20 sai type; BK-21 có hai field mâu thuẫn. Broker latency không chứng minh consumer success.

## Root cause
Producer thay đổi schema trên cùng topic không bảo toàn compatibility; consumer im lặng ACK invalid event.

## Why the fix works
Một phương án bảo vệ được: versioned ingress adapter/upcaster tạo canonical envelope gồm `eventId`, `schemaVersion`, `deviceId`, `recordedAt`, `receivedAt`, `temperature:{value,unit}`. Giữ raw immutable 90 ngày; chỉ gửi tới legacy A qua compatibility adapter hoặc stream đã kiểm thử. Không route v2 raw tới A. Reject/quarantine unknown version, invalid type, conflicting values; phát metric và DLQ, không ACK mất dữ liệu.

## How to verify
| Event | A cũ | B mới | Canonical adapter |
|---|---|---|---|
| BK-17 v1 4.3 | no alert | no alert | 4.3°C |
| BK-17 v2 8.4 | silent drop | alert | alert |
| BK-18 v1 7.2 | alert | alert | alert |
| BK-19 v2 4.1 | silent drop | no alert | no alert |
| BK-20 string 7.8 | silent drop | invalid | quarantine |
| BK-21 5.1 vs 8.9 | no alert | alert | quarantine |

Contract gates: missing fields, wrong type, unknown version, conflicting fields, duplicates, replay order, poison DLQ, old firmware rollback, end-to-end p99 <60s on representative load. Fixture validates contract logic, not real SLO.

## Alternative fixes
Additive dual-write giữ cả hai field trong 21 ngày, rẻ nhưng có drift risk. Versioned topics tách consumers rõ nhưng thêm routing và operational cost. Adapter tập trung logic nhưng cần HA và observability.

## Wrong / misleading fixes
Scale broker không sửa compatibility; default missing temperature = 0 tạo false negative; ACK malformed payload mất khả năng replay; disable alert khi nâng cấp vi phạm nghiệp vụ.

## Production implications
Canary 5% → 25% → 100%, stop nếu high-temp test mất alert, invalid không quarantine, hay latency vượt SLO. Giám sát per-version processing/invalid counters, DLQ, end-to-end age, replay success. Rollback phải giữ khả năng đọc 90 ngày raw.

## Trade-offs
Adapter + compatibility stream phù hợp coexistence 21 ngày, đổi lại thêm một thành phần vận hành. Xóa legacy path chỉ sau khi consumer đã nâng cấp và replay được kiểm chứng.

## What a Senior engineer should notice
JSON parse thành công không đồng nghĩa semantic compatibility. Kiểm tra business invariant và failure-path trước rollout.

## Requirement change
Fahrenheit là một unit mới có khai báo, không đổi nghĩa field Celsius; canonical conversion với tolerance. Dedupe theo stable eventId; event-time watermark và late-event route; phân biệt recordedAt/receivedAt.
