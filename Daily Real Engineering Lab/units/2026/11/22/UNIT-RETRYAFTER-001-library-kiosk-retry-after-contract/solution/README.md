# Reference solution (spoiler)

Root cause: HTTP `Retry-After` hỗ trợ delta-seconds **và** HTTP-date. Starter chỉ parse integer, nên ngày hợp lệ rơi vào fallback 5 giây, có thể retry sớm.

So sánh `solution/Program.cs` với starter. Parse HTTP-date RFC 1123 bằng `DateTimeOffset.TryParseExact(..., "r", CultureInfo.InvariantCulture, ...)`, trừ đồng hồ inject, clamp delay âm về 0; malformed header dùng fallback. Chép sửa đổi vào starter rồi chạy `./scripts/verify.ps1`.

Wrong fixes: tăng fallback cố định làm chậm request khác; dùng `DateTime.Now` khiến test lệ thuộc clock/timezone; parse theo CurrentCulture thiếu ổn định; retry 429 ngay làm tăng tải.

Production trade-offs: giới hạn delay, jitter, clock skew, cancellation, retry budget, và idempotency của request cần xử lý riêng. Lab này cô lập hợp đồng header.
