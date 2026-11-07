# Reference Solution — Spoiler

## 1. Symptoms
Cảnh báo hiển thị đầy đủ mã claim và part, nhưng filter theo trường `ClaimId`/`PartNumber` trả về 0.

## 2. Evidence
Trong `CapturingLogger.Log<TState>`, `state` không có cặp key/value nghiệp vụ ở starter dù `formatter` trả về đúng text.

## 3. Root cause
Starter dùng string interpolation tạo chuỗi hoàn chỉnh **trước** khi gọi `LogWarning`. Provider không thể khôi phục named attributes từ chuỗi đã render.

## 4. Why the fix works
[Program.cs](Program.cs) dùng template cố định `"Warranty hold {ClaimId} for part {PartNumber} in {Region}"` với `claimId, partNumber, region` là các tham số riêng. Provider nhận named fields và vẫn render đúng thông điệp.

## 5. How to verify
```powershell
./verify.ps1
dotnet run --project solution/ClaimLogLab.csproj -- --verify
```
Lệnh đầu kiểm tra starter người học sửa. Lệnh sau là self-check độc lập cho reference.

## 6. Alternative fixes
Source-generated `LoggerMessage` phù hợp với hot path và schema ổn định. `BeginScope` có thể mang thuộc tính request-scoped nhưng không thay thế event fields.

## 7. Wrong / tempting fixes
- Chỉ bật full-text search: không đảm bảo filter chính xác theo thuộc tính.
- Regex parse text sau ingest: mong manh khi format thay đổi, tốn chi phí.
- Đưa cả object vào một placeholder: có thể lộ PII hoặc tăng cardinality.
- Tăng log level: không tạo ra fields chưa được emit.

## 8. Production implications
Cần kiểm tra sink thực tế giữ named fields và mapping; quản lý PII, secrets, retention, sampling. Tránh dùng claim ID làm metric label có cardinality cao.

## 9. Trade-offs
Message templates giúp truy vấn và schema nhất quán, đổi lại cần quy ước tên trường và contract tests. Source-generated logging giảm allocations nhưng tăng ceremony.

## 10. Senior engineering observations
Test logging contract ở provider boundary và integration test ở sink thực tế. Alert nên xác nhận khả năng query theo trường quan trọng, không chỉ đếm warning.

## Official references
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [High-performance logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging)
