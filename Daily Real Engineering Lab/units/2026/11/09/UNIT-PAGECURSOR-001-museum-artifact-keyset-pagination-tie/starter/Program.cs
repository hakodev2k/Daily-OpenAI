using System;
using System.Collections.Generic;
using System.Linq;
record Artifact(int Id, DateTime UpdatedAt, string Label);
record Cursor(DateTime UpdatedAt);
static class Exporter {
 public static IReadOnlyList<Artifact> Next(IReadOnlyList<Artifact> rows, Cursor? cursor, int size) {
  var ordered = rows.OrderBy(x => x.UpdatedAt).ThenBy(x => x.Id);
  var eligible = cursor is null ? ordered : ordered.Where(x => x.UpdatedAt > cursor.UpdatedAt);
  return eligible.Take(size).ToArray();
 }
 public static Cursor FromLast(Artifact row) => new(row.UpdatedAt);
}
class Program {
 static int Main(string[] args) {
  var t = new DateTime(2026,11,9,0,0,0,DateTimeKind.Utc);
  var rows = new[] { new Artifact(10,t,"Bell"),new Artifact(11,t,"Bowl"),new Artifact(12,t,"Tablet"),new Artifact(13,t.AddSeconds(1),"Seal"),new Artifact(14,t.AddSeconds(2),"Rubbing") };
  var seen = new List<int>(); Cursor? cursor=null;
  for(int page=0;page<10;page++) { var batch=Exporter.Next(rows,cursor,2); if(batch.Count==0)break;
   Console.WriteLine($"page={page+1} ids={string.Join(",",batch.Select(x=>x.Id))}");
   seen.AddRange(batch.Select(x=>x.Id)); cursor=Exporter.FromLast(batch[^1]); }
  bool complete=seen.SequenceEqual(new[]{10,11,12,13,14});
  Console.WriteLine($"exported={seen.Count} expected={rows.Length} complete={complete}");
  if(args.Contains("--verify")) { if(!complete || seen.Distinct().Count()!=rows.Length)return 1; Console.WriteLine("PASS"); }
  return 0;
 }
}
