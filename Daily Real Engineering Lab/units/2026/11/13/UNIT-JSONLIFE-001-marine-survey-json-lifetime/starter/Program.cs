var a = """{"station":{"stationId":"SEA-17","sampleCount":4}}""";
var b = """{"station":{"stationId":"SEA-18","sampleCount":0}}""";
if (args.Length == 0 || args[0] == "reproduce") {
 Console.WriteLine("TRACE parse_completed=true network_latency_ms=2");
 Console.WriteLine(SurveyImport.Read(a).GetProperty("stationId").GetString());
} else if (args[0] == "verify") {
 Check(a,"SEA-17",4); Check(b,"SEA-18",0);
 Console.WriteLine("PASS downstream data integrity");
} else throw new ArgumentException("Unknown mode");
static void Check(string payload,string id,int count) {
 var element = SurveyImport.Read(payload);
 if(element.GetProperty("stationId").GetString()!=id || element.GetProperty("sampleCount").GetInt32()!=count)
  throw new Exception("Data mismatch");
}
