# Before — baseline
- `reproduce.ps1` exit 0 khi chứng minh `maxActive >= 2`; đó là PASS của **reproduction**, không phải PASS của ứng dụng.
- `BATCH_START` xuất hiện nhiều lần trước khi batch đầu kết thúc.
- `verify.ps1` chạy starter nguyên bản sẽ exit khác 0 vì overlap.
- Thời gian thực tế và số batch có thể khác nhau; không so sánh chính xác timestamp.
