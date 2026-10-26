using GradesLab;

var mode = args.SingleOrDefault() ?? "--reproduce";
if (mode is not ("--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --reproduce | --verify");
    return 2;
}

var actual = await GradeWorkflow.RunAsync();
Console.WriteLine($"Persisted: Id={actual.Id}, Score={actual.Score}, Status={actual.Status}, Note={actual.ReviewerNote}");

bool common = actual.Id == 41 && actual.Score == 92 && actual.ReviewerNote == "QA-reviewed";
bool success = mode == "--reproduce"
    ? common && actual.Status == "Draft"
    : common && actual.Status == "Published";

Console.WriteLine(success ? "CONTRACT PASS" : "CONTRACT FAIL");
return success ? 0 : 1;
