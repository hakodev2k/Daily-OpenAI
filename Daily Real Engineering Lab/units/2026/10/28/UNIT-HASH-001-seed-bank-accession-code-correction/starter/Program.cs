using SeedRegistryLab;

var mode = args.Length == 0 ? "--demo" : args[0];
try
{
    switch (mode)
    {
        case "--demo":
        {
            var registry = new AccessionRegistry();
            registry.Register("SEED-041");
            registry.CorrectCode("SEED-041", "SEED-042");
            Console.WriteLine("LOOKUP_AFTER_CORRECTION=" + registry.ContainsCode("SEED-042"));
            Console.WriteLine("COUNT=" + registry.Count);
            return 0;
        }
        case "--reproduce":
        {
            var baseline = new BaselineRegistry();
            if (!baseline.Register("SEED-041") ||
                !baseline.CorrectCode("SEED-041", "SEED-042"))
                throw new InvalidOperationException("Baseline fixture setup failed.");
            var found = baseline.ContainsCode("SEED-042");
            var duplicateAccepted = baseline.Register("SEED-042");
            var removed = baseline.RemoveCode("SEED-042");
            Console.WriteLine("FOUND_CORRECTED=" + found);
            Console.WriteLine("DUPLICATE_ACCEPTED=" + duplicateAccepted);
            Console.WriteLine("REMOVED_NEW_CODE=" + removed);
            Console.WriteLine("REMAINING_COUNT=" + baseline.Count);
            if (!found && duplicateAccepted && removed && baseline.Count == 1)
            {
                Console.WriteLine("REPRODUCED: corrected record remains unreachable");
                return 0;
            }
            throw new InvalidOperationException("Original symptom did not match deterministic fixture.");
        }
        case "--verify":
        {
            var passed = 0;
            void Expect(bool condition, string label)
            {
                if (!condition) throw new InvalidOperationException(label);
                passed++;
                Console.WriteLine("PASS " + label);
            }

            var registry = new AccessionRegistry();
            Expect(registry.Register("SEED-041") && registry.ContainsCode("SEED-041"), "initial-registration");
            Expect(registry.CorrectCode("SEED-041", "SEED-042") &&
                   !registry.ContainsCode("SEED-041") &&
                   registry.ContainsCode("SEED-042") && registry.Count == 1,
                   "correction-index-consistency");
            Expect(!registry.Register("SEED-042") && registry.Count == 1,
                   "duplicate-rejected-after-correction");
            Expect(registry.RemoveCode("SEED-042") && registry.Count == 0 &&
                   !registry.ContainsCode("SEED-042"), "remove-corrected-record");

            var conflict = new AccessionRegistry();
            conflict.Register("SEED-041");
            conflict.Register("SEED-099");
            Expect(!conflict.CorrectCode("SEED-041", "SEED-099") &&
                   conflict.ContainsCode("SEED-041") &&
                   conflict.ContainsCode("SEED-099") && conflict.Count == 2,
                   "conflict-preserves-both-records");
            Expect(!conflict.CorrectCode("MISSING", "SEED-050") &&
                   conflict.CorrectCode("SEED-041", "SEED-041") &&
                   conflict.Count == 2, "missing-and-noop-contract");

            var reuse = new AccessionRegistry();
            reuse.Register("SEED-041");
            reuse.CorrectCode("SEED-041", "SEED-042");
            Expect(reuse.Register("SEED-041") && reuse.Count == 2 &&
                   reuse.ContainsCode("SEED-041") && reuse.ContainsCode("SEED-042"),
                   "old-code-can-be-reused");
            Console.WriteLine("CHECKS_PASSED=" + passed);
            Console.WriteLine("VERIFIED: learner registry satisfies all contracts");
            return 0;
        }
        default:
            Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
            return 2;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("CHECK_FAILED: " + ex.Message);
    return 1;
}
