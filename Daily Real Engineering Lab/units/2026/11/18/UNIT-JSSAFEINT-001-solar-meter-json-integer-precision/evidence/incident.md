# Incident evidence (synthetic, deterministic)
- 09:10: server reconciliation receives two meter IDs: 9007199254740992 and 9007199254740993.
- 09:12: API JSON contains two distinct numeric tokens.
- 09:13: browser client maps both IDs to the same JavaScript Number; selection loses one meter.
- 09:15: kWh readings remain 12.5 and 13.5; no evidence of database duplication.
- Investigation task: distinguish server storage, wire representation, parsing and UI selection.

