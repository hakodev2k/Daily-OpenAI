# Before — expected observable behavior
- `reproduce.ps1` exits 0 and prints `REPRODUCED`.
- `production.ROUTE_DB_ENDPOINT` changes from `sql-prod.internal` to `sql-stage.internal`.
- `staging.ROUTE_DB_ENDPOINT` changes from `sql-stage.internal` to `sql-prod.internal`.
- `RELEASE_LABEL` and `FEATURE_NEW_ROUTER` move to the opposite slot.
- `SLOT_ENVIRONMENT` stays with the original slot.
- Unmodified `starter/fixtures/slots.json` fails `verify.ps1` with nonzero exit code.
