using System.Text.RegularExpressions;

namespace FleetDeployLab;

public sealed record DeploymentSpec(string ImageReference, string PullPolicy);

public static class DeploymentPlanner
{
    public static DeploymentSpec Plan(string repository, string approvedDigest)
    {
        if (string.IsNullOrWhiteSpace(repository))
            throw new ArgumentException("Repository is required.", nameof(repository));
        if (repository.Contains('@') || repository.EndsWith(':'))
            throw new ArgumentException("Repository must not contain a digest or trailing colon.", nameof(repository));
        if (string.IsNullOrWhiteSpace(approvedDigest)
            || !Regex.IsMatch(approvedDigest, @"\Asha256:[0-9a-f]{64}\z", RegexOptions.CultureInvariant))
            throw new ArgumentException("Expected a lowercase sha256 digest with 64 hex characters.", nameof(approvedDigest));

        return new DeploymentSpec($"{repository}@{approvedDigest}", "IfNotPresent");
    }
}
