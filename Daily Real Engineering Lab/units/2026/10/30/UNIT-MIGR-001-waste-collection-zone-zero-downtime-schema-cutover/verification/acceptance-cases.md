# Acceptance replay — tabletop, không phải runtime test

Điền từng case vào workspace. Với mỗi case cần nêu cơ chế, bằng chứng, rollback và người chịu trách nhiệm.

| Case | Input | Kết quả bắt buộc |
|---|---|---|
| A | V1 insert `Central` khi V2 đang chạy | Không lỗi schema, V2 đọc nhất quán |
| B | Route 1203 `Old Town` có hai mapping | Không tự chọn zone; manual review, không dispatch sai |
| C | Route 1204 `Unknown Pier` | Không gán default; quarantine và metric |
| D | Backfill đọc route 1205 = `North Dock`, V1 đổi thành `Central` trước batch write | Không stale overwrite; catch-up phát hiện |
| E | Backfill crash sau 2 triệu hàng, restart | Resume idempotent, không trùng/sai, throttle đảm bảo SLO |
| F | V2 ghi zone mới rồi rollback về V1; có đề nghị drop text khi V1 vẫn chạy | V1 vẫn đọc được, drop bị chặn |

**Pass:** cả A–F có evidence/verification plan; 5 metrics và abort gates; 2 alternatives; rollback không gây mất write; unresolved records không được tự dispatch; change request được xử lý. Chỉ ghi `static-review`, không tuyên bố đã benchmark.
