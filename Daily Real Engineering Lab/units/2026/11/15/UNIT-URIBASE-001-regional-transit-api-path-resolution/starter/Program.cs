using System;
using System.Collections.Generic;
using System.Linq;
record Endpoint(string Name, string Relative, string ExpectedPath);
static class TransitClient {
 public static Uri Resolve(Uri baseAddress, string relative) => new(baseAddress, relative);
}
class Program {
 static int Main() {
  var baseAddress = new Uri("https://partner.example.test/api/v2/");
  var endpoints = new[] {
   new Endpoint("Stations", "stations", "/api/v2/stations"),
   new Endpoint("Fare quote", "/fares/quote", "/api/v2/fares/quote"),
   new Endpoint("Service alert", "../alerts", "/api/v2/alerts"),
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
