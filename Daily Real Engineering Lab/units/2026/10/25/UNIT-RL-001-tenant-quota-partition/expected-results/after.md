# After — acceptance criteria

`verify.ps1` phải in `VERIFICATION_PASSED`, exit 0 và `failed_scenarios=0`.

- A: Alpha dùng 2 permits không ảnh hưởng đến permit đầu tiên của Beta.
- B: Alpha dùng tổng cộng 2 permits xuyên endpoint; request thứ ba bị chặn.
- C: các request anonymous chia sẻ một ngân sách fallback, không được mở rộng bằng cách đổi path.
- D: cùng tenant cùng endpoint vẫn bị chặn ở request thứ ba.

Kiểm tra thêm bằng suy luận: tenant ID từ user claims phải là định danh đáng tin cậy do auth middleware xác lập. Khi triển khai nhiều replicas, limiter in-memory không tự đồng bộ quota toàn cụm. Bài lab không khẳng định đã kiểm thử HTTP middleware, Redis, hay distributed quota.
