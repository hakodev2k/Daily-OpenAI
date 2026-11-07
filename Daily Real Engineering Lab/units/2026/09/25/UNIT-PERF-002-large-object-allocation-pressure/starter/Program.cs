using System.Text;
long before=GC.GetTotalAllocatedBytes(true);
int checksum=0;
for(int round=0;round<20;round++){
 var rows=Enumerable.Range(0,12000).Select(i=>$"{i},customer-{i},status-{i%7}");
 var text=string.Join("\n",rows);
 var bytes=Encoding.UTF8.GetBytes(text);
 checksum=HashCode.Combine(checksum,bytes.Length,bytes[0]);
}
long allocated=GC.GetTotalAllocatedBytes(true)-before;
Console.WriteLine($"checksum={checksum}");
Console.WriteLine($"allocated={allocated}");
Console.WriteLine($"gen2={GC.CollectionCount(2)}");