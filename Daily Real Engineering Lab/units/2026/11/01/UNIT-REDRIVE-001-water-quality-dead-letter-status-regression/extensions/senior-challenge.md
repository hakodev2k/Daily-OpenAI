# Senior / Architect extension

1. Thiết kế schema PostgreSQL cho `projection`, `inbox`, `audit` và SQL transaction/UPSERT compare-and-set. Viết test với hai consumer đồng thời cùng version khác payload.
2. Thử crash ngay sau commit nhưng trước broker ack; chứng minh không lặp side effect.
3. Thêm policy xử lý event có cùng version và cùng payload nhưng EventId khác: quyết định dựa trên business semantics, không âm thầm chọn một.
4. Đổi nguồn phát sang **delta event** và mô tả vì sao thuật toán snapshot không còn đủ; thiết kế gap buffering + reconciliation.
5. Lập capacity estimate cho 1.5M deliveries/ngày, inbox retention 90 ngày và audit 1 năm; phân biệt storage và index overhead.
