# After
- `./reproduce.ps1` vẫn in `REPRODUCED` vì baseline được cố định để giữ chứng cứ.
- `./verify.ps1` trả exit code 0, in `PASS` cho 9 trường hợp và dòng `PASS: all learner-code contract cases.`.
- Frame hợp lệ phân mảnh được giải mã nguyên vẹn; EOF sớm vẫn bị từ chối; độ dài ngoài giới hạn không cấp phát payload; cancellation được tôn trọng.
- Không cần đo thời gian hoặc kết nối mạng thật.
