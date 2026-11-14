using System;
using System.Collections.Generic;
using System.Linq;
record Ticket(string Id,int Severity,long Sequence);
static class Scheduler {
 public static string[] Dispatch(IEnumerable<Ticket> tickets) {
  var q=new PriorityQueue<Ticket,(int,long)>();
  foreach(var t in tickets) q.Enqueue(t,(t.Severity,t.Sequence));
  var result=new List<string>();
  while(q.TryDequeue(out var t,out _)) result.Add(t.Id);
  return result.ToArray();
 }
}
var input=new[]{new Ticket("A",2,1),new Ticket("B",2,2),new Ticket("C",2,3),new Ticket("D",2,4),new Ticket("URGENT",1,5)};
var expected=input.OrderBy(x=>x.Severity).ThenBy(x=>x.Sequence).Select(x=>x.Id).ToArray();
var actual=Scheduler.Dispatch(input);
Console.WriteLine("Expected: "+string.Join(",",expected));
Console.WriteLine("Actual: "+string.Join(",",actual));
bool pass=actual.SequenceEqual(expected);
Console.WriteLine(pass?"CONTRACT PASS":"CONTRACT FAIL");
return args.Contains("--verify")?(pass?0:1):(pass?1:0);
