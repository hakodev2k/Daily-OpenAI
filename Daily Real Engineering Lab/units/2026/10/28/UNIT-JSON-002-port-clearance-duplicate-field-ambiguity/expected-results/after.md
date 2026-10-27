# After — Learner fix

`./verify.ps1` phải kết thúc với `CHECKS_PASSED=8` và `VERIFICATION_CONFIRMED`. Tất cả 4 valid paths được chấp nhận và 4 invalid paths bị từ chối bằng `JsonException`. Payload khác casing nhưng không xung đột vẫn hoạt động. `./reproduce.ps1` không còn xác nhận symptom trên code đã sửa; đây là kết quả mong đợi.
