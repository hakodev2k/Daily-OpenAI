# Before
Chạy ./scripts/reproduce.ps1 với starter nguyên bản.

- Ticks 2–5: optional risk engine down, 4 requests bị unrouted.
- Có 2 restart trong 4 ticks outage.
- SUMMARY restarts=2 dropped=4 fallback=0 personalized=4.
- Script exit 0 và in REPRODUCED.
- ./scripts/verify.ps1 FAIL với các yêu cầu availability của optional dependency.

Nếu không tái hiện, kiểm tra starter/ProbePolicy.cs chưa bị sửa.
