# Tabletop verification contract

Không có script thực thi; dùng bảng sự kiện để replay bằng tay và ghi bằng chứng.

| Case | Injection | Điều kiện PASS |
|---|---|---|
| V1 | API write lúc 08:59:59 ACK trước cutover | Xuất hiện ở destination sau sync; không mất dữ liệu ACK |
| V2 | API write 09:00:01 từ node cache cũ | Bị fence hoặc route đúng EU-Central, không commit ở EU-West |
| V3 | Worker giữ message từ 08:59, chạy 09:02 | Không ghi artifact cá nhân mới ở EU-West |
| V4 | Worker crash sau DB commit trước queue ACK | Retry không nhân đôi business side effect |
| V5 | Message duplicate + out-of-order | Dedup bền vững, không ghi lùi phiên bản |
| V6 | Object copy thành công, DB metadata chưa sync | Không expose object sai phiên bản hoặc dangling reference |
| V7 | EU-Central không theo kịp watermark ở phút cutover | No-go hoặc tiếp tục hold; không tuyên bố RPO=0 |
| V8 | EU-Central mất kết nối sau cutover | Không tự động failback ghi EU-West trái residency policy |

## Acceptance evidence
- Chứng minh mọi write path kiểm tra authoritative epoch/home ở write boundary.
- Đối soát số records, object manifest, checksums, eventId và committed high-watermark.
- Kiểm chứng retry, idempotency và quarantine có owner/alert.
- Ghi điều kiện rollback/fail-forward, người phê duyệt, dashboard và runbook.
- Đánh giá downtime tối đa 5 phút; nếu không đạt, ghi rõ trade-off và phương án giảm rủi ro.

## Đánh giá
Đạt khi cả V1–V8 có kết quả và bằng chứng/giả định rõ ràng; không đạt nếu chỉ đổi router hoặc dựa vào API error rate.