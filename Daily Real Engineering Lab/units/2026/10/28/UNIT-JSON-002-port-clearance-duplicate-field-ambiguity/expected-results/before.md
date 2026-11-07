# Before — Starter

`./reproduce.ps1` phải kết thúc với `REPRODUCTION_CONFIRMED`, `BASELINE_REQUIRES_INSPECTION=True`, `AMBIGUOUS_ACCEPTED=True`, `INSPECTION_DOWNGRADED=True`. `./verify.ps1` phải **fail** ở negative contract check; failure này là chủ đích của starter. Nếu build fail, không tính là reproduction.
