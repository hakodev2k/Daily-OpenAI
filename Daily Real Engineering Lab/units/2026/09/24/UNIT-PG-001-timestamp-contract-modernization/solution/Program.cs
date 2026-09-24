using System.Globalization;
var rows=new[]{"2026-09-24 01:15:00","2026-09-24 09:45:00"};
var migrated=rows.Select(x=>DateTime.SpecifyKind(DateTime.ParseExact(x,"yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture),DateTimeKind.Utc));
foreach(var instant in migrated) Console.WriteLine(instant.ToString("O"));