# Before — starter gốc
`./scripts/reproduce.ps1` exit 0 khi xuất hiện:

| Case | Expected | Actual |
|---|---:|---:|
| steady-session | 2 | 2 |
| repeat-connect | 1 | 1 |
| reconnect | 1 | 2 |
| offline-session | 0 | 1 |
| disposed-session | 0 | 1 |

`behavioral-mismatches=3`. `./scripts/verify.ps1` phải exit khác 0 trên starter chưa sửa. Nếu không giống, kiểm tra đúng project và chưa sửa starter.
