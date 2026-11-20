# Incident timeline (giả lập)

| UTC | Event | Region | Evidence |
|---|---|---|---|
| 03:01 | CI build #840 | - | commit a1b2c3d; tag water-api:2026.11.20; digest sha256:aaa111 |
| 03:07 | Deploy | A | Pod imageID sha256:aaa111; label 2026.11.20 |
| 03:10 | CI rebuild #841 | - | commit e4f5a6b; push lại cùng tag; digest sha256:bbb222 |
| 03:14 | Deploy | B | Pod imageID sha256:bbb222; label 2026.11.20 |
| 03:17 | Alert | A/B | payload sensor=Q7,raw=8.46; A trả 8.5, B trả 8.46 |
| 03:19 | Registry | - | tag hiện trỏ sha256:bbb222 |

**Lưu ý:** Các digest rút gọn chỉ là nhãn mô phỏng, không phải SHA-256 hợp lệ để deploy.

Ràng buộc: RTO 15 phút; không mất dữ liệu; A 400 req/s, B 200 req/s. Chưa xác định output nào đúng. Registry có thể còn digest cũ nhưng chưa được xác nhận. CPU B cao hơn 8%; một DNS timeout lúc 02:50; schema DB đều version 42.

Nhiệm vụ: phân biệt nguyên nhân với tín hiệu nhiễu, chỉ ra evidence cần thu thập thêm, thiết kế containment và verify có thể rollback an toàn.
