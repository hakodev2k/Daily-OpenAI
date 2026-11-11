public record Patient(string Id, string Priority);
public record ShiftBoard(string Shift, List<Patient> Patients);

public static class Handover
{
    public static ShiftBoard Save(ShiftBoard live) => live with { Shift = live.Shift };
}

var live = new ShiftBoard("night", new List<Patient> { new("P-100", "urgent"), new("P-101", "routine") });
var saved = Handover.Save(live);
Console.WriteLine("before=" + string.Join(",", saved.Patients.Select(x => x.Priority)));
live.Patients[0] = live.Patients[0] with { Priority = "routine" };
Console.WriteLine("after=" + string.Join(",", saved.Patients.Select(x => x.Priority)));
Console.WriteLine("live=" + string.Join(",", live.Patients.Select(x => x.Priority)));
