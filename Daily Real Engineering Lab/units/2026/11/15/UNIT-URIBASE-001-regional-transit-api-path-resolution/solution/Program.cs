using System;
using System.Collections.Generic;
using System.Linq;
record Endpoint(string Name, string Relative, string ExpectedPath);
static class TransitClient {
 public static Uri Resolve(Uri baseAddress, string relative) {
  if (!baseAddress.IsAbsoluteUri || baseAddress.Scheme != Uri.UriSchemeHttps)
   throw new ArgumentException("HTTPS base URI required", nameof(baseAddress));
  if (Uri.TryCreate(relative, UriKind.Absolute, out _))
   throw new ArgumentException("Absolute endpoint URI forbidden", nameof(relative));
  var prefix = baseAddress.AbsolutePath.TrimEnd('/') + "/";
  var segments = relative.Split('/', StringSplitOptions.RemoveEmptyEntries);
  if (segments.Any(x => x == ".." || x == "." || x.Contains('%') || x.Contains('?') || x.Contains('#')))
   throw new ArgumentException("Endpoint traversal or encoded delimiters forbidden", nameof(relative));
  var path = string.Join("/", segments);
  var result = new Uri(baseAddress, prefix + path);
  if (result.Host != baseAddress.Host || !result.AbsolutePath.StartsWith(prefix, StringComparison.Ordinal))
   throw new ArgumentException("Endpoint escaped API prefix", nameof(relative));
  return result;
 }
}
class Program {
 static int Main() {
  var baseAddress = new Uri("https://partner.example.test/api/v2/");
  var endpoints = new[] {
   new Endpoint("Stations", "stations", "/api/v2/stations"),
   new Endpoint("Fare quote", "/fares/quote", "/api/v2/fares/quote"),
   new Endpoint("Service alert", "alerts", "/api/v2/alerts"),
   new Endpoint("Journey", "journeys/plan", "/api/v2/journeys/plan")
  };
  var mismatches=0;
  foreach(var e in endpoints) {
   var uri=TransitClient.Resolve(baseAddress,e.Relative);
   bool ok=uri.AbsolutePath==e.ExpectedPath && uri.Host==baseAddress.Host;
   Console.WriteLine($"{e.Name}: ACTUAL={uri.AbsolutePath} EXPECTED={e.ExpectedPath} MATCH={ok}");
   if(!ok)mismatches++;
  }
  Console.WriteLine($"MISMATCHES={mismatches}");
  return mismatches==0?0:1;
 }
}
