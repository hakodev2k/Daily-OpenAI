# Incident evidence (synthetic, deterministic)

- 2026-10-28 00:03 UTC: 6 observations đã được lưu trong upstream fixture.
- 00:04 UTC: client lấy feed theo `pageSize=2` với HTTP success ở mọi lượt.
- Upstream count: 6; downstream count: 5; không có exception.
- Page 1: `obs-a,obs-b` (cùng thời điểm).
- Page 2: `obs-d,obs-e` (thời điểm kế tiếp).
- Page 3: `obs-f`.
- Page 4: empty. Bản ghi `obs-c` không được nhận.
- Nhiễu: log của thiết bị `reef-east` báo chậm 250 ms; fixture local không mô phỏng network timeout.

Hãy kiểm tra quy luật ranh giới trang, chứ không suy luận nguyên nhân từ một log latency không liên quan.
