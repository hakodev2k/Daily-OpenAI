# Investigation Guide — chưa mở solution

## Tình huống cần phân biệt
1. Đối tác có thực sự chậm ở kết nối, headers hay body?
2. HTTP status 200 có đồng nghĩa operation đã hoàn tất đúng SLA không?
3. Token do caller hủy có tác dụng trong mọi pha của operation không?
4. Budget được đo cho cả công việc hay chỉ một phần công việc?

## Quy trình
- Chạy `./reproduce.ps1` trước khi chỉnh code; lưu ba dòng FAST/SLOW_BODY/CALLER_CANCEL.
- Đọc `starter/PartnerAttachmentClient.cs`, `harness/Program.cs` và evidence. Lập ba giả thuyết cạnh tranh.
- Xác định chính xác pha nào của HTTP operation được đo và pha nào còn hoạt động sau khi nhận headers.
- Chỉnh duy nhất client. Giữ nguyên method signature và HTTP response validation.
- Chạy `./verify.ps1`, giải thích vì sao cả timeout và caller cancellation đều đạt.
- Đề xuất telemetry production: download duration, headers latency, body latency, cancellation reason, byte count.

Không xem `solution/` trước khi ghi giả thuyết.
