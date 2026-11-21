# Before
- `./reproduce.ps1` exit 0 khi phát hiện lỗi starter.
- Console in `received=1,2,3 canceled=False` và `CANCEL_IGNORED`.
- `./verify.ps1` exit 1, chứng tỏ yêu cầu cancellation chưa được đáp ứng.
- `./run.ps1` exit 0 với `normal=1,2,3 result=PASS`.
Không dùng thời gian thực thi làm tiêu chí pass/fail.
