using System;
using System.Collections.Generic;
using System.Linq;
record Alert(string Id,int Severity);
class Program {
 static readonly Alert[] Data={new("A1",5),new("A2",5),new("B1",3),new("A3",5),new("C1",1)};
 static List<Alert> Dispatch(IEnumerable<Alert> source) {
  var queue=new SortedSet<Alert>(Comparer<Alert>.Create((a,b)=>b.Severity.CompareTo(a.Severity)));
  foreach(var a in source) queue.Add(a);
  return queue.ToList();
 }
 static int Main(string[] args) {
  var result=Dispatch(Data);
  Console.WriteLine($"Input={Data.Length} Dispatched={result.Count} IDs={string.Join(",",result.Select(x=>x.Id))}");
  if(args.Contains("--reproduce")) return result.Count<Data.Length?0:1;
  if(args.Contains("--verify")) {
   bool Check(IEnumerable<Alert> src) {
    var expected=src.GroupBy(x=>x.Id).Select(x=>x.First()).OrderByDescending(x=>x.Severity).ThenBy(x=>x.Id,StringComparer.Ordinal).Select(x=>x.Id);
    return expected.SequenceEqual(Dispatch(src).Select(x=>x.Id));
   }
   bool ok=Check(Data)&&Check(Data.Reverse())&&Check(Data.Append(new("A1",5)));
   Console.WriteLine(ok?"VERIFY_PASS":"VERIFY_FAIL");
   return ok?0:2;
  }
  return 0;
 }
}
