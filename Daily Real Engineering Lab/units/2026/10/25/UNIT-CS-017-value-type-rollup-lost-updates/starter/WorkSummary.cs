namespace FieldOpsLab;

internal readonly record struct WorkEvent(string TechnicianId, bool Succeeded);

internal struct WorkSummary
{
    public int Completed { get; private set; }
    public int Failed { get; private set; }
    public int Total => Completed + Failed;

    public void Record(bool succeeded)
    {
        if (succeeded) Completed++;
        else Failed++;
    }
}
