using System.Threading.Tasks.Sources;
namespace ValueTaskLab;
public interface IRateProvider { ValueTask<decimal> GetRateAsync(bool slow); }
public sealed class RateProvider : IRateProvider {
 public ValueTask<decimal> GetRateAsync(bool slow) {
  if (!slow) return new ValueTask<decimal>(1.25m);
  var s=new Source(); s.Start(); return new ValueTask<decimal>(s,s.Version);
 }
 private sealed class Source:IValueTaskSource<decimal> {
  private ManualResetValueTaskSourceCore<decimal> core; private int consumed;
  public short Version=>core.Version;
  public void Start(){core.RunContinuationsAsynchronously=true; _=Complete();}
  private async Task Complete(){await Task.Delay(80);core.SetResult(1.25m);}
  public decimal GetResult(short t){if(Interlocked.Exchange(ref consumed,1)!=0)throw new InvalidOperationException("Result consumed more than once.");return core.GetResult(t);}
  public ValueTaskSourceStatus GetStatus(short t)=>core.GetStatus(t);
  public void OnCompleted(Action<object?> c,object? s,short t,ValueTaskSourceOnCompletedFlags f)=>core.OnCompleted(c,s,t,f);
 }
}