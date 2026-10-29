# Synthetic incident evidence — 29/10/2026

Tất cả dữ liệu dưới đây được dựng cho lab, không phải log production thực.

| Time (UTC+7) | Signal | Value |
|---|---|---|
| 08:02 | Feed v1 sample R-100 | part BRK-22, 7 units |
| 08:07 | Supplier deploy note | "minor payload formatting update" |
| 08:11 | Incoming feed R-101 | partner document says 7 units |
| 08:11 | Ingestion response | accepted=true, exceptionCount=0 |
| 08:12 | Warehouse reservation ledger | R-101: 0 units |
| 08:13 | Available-to-promise | +7 units compared with partner reservation sheet |
| 08:16 | CPU / memory / dependency latency | baseline, no anomalies |

## Các payload quan sát

**R-100**: `{"schemaVersion":1,"reservationId":"R-100","partCode":"BRK-22","reservedUnits":7}`

**R-101**: `{"schemaVersion":1,"reservationId":"R-101","partCode":"BRK-22","reserved_units":7}`

**R-102**: `{"schemaVersion":1,"reservationId":"R-102","partCode":"BRK-22"}`

## Điều cần kiểm chứng

Phân biệt lỗi nguồn, lỗi transform và lỗi ghi kho. Đo giá trị sau deserialize trước khi suy luận về database; ghi lại input nào bị từ chối và input nào được chấp nhận. Không dựa vào việc HTTP 200 để kết luận contract hợp lệ.
