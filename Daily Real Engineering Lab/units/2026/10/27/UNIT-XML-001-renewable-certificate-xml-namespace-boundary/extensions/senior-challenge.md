# Senior extension — versioned XML ingress

Yêu cầu mới: đối tác vẫn gửi v1 trong 90 ngày, v2 có thể có tới 200.000 meter/batch, và mỗi batch phải được audit.

Thiết kế adapter v1/v2 cùng đưa về một canonical model. Quy định chính sách reject/quarantine, giới hạn kích thước và số phần tử, streaming `XmlReader`, bảo vệ chống entity expansion, metrics theo version, kiểm thử contract, rollback khi schema rollout sai và cách xử lý batch import dở dang. Viết ADR ngắn so sánh DOM vs streaming, strict reject vs compatibility window. Không bắt buộc triển khai toàn bộ extension để hoàn thành lab chính.
