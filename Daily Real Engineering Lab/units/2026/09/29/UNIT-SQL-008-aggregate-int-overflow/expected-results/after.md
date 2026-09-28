# Expected Results — After

Một fix hợp lệ phải:

- chạy thành công trên chính dataset ban đầu
- trả đúng `3300000000`
- không bỏ row
- không giảm dữ liệu mẫu
- không hard-code tổng
- giữ nguyên business filter `MerchantId = 42`

`verify.ps1` chạy trực tiếp `starter/query.sql`, tức là kiểm tra code learner đã sửa chứ không kiểm tra reference solution.
