# After — acceptance criteria
- `verify.ps1` exits 0 and prints `VERIFIED`.
- Production always keeps `sql-prod.internal`; staging keeps `sql-stage.internal`.
- Release label and feature flag move to the other slot.
- Slot environment labels stay in place.
- Reversing the swap restores all original app-setting values.
- Baseline reproduction remains independent of the learner-edited fixture.
