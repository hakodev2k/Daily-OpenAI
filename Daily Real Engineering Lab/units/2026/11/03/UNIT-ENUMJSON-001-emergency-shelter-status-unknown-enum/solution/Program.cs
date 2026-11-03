using System.Text.Json;
enum Status { Open=1, Full=2, Closed=3 }
record Feed(string ShelterId, Status Status);
class Program {
 static Feed Parse(string json) => Validate(JsonSerializer.Deserialize<Feed>(json) ?? throw new JsonException("Empty feed"));
 static Feed Validate(Feed f) => Enum.IsDefined(f.Status) ? f : throw new JsonException($"Unknown status {(int)f.Status}");
 static string Route(Feed f) => f.Status switch { Status.Open=>"AVAILABLE", Status.Full=>"WAITLIST", Status.Closed=>"CLOSED", _=>"IGNORED" };
 static int Main(string[] args) {
  const string drift = """{"ShelterId":"S-42","Status":9}""";
  if (args.FirstOrDefault()=="reproduce") { var f=Parse(drift); Console.WriteLine($"status={(int)f.Status}; route={Route(f)}"); return Route(f)=="IGNORED"?0:1; }
  if (args.FirstOrDefault()!="verify") return 2;
  int failures=0;
  foreach(var (json, expected) in new[] { ("""{"ShelterId":"S-42","Status":1}""","AVAILABLE"), ("""{"ShelterId":"S-42","Status":3}""","CLOSED") }) {
   try { if(Route(Parse(json))!=expected) failures++; } catch { failures++; }
  }
  foreach(var json in new[] { drift, """{"ShelterId":"S-42","Status":"unknown"}""" }) {
   try { Parse(json); failures++; Console.WriteLine("FAIL invalid status accepted"); }
   catch(JsonException) { Console.WriteLine("PASS invalid status rejected"); }
  }
  Console.WriteLine($"failures={failures}"); return failures==0?0:1;
 }
}
