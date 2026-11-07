record Message(Guid Id,int OrderId); record Shipment(int OrderId);
sealed class Store { public List<Shipment> Items {get;}=[]; public HashSet<Guid> Done {get;}=[]; }
sealed class Consumer(Store s) { public Task Handle(Message m){ if(!s.Done.Add(m.Id)) return Task.CompletedTask; s.Items.Add(new(m.OrderId)); return Task.CompletedTask; } }
var s=new Store();var c=new Consumer(s);var m=new Message(Guid.Parse("11111111-1111-1111-1111-111111111111"),4201);await c.Handle(m);await c.Handle(m);Console.WriteLine(s.Items.Count);