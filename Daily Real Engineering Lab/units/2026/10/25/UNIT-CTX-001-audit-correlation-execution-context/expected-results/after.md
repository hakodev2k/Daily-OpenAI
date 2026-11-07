# Sau sửa

- `./verify.ps1` kết thúc với exit code 0 và `PASS: isolated audit correlation`.
- CASE-A có `audit=corr-risk-A-2042`, CASE-B có `audit=corr-risk-B-2043`; `control` vẫn đúng.
- Không phụ thuộc thứ tự thực thi và không chia sẻ correlation ID giữa các request.
- Có thể tự kiểm tra reference: `dotnet run --project ./solution/CorrelationLab.csproj -- --verify`.
- Sau khi sửa, `reproduce.ps1` có thể không còn thành công: script đó chỉ chứng minh lỗi gốc, không phải kiểm tra fix.
