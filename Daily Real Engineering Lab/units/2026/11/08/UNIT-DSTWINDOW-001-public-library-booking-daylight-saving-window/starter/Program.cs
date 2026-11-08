using BookingLab;
using System.Globalization;
var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
var cases = new (DateOnly BookingDate, string ExpectedUtc)[]
{
    (new DateOnly(2026, 3, 14), "2026-03-07T14:00:00+00:00"),
    (new DateOnly(2026, 3, 16), "2026-03-09T13:00:00+00:00"),
    (new DateOnly(2026, 11, 7), "2026-10-31T13:00:00+00:00"),
    (new DateOnly(2026, 11, 9), "2026-11-02T14:00:00+00:00")
};
bool inspect = args.Contains("--inspect");
int failures = 0;
foreach (var (bookingDate, expectedText) in cases)
{
    var actual = BookingWindow.OpenAtUtc(bookingDate);
    var expected = DateTimeOffset.Parse(expectedText, CultureInfo.InvariantCulture);
    var local = TimeZoneInfo.ConvertTime(actual, zone);
    var pass = actual == expected && local.Hour == 9 && local.Minute == 0;
    Console.WriteLine($"booking={bookingDate:yyyy-MM-dd} actualUTC={actual:O} local={local:yyyy-MM-dd HH:mm zzz} expectedUTC={expected:O} {(pass ? "PASS" : "FAIL")}");
    if (!pass) failures++;
}
Console.WriteLine($"failed={failures}");
if (!inspect && failures != 0) Environment.ExitCode = 1;
