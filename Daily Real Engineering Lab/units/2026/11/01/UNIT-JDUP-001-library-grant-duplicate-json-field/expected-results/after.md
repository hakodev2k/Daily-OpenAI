# Sau khi sửa

- `./scripts/verify.ps1` chạy các contract case trên **starter đã được học viên sửa**, không phải thư mục `solution/`.
- Các yêu cầu hợp lệ 4200 và 5000 được chấp nhận, số tiền ghi nhận đúng bằng số tiền đã duyệt.
- 5001, 0, JSON trùng property, sai casing, unknown field, thiếu trường, số lẻ, ID trống, non-object và malformed JSON bị từ chối mà không phát sinh exception ra caller.
- Contract runner trả exit code 0, in `failures: 0`.
- Nếu kết quả không giống kỳ vọng: kiểm tra đang chạy đúng `starter/`, đúng .NET SDK và không sửa `checks/`.
