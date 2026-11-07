# UNIT-API-001 — Offset pagination drift under concurrent inserts

Reproduce pagination drift with offset paging while new items are inserted, then replace it with stable cursor/keyset pagination. Run `reproduce.ps1`, fix `starter/Program.cs`, then run `verify.ps1`.
