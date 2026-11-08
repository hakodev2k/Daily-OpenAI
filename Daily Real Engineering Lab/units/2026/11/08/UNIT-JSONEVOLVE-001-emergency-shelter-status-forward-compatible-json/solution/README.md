# Reference solution — UNIT-JSONEVOLVE-001

Nguyên nhân: `JsonStringEnumConverter` ném `JsonException` khi status string chưa được định nghĩa trong enum. Việc deserialize nguyên mảng làm một phần tử mới khiến cả batch thất bại.

## Áp dụng
Sau khi điều tra, sao chép `solution/Program.cs` sang `starter/Program.cs`, rồi chạy `./verify.ps1`. Không sửa `verify.ps1`.

## Tại sao cách này đúng
Giữ `RawStatus` từ wire contract và ánh xạ giá trị đã biết sang domain enum; giá trị chưa biết trở thành `Unknown` và được đưa vào review, không bị gán mặc định sang `Available`. Không thay đổi payload nguồn. Import 3 records không bị chặn bởi giá trị mới.

## Trade-offs
- Có thể dùng DTO `string Status` và mapping layer riêng thay cho `JsonDocument`; phù hợp production hơn khi schema lớn.
- Cần phân biệt **unknown-but-valid** với payload hỏng, thiếu id, duplicate id hoặc JSON malformed. Không được nuốt mọi exception.
- Với batch lớn, stream parse và xử lý lỗi từng record để tránh memory pressure; cần xác định rõ partial-success semantics và idempotency.
- Metrics: `unknown_status_total`, `imported_total`, sample raw values giới hạn cardinality; alert khi tỷ lệ Unknown tăng.

## Wrong fixes
1. Bắt `JsonException` rồi trả list rỗng: mất toàn bộ dữ liệu và che lỗi.
2. Thêm `Standby` vào enum duy nhất: khắc phục tạm thời nhưng bản nâng cấp kế tiếp lại phá vỡ client.
3. Ép giá trị lạ thành `Available`: nguy hiểm khi điều phối người đến nơi trú ẩn không sẵn sàng.
4. Chỉ sửa solution mà không sửa starter: `verify.ps1` vẫn fail.

## Regression
Thử hoán đổi thứ tự các record, thêm status chưa biết khác, thiếu field bắt buộc và JSON sai cú pháp. Định nghĩa chính sách rõ ràng cho mỗi loại.
