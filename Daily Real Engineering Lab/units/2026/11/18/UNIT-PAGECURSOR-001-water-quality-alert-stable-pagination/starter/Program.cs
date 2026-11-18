using System;
using System.Collections.Generic;
using System.Linq;
var alerts = new List<Alert> { new(101,600,"A"), new(102,500,"B"), new(103,500,"C"), new(104,400,"D"), new(105,300,"E"), new(106,200,"F") };
var expected = alerts.Select(x=>x.Id).ToHashSet();
var collected = new List<int>();
var session = AlertPager.Start(alerts);
for (var page=0;page<5;page++) {
 var batch = AlertPager.ReadPage(alerts, session, page, 3);
 if(batch.Count==0) break;
 Console.WriteLine($"Page {page}: {string.Join(",",batch.Select(x=>x.Id))}");
 collected.AddRange(batch.Select(x=>x.Id));
 if(page==0) { alerts.Add(new Alert(107,650,"New reading")); Console.WriteLine("New alert arrived."); }
}
Console.WriteLine($"Duplicates={collected.Count-collected.Distinct().Count()}; Missing={string.Join(",",expected.Except(collected))}");
