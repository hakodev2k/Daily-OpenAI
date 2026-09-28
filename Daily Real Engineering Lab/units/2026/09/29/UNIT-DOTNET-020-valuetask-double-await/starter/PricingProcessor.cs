namespace ValueTaskLab;
public sealed class PricingProcessor(IRateProvider rates) {
 public async Task<decimal> ProcessAsync(decimal price,bool slow) {
  var pending=rates.GetRateAsync(slow);
  var rateForPrice=await pending;
  var result=price*rateForPrice;
  var rateForTelemetry=await pending;
  Console.WriteLine($"telemetry.rate={rateForTelemetry}");
  return result;
 }
}