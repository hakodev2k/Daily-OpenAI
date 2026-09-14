using Xunit;

namespace NotificationLab;

public sealed class NotificationCoordinatorTests
{
    [Fact]
    public void Deliver_preserves_notification_contract()
    {
        var probe = new CollaborationProbe();
        var sut = new NotificationCoordinator(probe, probe, probe);

        var result = sut.Deliver("user-42");

        Assert.True(result.Sent);
        Assert.Equal("user-42", result.UserId);
        Assert.Equal("Hello user-42", result.Message);
        Assert.Single(probe.SentMessages);
        Assert.Equal(("user-42", "Hello user-42"), probe.SentMessages[0]);
        Assert.Single(probe.AuditUsers);
        Assert.Equal("user-42", probe.AuditUsers[0]);

        // Investigation target: decide whether this sequence is part of the
        // observable business contract or only an internal implementation detail.
        Assert.Equal(new[] { "template", "send", "audit" }, probe.Calls);
    }
}
