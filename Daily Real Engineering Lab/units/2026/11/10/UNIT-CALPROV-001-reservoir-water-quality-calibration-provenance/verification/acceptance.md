# Verification — checklist và replay matrix

Điền giá trị cụ thể, khóa định danh và hành vi API cho mỗi case; không chỉ trả lời “pass”.

| Case | Thao tác | Invariant cần chứng minh |
|---|---|---|
| A | Đọc R-204/revisions/1 trước và sau T2 | payload/hash không đổi |
| B | Tạo reading M-73 sau T2 | dùng cấu hình mới đúng effective interval |
| C | Hiệu chỉnh hồi tố M-71 do lỗi thiết bị | tạo revision 2 có phê duyệt; revision 1 vẫn truy xuất được |
| D | Đồng thời hai người phát hành revision 2 | chỉ một revision được commit; không có hai “current” |
| E | Đọc dữ liệu legacy thiếu calibration ID | gắn trạng thái provenance-unknown; không tự khẳng định số liệu |
| F | Calibration có interval chồng lấn | từ chối hoặc quarantine; không chọn tùy tiện |
| G | API cache trả report current sau correction | cache key có revision hoặc invalidation rõ ràng |

## Quality gate
- [ ] T1/T2 tính đúng số liệu và giải thích sai lệch
- [ ] Report revision bất biến, signed payload được lưu nguyên dạng
- [ ] Raw reading và calibration version có provenance
- [ ] Correction không phá audit trail
- [ ] Có transaction / optimistic concurrency cho publish
- [ ] Có ít nhất hai trade-off về storage, migration hoặc consistency
- [ ] Có test cho từng case A–G
