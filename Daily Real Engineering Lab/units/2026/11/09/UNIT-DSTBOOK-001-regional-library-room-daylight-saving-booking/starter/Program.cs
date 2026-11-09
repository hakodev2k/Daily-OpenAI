using System.Globalization;
public record BookingRequest(string LocalStart, string ZoneId, string? Offset);
public record BookingResult(string Status, string? Utc, string? Error);
public static class BookingService
{
 public static BookingResult Book(BookingRequest request)
 {
  if (!DateTime.TryParseExact(request.LocalStart, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
   return new("REJECTED", null, "invalid-format");
  var local = DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified);
  TimeZoneInfo zone;
  try { zone = TimeZoneInfo.FindSystemTimeZoneById(request.ZoneId); }
  catch (TimeZoneNotFoundException) { return new("REJECTED", null, "unknown-zone"); }
  catch (InvalidTimeZoneException) { return new("REJECTED", null, "invalid-zone"); }
  var utc = TimeZoneInfo.ConvertTimeToUtc(local, zone);
  return new("CONFIRMED", utc.ToString("O", CultureInfo.InvariantCulture), null);
 }
}
public static class Program
{
 public static int Main(string[] args)
 {
  if (args.Length != 3) { Console.Error.WriteLine("Usage: <yyyy-MM-dd HH:mm> <zone> <offset-or-dash>"); return 2; }
  try
  {
   var result = BookingService.Book(new BookingRequest(args[0], args[1], args[2] == "-" ? null : args[2]));
   Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result));
   return 0;
  }
  catch (Exception ex)
  {
   Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new BookingResult("ERROR", null, ex.GetType().Name)));
   return 1;
  }
 }
}
