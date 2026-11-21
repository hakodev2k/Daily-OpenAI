using System;
using System.Threading;
var job = new BillingJob();
using var timer = new Timer(_ => job.Tick(), null, Timeout.Infinite, Timeout.Infinite);
timer.Change(0, Timeout.Infinite);
job.WaitStarted();
timer.Change(0, Timeout.Infinite);
Thread.Sleep(300);
job.Release();
Thread.Sleep(300);
Console.WriteLine($"MAX_ACTIVE={job.MaxActive}; POSTED={job.Posted}; CYCLES={job.Cycles}");
if(job.MaxActive>1 || job.Posted!=1) Environment.ExitCode=2;
sealed class BillingJob {
 readonly ManualResetEventSlim started=new(false), release=new(false);
 int active,maxActive,posted,cycles; 
 public int MaxActive=>Volatile.Read(ref maxActive);
 public int Posted=>Volatile.Read(ref posted);
 public int Cycles=>Volatile.Read(ref cycles);
 public void WaitStarted(){if(!started.Wait(TimeSpan.FromSeconds(5)))throw new TimeoutException("no callback");}
 public void Release()=>release.Set();
 public void Tick(){
 
 try { int n=Interlocked.Increment(ref active); int old;
 do {old=Volatile.Read(ref maxActive);if(old>=n)break;}while(Interlocked.CompareExchange(ref maxActive,n,old)!=old);
 Interlocked.Increment(ref cycles);started.Set();
 if(!release.Wait(TimeSpan.FromSeconds(5)))throw new TimeoutException("gateway blocked");
 Interlocked.Increment(ref posted);
 }finally{Interlocked.Decrement(ref active);}
 }
}
