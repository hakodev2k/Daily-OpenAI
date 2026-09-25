using System.Text;
long before=GC.GetTotalAllocatedBytes(true);
long total=0;
for(int round=0;round<20;round++){
 using var stream=new MemoryStream();
 using(var writer=new StreamWriter(stream,new UTF8Encoding(false),4096,true)){
  for(int i=0;i<12000;i++){if(i>0) writer.Write('\n'); writer.Write($"{i},customer-{i},status-{i%7}");}
 }
 total+=stream.Length;
}
long allocated=GC.GetTotalAllocatedBytes(true)-before;
Console.WriteLine($"totalBytes={total}");
Console.WriteLine($"allocated={allocated}");
Console.WriteLine($"gen2={GC.CollectionCount(2)}");