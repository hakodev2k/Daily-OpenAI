namespace ValueTaskLab;
public sealed class PricingProcessor(IRateProvider rates) {
 public async Task<decimal> ProcessAsync(decimal price,bool slow) {
  var rate=await rates.GetRateAsync(slow);
  var result=price*rate;
  Console.WriteLine($"telemetry.rate={rate}");
  return result;
 }
}