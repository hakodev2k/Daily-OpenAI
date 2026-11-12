using System;
using System.Threading.Tasks;
internal static class Program {
 static async Task<string> Probe(string name,int ms,bool ok) { await Task.Delay(ms); if(!ok)throw new InvalidOperationException(name+":unavailable"); return name; }
 static async Task<string> Select(bool outage) { Task<string>[] probes=[Probe("primary",20,false),Probe("backup",80,!outage)]; var first=await Task.WhenAny(probes); return await first; }
 static async Task<int> Main(string[] args) { bool outage=args.Length>0&&args[0]=="all-failed"; try { var value=await Select(outage); Console.WriteLine("DELIVERED:"+value); return 0; } catch(AggregateException e) { Console.WriteLine("ALL_FAILED_HANDLED:"+e.InnerExceptions.Count); return 0; } catch(Exception e) { Console.Error.WriteLine("UNAVAILABLE:"+e.Message); return 1; } }
}
