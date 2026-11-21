# Hồ sơ kiến trúc và constraints

- Người dùng: phụ huynh có quan hệ với học sinh; quan hệ có thể bị thu hồi trong phiên.
- Blob storage private, server-side encryption; mỗi PDF trung bình 6 MiB, p95 22 MiB.
- Peak 90 downloads/s trong 15 phút; 2 App Service instances hiện tại.
- SLO: 99,9% lượt yêu cầu cấp quyền có kết quả trong 800 ms; download hoàn tất phụ thuộc mạng client.
- Chính sách: không public container, không lưu credential trong browser, không tiết lộ blob path dễ đoán, audit ai được cấp quyền truy cập, thời gian và đối tượng.
- Revocation: sau khi admin thu hồi, không được **bắt đầu** một download mới quá 60 giây; xác định rõ download đang truyền có được tiếp tục hay không.
- Hạn mức: tối đa 2 ngày phát triển và không tăng hơn 20% chi phí hàng tháng.
- Chưa có CDN; tệp có thông tin cá nhân và không được cache ở shared proxy.
- Token có thể bị copy ra ngoài qua log, referrer hoặc browser history.
- Kiến trúc phải có giải pháp khi identity provider, storage hoặc audit sink lỗi.

**Deliverables:** ADR 1 trang, threat model 5 tình huống, bảng chi phí định tính, ít nhất 3 failure-path tests, rollback checklist. Không giả định SAS tự hỗ trợ thu hồi ngay từng token.
