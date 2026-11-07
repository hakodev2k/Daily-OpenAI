# Before — starter nguyên gốc

Chạy `./scripts/reproduce.ps1`:
- Ba lần export có tài liệu đều trả về byte[] không rỗng, nhưng reader báo `InvalidContainer`.
- Gói không có entry cũng báo `InvalidContainer`.
- Dòng cuối là `REPRODUCED: invalid ZIP containers`; exit code 0 nghĩa là tái hiện thành công.
- Không phụ thuộc kích thước byte chính xác theo máy hoặc SDK patch.

Nếu thay vì `InvalidContainer` bạn thấy lỗi build, kiểm tra .NET 8 SDK và dùng bản starter nguyên gốc.
