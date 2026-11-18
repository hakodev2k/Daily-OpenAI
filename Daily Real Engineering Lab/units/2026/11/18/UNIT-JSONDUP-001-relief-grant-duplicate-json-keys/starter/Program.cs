using System.Text.Json;
var raw=args.Contains("--valid") ? """{"CaseId":"A1","Amount":500}""" : """{"CaseId":"A1","Amount":500,"Amount":900}""";
var item=Read(raw);Console.WriteLine($"AMOUNT={item.Amount}");
if(args.Contains("--assert")&&!args.Contains("--valid"))throw new Exception("Ambiguous JSON accepted");
static Grant Read(string json) {
    var result=JsonSerializer.Deserialize<Grant>(json)??throw new InvalidDataException("Empty");
    if(result.Amount<=0||string.IsNullOrEmpty(result.CaseId))throw new InvalidDataException("Invalid");
    return result;
}
record Grant(string CaseId,long Amount);