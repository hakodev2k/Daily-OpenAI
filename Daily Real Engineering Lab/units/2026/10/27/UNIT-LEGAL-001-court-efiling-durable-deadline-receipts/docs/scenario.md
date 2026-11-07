# Scenario & ràng buộc đã được Product/Legal xác nhận

## Luồng hiện tại

Web/mobile client gửi một submission envelope (metadata, file hashes, tham chiếu attachments đã upload) đến Submission API. API gọi Submission Store, trả receipt ID; workers xử lý malware scan, chữ ký, format và đẩy hồ sơ sang case-management. Một gateway và hai cụm application instances phục vụ traffic.

## Ngữ nghĩa pháp lý cho **bài lab này**

- Deadline: **17:00:00.000 Asia/Ho_Chi_Minh** ngày xét xử; instant được tính bằng server-side trusted time, không dùng client clock.
- Một hồ sơ được coi là **nộp đúng hạn** khi **durable acceptance commit** của envelope xảy ra **trước hoặc đúng** instant deadline. Request bắt đầu trước hạn nhưng commit sau hạn là trễ.
- `RECEIVED` chỉ chứng minh envelope đã được lưu và được gắn thời điểm nộp; **không** có nghĩa `ACCEPTED_FOR_FILING`. Kiểm tra malware/chữ ký chuyên sâu có thể kết luận `REJECTED` sau đó, nhưng không được sửa lịch sử tiếp nhận.
- Receipt phải chứa receipt ID, submission ID, filing party, canonical payload digest, authoritative acceptedAt UTC, deadline verdict, trạng thái `RECEIVED`, schema version và proof/reference có thể xác minh. Không đưa PII vào log hoặc URL.
- Client cung cấp idempotency key ổn định cho cùng một ý định nộp; retry có thể đến sau khi mất kết nối.

## Capacity / availability

| Chỉ tiêu | Yêu cầu |
|---|---|
| Traffic | 12.000 submission/ngày; 70% trong 15 phút cuối; peak 300 request/s |
| Envelope | <= 32 KiB, attachments đã được staging và có content digest |
| API | p95 <= 1.5 s khi hệ thống khỏe, p99 <= 4 s |
| Availability | >= 99.9%/tháng; ưu tiên không phát hành receipt sai khi mất quorum |
| Durability | **RPO = 0 cho mọi receipt đã xác nhận** sau mất một failure domain |
| Recovery | RTO <= 15 phút khi một site lỗi; không có quyền trả receipt giả trong thời gian phục hồi |
| Data | Residency trong quốc gia, mã hóa at rest/in transit, audit 7 năm, truy cập theo vai trò |
| Team | 5 backend engineers, 1 SRE part-time; ngân sách hạn chế |
| External systems | Case-management và malware scanner có thể offline 45 phút; không được làm mất receipt |

Hai site trong cùng quốc gia có thể mất kết nối; chưa cam kết rằng hạ tầng hỗ trợ đồng bộ quorum trên mọi tình huống. **Không được giả định đồng thời availability vô điều kiện và RPO 0**.

## Deliverables

1. Chỉ ra chính xác **acceptance commit point** và điều kiện phát hành receipt; nêu rõ failure semantics cho HTTP 200/202, 409, 503, timeout/unknown.
2. State machine tối thiểu: `RECEIVED`, `VALIDATING`, `ACCEPTED_FOR_FILING`, `REJECTED`; không overwrite event history.
3. Data model cho submission, unique idempotency key scoped theo filing party, digest, acceptedAt, receipt; xử lý same-key/different-payload.
4. Sequence diagram normal, ambiguous timeout, site failover và downstream outage.
5. Hai architecture options có trade-offs về latency, consistency, operational cost; ADR chọn một option.
6. Failure-test matrix cho evidence A–E; metrics/alerts và deployment rollout.
7. Làm [change request](../extensions/change-request.md) sau khi chốt ADR đầu tiên.

Không cần chọn vendor/cloud cụ thể; mọi claim về durability phải gắn với **thực tế cấu hình replication và commit acknowledgement**.
