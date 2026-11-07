var invoices=new List<Invoice>{new(1,true),new(2,true),new(3,false)};
IEnumerable<Invoice> batch=invoices.Where(x=>x.Ready);
var audited=batch.Count(); Console.WriteLine($"AUDIT count={audited}");
invoices.Add(new Invoice(4,true));
var processed=batch.Select(x=>x.Id).ToArray(); Console.WriteLine($"PROCESSED {string.Join(',',processed)}");
if(audited!=processed.Length) Environment.ExitCode=2;
record Invoice(int Id,bool Ready);