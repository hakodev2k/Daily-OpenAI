# Evidence — gate seal intake

Dữ liệu giả lập, không có thông tin khách hàng thật.

| Tín hiệu | Giá trị ghi nhận |
|---|---|
| Input fixture | 3 scans (2 đạt, 1 không đạt) |
| Batch accepted counter | 3 |
| Persisted receipts | 0 |
| Rejected scan counter | 0 |
| Remaining scan source | 0 |
| Adapter enumerations | 2 |
| Process exception | không có |
| Destination availability | hoạt động với replayable input |

Timeline: 08:00 adapter mở batch; 08:00:01 batch được xác nhận có 3 scan; 08:00:02 không có phiếu mới; 08:00:03 operator thử lại bằng replayable test fixture và thấy 2 phiếu. Hãy phân biệt vấn đề ở source, business validation, destination và report logic. Evidence này chỉ là quan sát; không phải kết luận nguyên nhân.
