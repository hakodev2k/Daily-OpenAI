using System.Collections.Generic;

namespace FleetDeployLab;

internal static class Program
{
    private const string Repository = "registry.example.local/recycling-route-engine";
    private const string DigestA = "sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    private const string DigestB = "sha256:bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    private static int Main(string[] args)
    {
        try
        {
            return (args.Length == 0 ? "inspect" : args[0].ToLowerInvariant()) switch
            {
                "inspect" => Inspect(),
                "reproduce" => Reproduce(),
                "verify" => Verify(),
                _ => throw new ArgumentException("Usage: inspect | reproduce | verify")
            };
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 2;
        }
    }

    private static int Inspect()
    {
        var observation = Deploy(DigestB, DigestA);
        Print("Release candidate", observation);
        return 0;
    }

    private static int Reproduce()
    {
        var observation = Deploy(DigestB, DigestA);
        Print("Original symptom", observation);
        bool reproduced = observation.East != observation.West;
        Console.WriteLine(reproduced
            ? "REPRODUCED: Ready workers disagree on running revision."
            : "NOT REPRODUCED: both workers run the same revision.");
        return reproduced ? 0 : 1;
    }

    private static int Verify()
    {
        var failures = new List<string>();
        var release = Deploy(DigestB, DigestA);
        Print("New release", release);
        if (release.East != DigestB || release.West != DigestB)
            failures.Add("New release must run the approved digest on both nodes.");
        if (release.Spec.ImageReference != $"{Repository}@{DigestB}")
            failures.Add("Release specification must identify the approved immutable artifact.");

        var rollback = Deploy(DigestA, DigestB);
        Print("Rollback", rollback);
        if (rollback.East != DigestA || rollback.West != DigestA)
            failures.Add("Rollback must restore the previously approved digest on both nodes.");
        if (rollback.Spec.ImageReference != $"{Repository}@{DigestA}")
            failures.Add("Rollback specification must identify the previous immutable artifact.");
        if (release.Spec.ImageReference == rollback.Spec.ImageReference)
            failures.Add("Distinct approved releases must produce distinct rollout specifications.");

        bool rejectedInvalidDigest = false;
        try
        {
            _ = DeploymentPlanner.Plan(Repository, "sha256:invalid");
        }
        catch (ArgumentException)
        {
            rejectedInvalidDigest = true;
        }

        if (!rejectedInvalidDigest)
            failures.Add("Invalid digest must be rejected rather than silently replaced.");

        if (failures.Count > 0)
        {
            foreach (var failure in failures)
                Console.Error.WriteLine("FAIL: " + failure);
            return 1;
        }

        Console.WriteLine("PASS: release, rollback, provenance and invalid-input contracts.");
        return 0;
    }

    private static Observation Deploy(string approvedDigest, string preloadedTagDigest)
    {
        var registry = new FakeRegistry();
        registry.RegisterDigest(DigestA);
        registry.RegisterDigest(DigestB);
        registry.SetTag($"{Repository}:stable", DigestB);

        var east = new FakeNode("node-east");
        var west = new FakeNode("node-west");
        east.Preload($"{Repository}:stable", preloadedTagDigest);

        var spec = DeploymentPlanner.Plan(Repository, approvedDigest);
        return new Observation(spec, east.Start(spec, registry), west.Start(spec, registry));
    }

    private static void Print(string label, Observation observation)
    {
        Console.WriteLine($"{label}: ref={observation.Spec.ImageReference}, pull={observation.Spec.PullPolicy}");
        Console.WriteLine($"  node-east Ready, running={observation.East[..19]}...");
        Console.WriteLine($"  node-west Ready, running={observation.West[..19]}...");
    }

    private sealed record Observation(DeploymentSpec Spec, string East, string West);

    private sealed class FakeRegistry
    {
        private readonly Dictionary<string, string> _tags = new(StringComparer.Ordinal);
        private readonly HashSet<string> _digests = new(StringComparer.Ordinal);

        public void RegisterDigest(string digest) => _digests.Add(digest);

        public void SetTag(string tag, string digest)
        {
            if (!_digests.Contains(digest))
                throw new InvalidOperationException("Unknown artifact digest.");
            _tags[tag] = digest;
        }

        public bool HasDigest(string digest) => _digests.Contains(digest);

        public string ResolveTag(string tag) => _tags.TryGetValue(tag, out var digest)
            ? digest
            : throw new InvalidOperationException("Unknown image tag.");
    }

    private sealed class FakeNode
    {
        private readonly Dictionary<string, string> _localImages = new(StringComparer.Ordinal);

        public FakeNode(string name) => Name = name;

        public string Name { get; }

        public void Preload(string reference, string digest) => _localImages[reference] = digest;

        public string Start(DeploymentSpec spec, FakeRegistry registry)
        {
            if (spec.PullPolicy is not ("Always" or "IfNotPresent"))
                throw new InvalidOperationException("Unsupported image pull policy.");

            int at = spec.ImageReference.IndexOf("@sha256:", StringComparison.Ordinal);
            if (at >= 0)
            {
                string digest = spec.ImageReference[(at + 1)..];
                if (!registry.HasDigest(digest))
                    throw new InvalidOperationException("Unknown pinned image digest.");
                return digest;
            }

            if (spec.PullPolicy == "IfNotPresent"
                && _localImages.TryGetValue(spec.ImageReference, out var cached))
                return cached;

            string resolved = registry.ResolveTag(spec.ImageReference);
            _localImages[spec.ImageReference] = resolved;
            return resolved;
        }
    }
}
