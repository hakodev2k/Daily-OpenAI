using ValueTaskLab;
var p=new PricingProcessor(new RateProvider());
foreach(var slow in new[]{false,true}) {
 var n=slow?"SLOW":"FAST";
 try { Console.WriteLine($"{n}: price={await p.ProcessAsync(100m,slow)}"); }
 catch(Exception e){Console.WriteLine($"{n}: {e.GetType().Name}: {e.Message}");}
}