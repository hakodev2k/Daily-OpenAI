# Expected results

## Before — original starter
- run.ps1: UNIT=s; SAMPLES=120.000,180.000,240.000,300.000,600.000; P95_SECONDS=600.000; BREACH=true.
- reproduce.ps1: exit 0, confirms original symptom.
- verify.ps1: fails measurement contract.
- Independent traces 120–600 ms all below 750 ms.

## After — learner fix
- run.ps1: UNIT=s; SAMPLES=0.120,0.180,0.240,0.300,0.600; P95_SECONDS=0.600; BREACH=false.
- verify.ps1: pass on learner-editable starter, not reference solution.
- reproduce.ps1 no longer confirms old symptom (expected).

If reproduction fails unexpectedly, check SDK, working directory and whether starter was already modified.
