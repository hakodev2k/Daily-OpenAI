# Incident evidence — synthetic

Bản ghi origin (được cấp quyền theo tuple tenant/report):

| Tenant | Report | Nội dung |
|---|---|---|
| agency:west | deck-9 | WEST:restricted-inspection |
| agency | west:deck-9 | CENTRAL:public-summary |
| north | bridge-1 | NORTH:normal |

Quan sát từ starter:
- NORMAL_REPEAT: hai lượt đọc cùng một tuple trả đúng nội dung, OriginReads=1, CacheHits=1, CacheEntries=1.
- COLLISION_FORWARD: lượt đầu đúng; lượt thứ hai trả WEST:restricted-inspection thay vì CENTRAL:public-summary. Chỉ một origin read được ghi nhận.
- COLLISION_REVERSE: lượt đầu đúng; lượt thứ hai trả CENTRAL:public-summary thay vì WEST:restricted-inspection. Chỉ một origin read được ghi nhận.
- MISSING_ORIGIN: lượt đầu tìm thấy bản ghi; lượt thứ hai lẽ ra không tồn tại tại origin nhưng vẫn có dữ liệu trả về.

Noise để phân loại: không có truy cập network, không có race condition, không có thay đổi dữ liệu nguồn. Các lượt thử tạo instance cache mới để loại trừ trạng thái từ lần chạy trước.

Tự đo thêm: instrument một cache hit và một origin read cho từng request, nhưng không log nội dung nhạy cảm ở production.
