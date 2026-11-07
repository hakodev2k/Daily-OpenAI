# Incident fixture (synthetic)
Nguồn: local fixture, không phải production log thực.

| Bước | Kỳ vọng | Quan sát baseline |
|---|---|---|
| Register SEED-041 | Thành công | Thành công |
| Correct SEED-041 → SEED-042 | Thành công | Thành công |
| Lookup SEED-042 | Found | Not found |
| Register SEED-042 lần nữa | Rejected | Accepted |
| Remove SEED-042 | Registry rỗng | Còn 1 bản ghi |

Điều tra trạng thái trong bộ nhớ và so sánh các thao tác; không suy luận chỉ từ thông báo UI.
