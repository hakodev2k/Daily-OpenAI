# Before — starter

- `reproduce.ps1` kết thúc thành công với thông báo `REPRODUCED`.
- Tải đầy đủ trả 200 và 10 byte.
- Tải đoạn `bytes=3-6` trả 206 và metadata mô tả đoạn được yêu cầu, nhưng body khác với bốn byte client kỳ vọng.
- Các trường hợp một byte, open-ended và suffix giúp khoanh vùng ranh giới.
- `verify.ps1` phải FAIL trên starter chưa sửa; test không được chạy trên `solution/`.
- Nếu không tái hiện: kiểm tra `dotnet --version`, thử `./scripts/setup.ps1`, bảo đảm loopback không bị policy chặn và đọc output chi tiết.
