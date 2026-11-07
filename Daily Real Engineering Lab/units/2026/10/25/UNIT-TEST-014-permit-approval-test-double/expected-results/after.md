# After — khi fix hợp lệ
- `verify` exit 0 với cả fake và detached store.
- Một approval hợp lệ tạo đúng một lần ghi, `Status=Approved`, `Version=8` sau reload.
- Approval lần hai trả `false` và không tạo ghi bổ sung.
- Permit không tồn tại trả `false` và không tạo ghi.
- `reproduce` không còn phát hiện bất nhất ban đầu (exit 1 là bình thường sau fix).
