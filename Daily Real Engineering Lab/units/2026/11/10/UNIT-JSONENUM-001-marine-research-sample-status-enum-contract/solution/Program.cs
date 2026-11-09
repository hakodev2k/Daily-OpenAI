using System.Text.Json;
using System.Text.Json.Serialization;
public enum SampleStatus { Collected=1, Quarantined=2, Ready=3 }
public sealed record SampleUpdate(string SampleId, SampleStatus Status);
public static class Intake {
 public static SampleUpdate Parse(string json) {
  var options=new JsonSerializerOptions();
  options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
  var update=JsonSerializer.Deserialize<SampleUpdate>(json,options) ?? throw new JsonException("Empty update");
  if (!Enum.IsDefined(update.Status)) throw new JsonException("Unknown status");
  return update;
 }
}
public static class Program {
 public static int Main() {
  string[] inputs={ """{"SampleId":"SEA-41","Status":"Ready"}""", """{"SampleId":"SEA-42","Status":"Quarantined"}""", """{"SampleId":"SEA-43","Status":99}""", """{"SampleId":"SEA-44","Status":"Unexpected"}""" };
  bool[] expected={true,true,false,false}; int failures=0;
  for(int i=0;i<inputs.Length;i++) {
   bool accepted; string result;
   try { var update=Intake.Parse(inputs[i]); accepted=true; result=$"{update.SampleId}: {update.Status} ({(int)update.Status})"; }
   catch(JsonException ex) { accepted=false; result=ex.GetType().Name; }
   bool pass=accepted==expected[i]; if(!pass) failures++;
   Console.WriteLine($"case={i+1} accepted={accepted} expected={expected[i]} {(pass?"PASS":"FAIL")} {result}");
  }
  Console.WriteLine($"contract_failures={failures}"); return failures==0?0:1;
 }
}
