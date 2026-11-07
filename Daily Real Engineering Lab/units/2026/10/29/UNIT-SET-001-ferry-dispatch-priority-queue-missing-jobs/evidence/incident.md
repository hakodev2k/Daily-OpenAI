# Incident evidence — fixture dự kiến (static review)
Bảng dưới là kỳ vọng suy ra từ fixture, **không phải log runtime đã được đo trong lần xuất bản**.

| Event | JobId | Priority | ReceivedSequence |
|---|---|---:|---:|
| 1 | F-204 | 5 | 10 |
| 2 | F-205 | 5 | 11 |
| 3 | F-206 | 3 | 12 |
| 4 | F-207 | 5 | 9 |
| 5 | F-208 | 5 | 11 |

Gateway nhận 5 yêu cầu; snapshot hàng đợi dự kiến chỉ có 2; không có exception. F-205 và F-208 có cùng sequence nhưng JobId khác. Không có downstream database, HTTP retry hoặc race trong fixture. Tách tín hiệu khỏi nhiễu: tập trung vào ranh giới nhận/chấp nhận/đọc hàng đợi.
