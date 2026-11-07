# Before — baseline

`./reproduce.ps1` kỳ vọng nhận `LAB_RESULT=FAIL` với hai case `WAITS_FOR_SEND` và `PROPAGATES_FAILURE` thất bại; script in `REPRODUCED=YES` và trả exit code 0 nếu đúng triệu chứng. Case `SUCCESS_PATH` có thể vẫn PASS, cho thấy happy path không chứng minh tính đúng đắn. Nếu thiếu SDK, build lỗi hoặc timeout harness, phải coi là lỗi setup chứ không phải reproduce thành công.
