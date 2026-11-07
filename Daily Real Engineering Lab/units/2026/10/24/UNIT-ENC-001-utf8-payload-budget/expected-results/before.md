# Before — starter chưa sửa
- `./reproduce.ps1` exit code 0, in `REPRODUCED`.
- `vietnamese-over-boundary`, `emoji-over-boundary` và `order-notice` được local policy chấp nhận nhưng provider trả 413.
- `./verify.ps1` exit code 1 vì có case bị accept sai.
- `ascii-exact-boundary` vẫn được chấp nhận và provider trả 202.
Nếu không reproduce được, kiểm tra .NET 8 SDK, đúng working directory, và không chỉnh `starter/PayloadPolicy.cs` trước khi reproduce.
