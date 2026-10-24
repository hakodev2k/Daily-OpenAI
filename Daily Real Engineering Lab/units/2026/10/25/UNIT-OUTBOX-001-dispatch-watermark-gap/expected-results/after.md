# Expected Results — Learner Fix

`verify.ps1` phải trả exit 0 với `ALL_VERIFICATIONS_PASSED`. Sau transient failure ở event 102, checkpoint không vượt qua event chưa acknowledge; lượt retry sau cùng downstream nhận 101, 102, 103 đúng thứ tự. Happy-path và batch boundary vẫn đúng. Khi checkpoint write thất bại sau publish, retry có thể phát lại cùng event ID; fake consumer chỉ áp dụng một lần.

`reproduce.ps1` sau khi sửa có thể trả exit 1 và in `INCIDENT_NOT_REPRODUCED` — đây là kỳ vọng vì script được thiết kế để xác nhận triệu chứng ở starter ban đầu, **không** phải script kiểm thử fix.
