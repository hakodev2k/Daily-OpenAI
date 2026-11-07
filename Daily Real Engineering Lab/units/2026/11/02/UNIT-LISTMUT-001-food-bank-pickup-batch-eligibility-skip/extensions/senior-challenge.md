# Senior extension — ownership và kiểm soát nghiệp vụ

Sau khi hoàn thành bản cơ bản, cân nhắc hai requirement mới (không sửa test gốc khi chưa chốt contract):

1. Caller muốn giữ danh sách ban đầu để audit. Đề xuất API trả batch mới và nêu rõ ai sở hữu input/output; so sánh chi phí allocation và cách tránh sửa dữ liệu ngoài ý muốn.
2. Có một producer thêm lot vào cùng batch trong lúc lọc. Vì sao `List<T>` không phải collection thread-safe? Thiết kế ranh giới snapshot hoặc message queue; đề xuất test đồng thời có tính xác định thay vì dựa vào race ngẫu nhiên.

Ghi một ADR ngắn: lựa chọn, lý do, chi phí, các invariant cần kiểm chứng và chiến lược rollout.
