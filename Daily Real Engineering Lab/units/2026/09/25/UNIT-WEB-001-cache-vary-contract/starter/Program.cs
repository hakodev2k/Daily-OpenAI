var cache=new Dictionary<string,string>();
string Get(string path,string language){
 if(cache.TryGetValue(path,out var hit)){Console.WriteLine($"HIT {language} => {hit}");return hit;}
 var body=language=="vi"?"Sản phẩm":"Product";
 cache[path]=body;Console.WriteLine($"MISS {language} => {body}");return body;
}
var a=Get("/products/42","vi");
var b=Get("/products/42","en");
if(a!="Sản phẩm"||b!="Product") Environment.ExitCode=2;