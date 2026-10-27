# Investigation guide
1. Chạy reproduction; lưu hex của encoded frame và expected vector.
2. So sánh byte 0–3, 4–5, 6–7 và 8–15; tìm pattern thay đổi.
3. Phân biệt textual UUID, in-memory Guid và wire representation.
4. Loại trừ giả thuyết wrong ID, truncated frame, và byte-order mismatch bằng evidence.
5. Định nghĩa acceptance contract ở cả encode và decode; local round-trip không đủ.
6. Sửa starter, chạy verification, không sửa checks để tạo PASS.
