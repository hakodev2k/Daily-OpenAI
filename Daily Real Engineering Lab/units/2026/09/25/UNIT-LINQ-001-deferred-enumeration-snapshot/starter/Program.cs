var invoices=new List<Invoice>{new(1,true),new(2,true),new(3,false)};
var selected=invoices.Where(x=>x.Ready).Select(x=>x.Id);
var preview=selected.ToArray();
Console.WriteLine($"PreviewIds={string.Join(',',preview)}");
invoices.Add(new Invoice(4,true));
var processed=selected.ToArray();
Console.WriteLine($"ProcessedIds={string.Join(',',processed)}");
if(!preview.SequenceEqual(processed)) Environment.ExitCode=2;
record Invoice(int Id,bool Ready);