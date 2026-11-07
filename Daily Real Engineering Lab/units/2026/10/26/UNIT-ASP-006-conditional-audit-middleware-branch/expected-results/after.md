# After — learner fixes starter

- `/health`: 200 `healthy`, không audit.
- `/api/v1/maintenance/active`: 200 `active:3`, audit=`audit-v1`.
- `/api/v1/maintenance/history`: 200 `history:2`, audit=`audit-v1`.
- `/missing`: 404, không audit.
- `verify.ps1`: exit 0 chỉ khi **starter** thỏa mọi assertion.

Fix làm API pass nhưng mất audit hoặc đổi missing thành 200 đều bị từ chối.
