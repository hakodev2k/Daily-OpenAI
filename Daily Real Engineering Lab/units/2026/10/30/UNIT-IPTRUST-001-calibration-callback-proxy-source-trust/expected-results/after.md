# After
- `./verify.ps1` exit 0, `mismatches=0`.
- `valid-proxy-partner` và `valid-proxy-whitespace` được ALLOW.
- Các direct connection, multi-hop, missing/malformed header và partner không được phép đều DENY.
- `./reproduce.ps1` exit 1 vì không còn tái hiện được fixture tấn công.
- Không thay đổi signature `IsAuthorized(IPAddress, string?)`.
