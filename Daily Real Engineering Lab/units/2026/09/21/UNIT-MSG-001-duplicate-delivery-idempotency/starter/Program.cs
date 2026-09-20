record Message(Guid Id,int OrderId); record Shipment(int OrderId);
sealed class Store { public List<Shipment> Items {get;}=[]; }
sealed class Consumer(Store s) { public Task Handle(Message m){ s.Items.Add(new(m.OrderId)); return Task.CompletedTask; } }
var s=new Store(); var c=new Consumer(s); var m=new Message(Guid.Parse("11111111-1111-1111-1111-111111111111"),4201);
await c.Handle(m); await c.Handle(m);
Console.WriteLine($"deliveries=2 shipments={s.Items.Count}");
if(args.Contains("--reproduce")) return s.Items.Count==2?0:2;
if(args.Contains("--verify")) { await c.Handle(new(Guid.Parse("22222222-2222-2222-2222-222222222222"),4202)); return s.Items.Count(x=>x.OrderId==4201)==1 && s.Items.Count(x=>x.OrderId==4202)==1?0:3; }
return 0;