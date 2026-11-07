# After — learner fix

`./verify.ps1` phải in `CASE=WAITS_FOR_SEND RESULT=PASS`, `CASE=PROPAGATES_FAILURE RESULT=PASS`, `CASE=SUCCESS_PATH RESULT=PASS`, `LAB_RESULT=PASS` và `VERIFIED=YES`. Job không thể hoàn thành trước gateway; lỗi đối tác phải đi tới caller; thành công chỉ được công nhận khi side effect hoàn tất. Harness không được chỉnh sửa để đạt kết quả.
