namespace ApprovalAuditLab;

// Investigation: compare the in-memory event with its persisted representation.
public abstract class ApprovalEvent
{
    public string EventId { get; init; } = "";
    public DateTimeOffset OccurredAt { get; init; }
}

public sealed class ApprovalGranted : ApprovalEvent
{
    public string ApprovedBy { get; init; } = "";
    public string PolicyVersion { get; init; } = "";
}

public sealed class ApprovalRejected : ApprovalEvent
{
    public string ReasonCode { get; init; } = "";
}
