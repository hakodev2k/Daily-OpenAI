# Sau khi sửa

`./reproduce.ps1` không còn xác nhận lỗi trên code đã sửa (đó là bình thường: reproduce chỉ dùng để chứng minh lỗi của starter gốc).

`./verify.ps1` phải trả exit code 0, in ra `PASS` cho sáu nhóm input và dòng `VERIFIED: 7 behavior contracts passed on learner-editable starter.`

Đầu ra batch mẫu sau sửa phải là `FB-103, FB-104`. Không có lô quá hạn hoặc cách ly, thứ tự tương đối của lô hợp lệ giữ nguyên, và gọi lại không làm thay đổi tập hợp hợp lệ. Không dùng thời gian chạy tuyệt đối làm tiêu chí pass/fail.
