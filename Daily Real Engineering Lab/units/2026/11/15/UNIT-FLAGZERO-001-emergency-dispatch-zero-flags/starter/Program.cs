using System;
[Flags] enum Role { None=0, Medical=1, Fire=2, Logistics=4 }
static class Policy {
 public static bool CanView(Role assigned, Role required) {
  return assigned.HasFlag(required);
 }
}
class Program {
 static int Main() {
  var cases = new (string Name,Role Assigned,Role Required,bool Expected)[] {
   ("Unassigned",Role.None,Role.None,false),
   ("Medical",Role.Medical,Role.Medical,true),
   ("Fire denied",Role.Medical,Role.Fire,false),
   ("Combined",Role.Medical|Role.Fire,Role.Medical|Role.Fire,true)
  };
  int mismatches=0;
  foreach(var c in cases) {
   bool actual=Policy.CanView(c.Assigned,c.Required);
   Console.WriteLine($"{c.Name}: ACTUAL={actual} EXPECTED={c.Expected}");
   if(actual!=c.Expected) mismatches++;
  }
  Console.WriteLine($"MISMATCHES={mismatches}");
  return mismatches==0?0:1;
 }
}
