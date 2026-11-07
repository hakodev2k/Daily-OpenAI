# After
Chạy ./scripts/verify.ps1 trên starter đã chỉnh sửa.

- VERIFY 14/14 passed.
- Optional engine down: live=true, ready=true, ServeSession trả 200-fallback.
- Primary store down: live=true, ready=false, ServeSession trả 503.
- Process down: live=false, ready=false.
- Demo: SUMMARY restarts=0 dropped=0 fallback=4 personalized=4.
- reproduce.ps1 có thể exit 1 sau khi sửa, vì chỉ dùng để chứng minh lỗi gốc.
