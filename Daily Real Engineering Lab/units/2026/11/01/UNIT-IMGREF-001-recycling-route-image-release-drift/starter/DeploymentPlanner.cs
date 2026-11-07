namespace FleetDeployLab;

public sealed record DeploymentSpec(string ImageReference, string PullPolicy);

public static class DeploymentPlanner
{
    // This policy is used by the release pipeline for both rollout and rollback.
    public static DeploymentSpec Plan(string repository, string approvedDigest)
    {
        if (string.IsNullOrWhiteSpace(repository))
            throw new ArgumentException("Repository is required.", nameof(repository));

        return new DeploymentSpec($"{repository}:stable", "IfNotPresent");
    }
}
