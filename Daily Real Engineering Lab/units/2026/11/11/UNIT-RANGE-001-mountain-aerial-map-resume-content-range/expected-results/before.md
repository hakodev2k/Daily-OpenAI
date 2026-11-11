# Before
`./scripts/reproduce.ps1` phải exit 0 và in `REPRODUCED`, `assembled=ABCDDEFGHIJKL` (13 byte) khác canonical 12 byte. `./scripts/verify.ps1` phải fail với `Mismatched range was accepted`.
