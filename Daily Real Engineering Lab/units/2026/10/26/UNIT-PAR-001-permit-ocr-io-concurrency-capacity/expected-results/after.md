# After
Trên starter đã sửa:
- N=8: effectiveConcurrency=8, maxActive từ 6 đến 8.
- N=3: effectiveConcurrency=3, maxActive=3.
- N=16: effectiveConcurrency=8, maxActive không vượt 8.
- Mọi lần: processedCount=24, missingCount=0, duplicateCount=0, rejectedCount=0.
- verify.ps1 exit 0. Thời gian N=8 giảm rõ so với baseline nhưng không dùng ngưỡng timing cứng vì máy khác nhau.
