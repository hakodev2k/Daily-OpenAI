# Reference Solution — chỉ xem sau khi đã tự reproduce và thử sửa

## Symptom
Lịch `09:00 America/New_York` trước DST được convert đúng thành `14:00Z`, nhưng sau khi DST bắt đầu starter vẫn tạo `14:00Z` thay vì `13:00Z`.

## Root cause
Code dùng fixed offset `UTC-05:00` để đại diện cho timezone New York. Offset không chứa DST rules nên chỉ đúng trong Standard Time.

## Fix
Resolve wall-clock time bằng `TimeZoneInfo`:

```csharp
static TimeZoneInfo GetNewYorkTimeZone()
{
    try
    {
        return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
    }
    catch (TimeZoneNotFoundException)
    {
        return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    }
}

static DateTimeOffset ScheduleAtNewYorkNine(DateOnly date)
{
    var zone = GetNewYorkTimeZone();
    var wallClock = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Unspecified);
    var utc = TimeZoneInfo.ConvertTimeToUtc(wallClock, zone);
    return new DateTimeOffset(utc, TimeSpan.Zero);
}
```

## Why it works
Timezone chứa transition rules theo ngày; fixed offset thì không. `DateTimeKind.Unspecified` ở đây biểu diễn wall-clock value, còn `TimeZoneInfo` cung cấp timezone semantics để chuyển thành UTC instant.

## Wrong fixes
- Hard-code `UTC-04:00`: sẽ sai vào Standard Time.
- Dùng timezone của server: deployment location không phải business timezone.
- Cộng/trừ một giờ theo tháng: DST rules thay đổi theo jurisdiction và lịch sử.

## Production implications
Temporal bugs thường chỉ xuất hiện vài lần mỗi năm, khó reproduce và dễ gây duplicate/missed jobs, billing sai kỳ hoặc notification sai giờ.

## Senior takeaway
Phân biệt rõ ba khái niệm: wall-clock local time, timezone rules và UTC instant. Không dùng fixed offset thay cho timezone khi business requirement phụ thuộc vào local civil time.
