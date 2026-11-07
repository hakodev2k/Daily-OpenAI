using System.Text.Json.Serialization;

namespace ApprovalAuditLab;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$eventType")]
[JsonDerivedType(typeof(ApprovalGranted), "approval-granted")]
[JsonDerivedType(typeof(ApprovalRejected), "approval-rejected")]
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
