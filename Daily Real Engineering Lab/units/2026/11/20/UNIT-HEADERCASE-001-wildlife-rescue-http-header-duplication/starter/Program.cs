using Microsoft.Extensions.Primitives;
var samples = new (string Name, StringValues Headers)[] {
("single", new("north")), ("same-duplicate", new(new[]{"north","north"})),
("conflict", new(new[]{"north","south"})), ("missing", StringValues.Empty), ("empty", new(""))
};
foreach(var s in samples) Console.WriteLine($"{s.Name}|{Router.Route(s.Headers)}");
public static class Router {
 public static string Route(StringValues values) {
  var first = values.ToString().Split(',',StringSplitOptions.TrimEntries)[0];
  return first is "north" or "south" ? $"ROUTE:{first}" : "REJECT";
 }
}