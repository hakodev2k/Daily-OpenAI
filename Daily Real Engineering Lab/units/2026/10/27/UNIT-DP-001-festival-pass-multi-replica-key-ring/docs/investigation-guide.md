# Investigation Guide

1. Ghi rõ process phát hành token và process đọc lại; chỉ dùng output của harness làm bằng chứng.
2. So sánh giả thuyết: token hỏng, sai purpose, state theo process, key không đồng bộ, clock drift.
3. Kiểm tra cả kết quả positive (cross-replica, restart) và negative (purpose khác, storage khác).
4. Tự sửa duy nhất file learner-editable rồi chạy verify. Đừng chép solution trước khi có hypothesis.
5. Đề xuất cách triển khai trên Kubernetes: nơi lưu key ring, quyền truy cập, mã hóa key at rest, backup, rotation và rollback.

## Lưu ý

Môi trường giả lập chỉ chứng minh hợp đồng giữa các process; nó không tự xác nhận cấu hình K8s hay độ an toàn của storage production.
