using InventoryLab;
var stock=new List<StockItem>{new("RICE",12),new("BEANS",8)};
var report=InventoryReport.Build(stock,()=>stock[0].Quantity+=5);
int detail=report.Lines.Sum(x=>x.Quantity);
Console.WriteLine($"Detail={detail}; Footer={report.Total}");
string mode=args.FirstOrDefault()??"reproduce";
if(mode=="reproduce"){bool mismatch=detail!=report.Total;Console.WriteLine(mismatch?"REPRODUCED":"NOT REPRODUCED");return mismatch?0:1;}
if(mode=="verify"){bool ok=detail==report.Total&&report.Total==20&&report.Lines.Count==2&&report.Lines[0].Quantity==12;Console.WriteLine(ok?"VERIFIED":"FAILED");return ok?0:1;}
return 2;
