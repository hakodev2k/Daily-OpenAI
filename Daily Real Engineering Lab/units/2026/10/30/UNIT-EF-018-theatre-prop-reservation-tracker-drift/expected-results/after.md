# After — Learner-edited starter
- `./verify.ps1` trả exit code 0, in ba dòng PASS.
- (10,2,1) → persisted=7, accepted.
- (12,3,4) → persisted=5, accepted.
- (3,2,2) → persisted=1, rejected.
- Không chỉnh sửa fixtures/expected values để làm xanh test.
- Đây chỉ là contract tuần tự; production concurrency cần thêm bảo vệ liên request.
