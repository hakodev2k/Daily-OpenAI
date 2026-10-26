# After — learner fix

Chạy ./verify.ps1: PASS cả 4 fixture, RESULT 4/4, exit 0.

- Một tuple đọc hai lần vẫn chỉ gây một origin read và một cache hit.
- Hai tuple khác nhau phải có hai cache entries và hai origin reads, không phụ thuộc thứ tự.
- Tuple không có tại origin phải trả KeyNotFoundException theo hợp đồng synthetic; không được nhận nội dung cache của tuple khác.
- Không thay đổi fixture hay bỏ cache chỉ để làm xanh test.

Production follow-up: version cache key, xử lý invalidation/migration các entry cũ, đặt TTL, kiểm tra mọi read/write path dùng cùng encoding, và audit nguy cơ exposure.
