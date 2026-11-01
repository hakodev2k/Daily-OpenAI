# Before
- ./reproduce.ps1: exit 0, Registrations=8, ParsedRows=8, RiskyCells=6.
- ./verify.ps1: exit 1; spreadsheet export contract fails.
- CSV parser preserves commas, quotes and embedded newline.
- No spreadsheet formulas are executed.
