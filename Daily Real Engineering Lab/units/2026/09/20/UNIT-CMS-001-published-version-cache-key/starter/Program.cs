var mode=args.FirstOrDefault()??"run";
var source=new ContentSource(new Content(42,1,"Summer sale"));
var delivery=new DeliveryCache();
var first=delivery.Get(source.Current);
Console.WriteLine($"first version={first.Version} body={first.Body}");
source.Publish(new Content(42,2,"Autumn sale"));
var second=delivery.Get(source.Current);
Console.WriteLine($"source version={source.Current.Version}; second version={second.Version} body={second.Body}");
var stale=second.Version!=source.Current.Version;
if(mode=="reproduce"){Console.WriteLine(stale?"SYMPTOM_REPRODUCED":"SYMPTOM_MISSING");return stale?0:2;}
if(mode=="verify"){Console.WriteLine(!stale?"VERIFY_PASS":"VERIFY_FAIL");return !stale?0:3;}
return 0;
record Content(int Id,int Version,string Body);
sealed class ContentSource(Content current){public Content Current{get;private set;}=current;public void Publish(Content next)=>Current=next;}
sealed class DeliveryCache{
 readonly Dictionary<string,Content> cache=[];
 public Content Get(Content published){var key=$"content:{published.Id}";if(cache.TryGetValue(key,out var hit))return hit;cache[key]=published;return published;}
}