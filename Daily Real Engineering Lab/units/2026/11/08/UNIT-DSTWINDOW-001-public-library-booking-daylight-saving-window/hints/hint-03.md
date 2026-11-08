# Hint 3
Dùng `TimeZoneInfo.FindSystemTimeZoneById("America/New_York")` và `GetUtcOffset`/ `ConvertTimeToUtc` cho local `DateTimeKind.Unspecified`. Định nghĩa chính sách cho invalid/ambiguous local time trước khi tổng quát hóa.
