using System;using System.Linq;
public static class ShiftClock{
static TimeZoneInfo Zone(){var a=TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1,1,1,2,0,0),3,2,DayOfWeek.Sunday);var b=TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1,1,1,2,0,0),11,1,DayOfWeek.Sunday);return TimeZoneInfo.CreateCustomTimeZone("LabEastern",TimeSpan.FromHours(-5),"Lab Eastern","Standard","Daylight",[TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2020,1,1),new DateTime(2035,12,31),TimeSpan.FromHours(1),a,b)]);}
public static DateTimeOffset Book(DateTime local,TimeSpan? selectedOffset=null){
var z=Zone();var wall=DateTime.SpecifyKind(local,DateTimeKind.Unspecified);
var offset=selectedOffset??z.GetUtcOffset(wall);
return new DateTimeOffset(wall,offset).ToUniversalTime();
}
public static void Main(string[] args){
var z=Zone();foreach(var w in new[]{new DateTime(2026,11,1,0,30,0),new DateTime(2026,11,1,1,30,0),new DateTime(2026,3,8,2,30,0)})
Console.WriteLine($"Local={w:O} Invalid={z.IsInvalidTime(w)} Ambiguous={z.IsAmbiguousTime(w)} UTC={Book(w):O}");
if(args.Contains("--check")){int fail=0;void Check(bool ok,string n){Console.WriteLine($"{(ok?"PASS":"FAIL")} {n}");if(!ok)fail++;}
Check(Book(new DateTime(2026,11,1,0,30,0))==new DateTimeOffset(2026,11,1,4,30,0,TimeSpan.Zero),"normal");
var fold=new DateTime(2026,11,1,1,30,0);bool rejected=false;try{Book(fold);}catch(ArgumentException){rejected=true;}Check(rejected,"fold-requires-offset");
Check(Book(fold,TimeSpan.FromHours(-4))==new DateTimeOffset(2026,11,1,5,30,0,TimeSpan.Zero),"fold-daylight");
Check(Book(fold,TimeSpan.FromHours(-5))==new DateTimeOffset(2026,11,1,6,30,0,TimeSpan.Zero),"fold-standard");
rejected=false;try{Book(new DateTime(2026,3,8,2,30,0));}catch(ArgumentException){rejected=true;}Check(rejected,"gap-rejected");
rejected=false;try{Book(fold,TimeSpan.FromHours(-6));}catch(ArgumentException){rejected=true;}Check(rejected,"offset-membership");
if(fail>0)Environment.ExitCode=1;
}}
}