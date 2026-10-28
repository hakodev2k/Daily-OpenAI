using TicketHoldLab;

var mode = args.Length == 0 ? "--demo" : args[0];
var budget = TimeSpan.FromMilliseconds(750);

try
{
    switch (mode)
    {
        case "--demo":
        {
            foreach (var milliseconds in new[] { 620, 749, 750, 751, 900, 1200, 1900, 2500 })
            {
                bool flagged = LatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(milliseconds), budget);
                Console.WriteLine($"SAMPLE durationMs={milliseconds} budgetMs=750 flagged={flagged}");
            }
            return 0;
        }
        case "--reproduce":
        {
            bool slow1200 = BaselineLatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(1200), budget);
            bool slow1900 = BaselineLatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(1900), budget);
            bool slow2500 = BaselineLatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(2500), budget);
            bool fast620 = BaselineLatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(620), budget);
            Console.WriteLine($"SLOW_1200_FLAGGED={slow1200}");
            Console.WriteLine($"SLOW_1900_FLAGGED={slow1900}");
            Console.WriteLine($"SLOW_2500_FLAGGED={slow2500}");
            Console.WriteLine($"FAST_620_FLAGGED={fast620}");
            if (!slow1200 && slow1900 && !slow2500 && !fast620)
            {
                Console.WriteLine("REPRODUCED: latency breach classification oscillates");
                return 0;
            }
            throw new InvalidOperationException("Baseline fixture did not reproduce the intended symptom.");
        }
        case "--verify":
        {
            int passed = 0;
            void Expect(bool condition, string label)
            {
                if (!condition) throw new InvalidOperationException("FAILED: " + label);
                passed++;
                Console.WriteLine("PASS " + label);
            }
            bool ThrowsArgumentOutOfRange(Action action)
            {
                try { action(); return false; }
                catch (ArgumentOutOfRangeException) { return true; }
            }
            bool Check(long elapsedMs, long budgetMs) =>
                LatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(elapsedMs),
                                           TimeSpan.FromMilliseconds(budgetMs));

            Expect(!Check(0, 750), "zero-duration");
            Expect(!Check(749, 750), "under-boundary");
            Expect(!Check(750, 750), "equal-boundary-is-not-breach");
            Expect(Check(751, 750), "first-millisecond-over");
            Expect(Check(999, 750), "within-first-second");
            Expect(Check(1000, 750), "exactly-one-second");
            Expect(Check(1200, 750), "twelve-hundred-ms");
            Expect(Check(1900, 750), "nineteen-hundred-ms");
            Expect(Check(2500, 750), "multiple-seconds");
            Expect(!Check(1200, 1500), "larger-budget-under");
            Expect(!Check(1500, 1500), "larger-budget-equal");
            Expect(Check(1501, 1500), "larger-budget-over");
            Expect(Check(60000, 750), "one-minute-duration");
            Expect(LatencyPolicy.IsOverBudget(TimeSpan.FromTicks(budget.Ticks + 1), budget),
                   "submillisecond-boundary");
            Expect(ThrowsArgumentOutOfRange(() =>
                { _ = LatencyPolicy.IsOverBudget(TimeSpan.FromMilliseconds(-1), budget); }),
                   "negative-elapsed-rejected");
            Expect(ThrowsArgumentOutOfRange(() =>
                { _ = LatencyPolicy.IsOverBudget(TimeSpan.Zero, TimeSpan.Zero); }),
                   "zero-budget-rejected");
            Expect(ThrowsArgumentOutOfRange(() =>
                { _ = LatencyPolicy.IsOverBudget(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1)); }),
                   "negative-budget-rejected");
            Console.WriteLine("CHECKS_PASSED=" + passed);
            Console.WriteLine("VERIFIED: learner latency policy satisfies SLA contracts");
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
