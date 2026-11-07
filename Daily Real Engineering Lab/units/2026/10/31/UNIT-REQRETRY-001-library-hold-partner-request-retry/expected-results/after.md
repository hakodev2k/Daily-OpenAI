# After — starter đã được sửa

- Verify trả exit code 0, in năm dòng PASS và một dòng VERIFIED.
- Partner nhận hai request tương đương cho 503 → 202; kết quả cuối là 202.
- Partner chỉ nhận một request cho 400; client báo HttpRequestException.
- Partner chỉ nhận một request cho 202 ngay lần đầu.
- 503 → 503 dừng sau hai request và báo HttpRequestException.
- Token đã hủy trước khi gửi không tạo request tại partner.
- Không yêu cầu timing cố định; quan sát status, attempt count và request contract.
