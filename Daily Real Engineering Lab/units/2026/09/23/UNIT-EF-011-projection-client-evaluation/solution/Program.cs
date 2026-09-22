using Microsoft.EntityFrameworkCore;
using var db = new CustomerDb();
db.Database.EnsureDeleted(); db.Database.EnsureCreated();
if (!db.Customers.Any()) { for (var i=1;i<=5000;i++) db.Customers.Add(new Customer { Name=$"Customer {i}", Region=i%10==0?"APAC":"EU", Notes=new string('x',1000) }); db.SaveChanges(); }
var result = db.Customers.AsNoTracking().Where(x => x.Region == "APAC").Select(x => new { x.Id, x.Name }).Take(20).ToList();
Console.WriteLine($"Returned={result.Count}");
public sealed class Customer { public int Id {get;set;} public string Name {get;set;}=""; public string Region {get;set;}=""; public string Notes {get;set;}=""; }
public sealed class CustomerDb : DbContext { public DbSet<Customer> Customers => Set<Customer>(); protected override void OnConfiguring(DbContextOptionsBuilder b)=>b.UseSqlite("Data Source=customers.db").LogTo(Console.WriteLine); }