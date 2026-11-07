# Reference Solution — spoiler

**Chỉ xem sau khi reproduce và tự sửa.**

## Symptoms
Hai record JSONL vẫn có thể parse được nhưng text tiếng Việt và emoji xuất hiện U+FFFD tùy ranh giới các lần đọc.

## Evidence
Fixture giữ nguyên payload giữa single-read, split-inside-character và one-byte-per-read. Chỉ việc chia frame làm thay đổi output. JSON parser chấp nhận replacement character nên test syntax đơn thuần chưa đủ.

## Root cause
Baseline giải mã `Encoding.UTF8.GetString(frame)` độc lập trên mỗi mảnh. Sequence UTF-8 nhiều byte bị cắt qua hai lần đọc mất trạng thái, được thay bằng U+FFFD.

## Why the fix works
Reference dùng một `Decoder` cho cả luồng, `GetChars(...,flush:false)` trên từng frame, `flush:true` tại EOF. `UTF8Encoding(false,true)` báo lỗi khi wire không hợp lệ; JSONL framing thực hiện trên text đã giải mã.

## How to verify
```powershell
./verify.ps1
dotnet run --project solution/Utf8StreamLab.csproj -- --verify
./reproduce.ps1
```
Lệnh đầu xác nhận learner starter; lệnh thứ hai xác nhận reference; lệnh cuối giữ baseline.

## Alternative fixes
`StreamReader` có decoder xuyên các lần đọc nếu nguồn là `Stream`; `System.IO.Pipelines` phù hợp hiệu năng cao; tích lũy byte tới newline rồi strict-decode dễ hiểu nhưng cần max-line bound.

## Wrong / misleading fixes
Đổi sang ASCII làm mất dữ liệu; normalize text sau U+FFFD không thể khôi phục byte; tăng chunk size chỉ giảm xác suất; bỏ strict fallback che lỗi nguồn.

## Production implications
Một decoder riêng cho mỗi connection, max-line bytes, metrics decode failures, kiểm tra EOF và không log toàn bộ payload nhạy cảm.

## Trade-offs
Stateful decoder tiết kiệm việc giữ cả payload nhưng đòi hỏi lifecycle rõ ràng; strict rejection cần chính sách retry/quarantine. Chọn giải pháp tối giản phù hợp throughput.

## Senior engineer should notice
JSON hợp lệ không bảo đảm text integrity; read boundary không phải message boundary. Kiểm thử nên độc lập với kích thước chunk và bao gồm malformed input.
