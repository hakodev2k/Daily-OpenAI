# Verification contract

| Case | Setup | Expected |
|---|---|---|
| V1 | Add LOT-42 at A-01 | Contains true |
| V2 | Move LOT-42 to COLD-07 | Contains true, Count 1 |
| V3 | Remove LOT-42 after move | Remove true, Count 0 |
| V4 | Compare LOT-42 and LOT-43 on same shelf | Not equal |
| V5 | Run verify command on starter | Exit code 0 after learner fix; nonzero before fix |

Run `dotnet run --project starter/SeedBank.csproj -- verify` after editing. This command tests the actual starter source, not the reference implementation. Record terminal output and explain how identity differs from mutable attributes.
