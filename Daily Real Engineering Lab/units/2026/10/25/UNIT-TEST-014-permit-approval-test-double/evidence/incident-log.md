# Evidence — trích xuất tình huống giả lập

| Nguồn | Mốc | Kết quả |
|---|---|---|
| CI unit-test suite | 09:10 | Approval returns `true`; assertion on fake state passes |
| API response | 09:14 | `permitId=P-204`, `accepted=true` |
| Read-after-write từ request mới | 09:15 | `permitId=P-204`, `status=Pending`, `version=7` |
| Connection monitor | 09:15 | Không có lỗi kết nối hay timeout |
| Retry thủ công | 09:18 | API tiếp tục báo `accepted=true` |

Các dòng này là dữ liệu mô phỏng phục vụ điều tra. Chưa có kết luận root cause. Dùng `observe` và `reproduce` để kiểm chứng thay vì tin một nguồn duy nhất.
