using System.Threading.Channels;
var b=WebApplication.CreateBuilder(args); b.Services.AddSingleton<JobQueue>(); b.Services.AddHostedService<Worker>(); var app=b.Build();
app.MapPost("/receipt/{id}",(string id,HttpContext h,ILogger<Program> log,JobQueue q)=>{var op=h.Request.Headers["X-Operation-Id"].FirstOrDefault()??Guid.NewGuid().ToString("N"); using(log.BeginScope(new Dictionary<string,object>{{"OperationId",op}})){log.LogInformation("Accepted {Id}",id);q.Add(new Job(id));} return Results.Accepted();}); app.Run();
record Job(string Id);
sealed class JobQueue{readonly Channel<Job> c=Channel.CreateUnbounded<Job>();public void Add(Job j)=>c.Writer.TryWrite(j);public IAsyncEnumerable<Job> Read(CancellationToken ct)=>c.Reader.ReadAllAsync(ct);}
sealed class Worker(JobQueue q,ILogger<Worker> log):BackgroundService{protected override async Task ExecuteAsync(CancellationToken ct){await foreach(var j in q.Read(ct)){await Task.Delay(100,ct);log.LogInformation("Processed {Id}",j.Id);}}}
