using Microsoft.Extensions.Primitives;
var samples = new (string Name, StringValues Headers)[] {
("single", new("north")), ("same-duplicate", new(new[]{"north","north"})),
("conflict", new(new[]{"north","south"})), ("missing", StringValues.Empty), ("empty", new(""))
};
foreach(var s in samples) Console.WriteLine($"{s.Name}|{Router.Route(s.Headers)}");
public static class Router {
 public static string Route(StringValues values) {
  if(values.Count != 1) return "REJECT";
  var zone=values[0];
  return zone is "north" or "south" ? $"ROUTE:{zone}" : "REJECT";
 }
}