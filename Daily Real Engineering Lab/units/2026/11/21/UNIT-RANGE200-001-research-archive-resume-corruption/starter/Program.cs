using System;
using System.Linq;
using System.Text;
byte[] original=Encoding.ASCII.GetBytes("ARCHIVE-2026-COMPLETE");
byte[] partial=Encoding.ASCII.GetBytes("ARCHIVE-");
var reply=Origin.Fetch(original,partial.Length,ignoreRange:true);
byte[] result=Downloader.Resume(partial,reply);
Console.WriteLine($"status={reply.Status} offset={partial.Length} received={reply.Body.Length} final={result.Length}");
Console.WriteLine($"actual={Encoding.ASCII.GetString(result)} expected={Encoding.ASCII.GetString(original)}");
if(!result.SequenceEqual(original)){Console.Error.WriteLine("INTEGRITY_MISMATCH");return 2;}
Console.WriteLine("INTEGRITY_OK");return 0;
record Reply(int Status,byte[] Body,int? Start,int? Total);
static class Origin {
 public static Reply Fetch(byte[] data,int offset,bool ignoreRange)=>ignoreRange?new(200,data.ToArray(),null,data.Length):new(206,data[offset..],offset,data.Length);
}
static class Downloader {
 public static byte[] Resume(byte[] partial,Reply reply) {
  return partial.Concat(reply.Body).ToArray();
 }
}
