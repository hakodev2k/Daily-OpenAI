# Reference Solution — CHỈ XEM SAU KHI TỰ ĐIỀU TRA

## 1. Symptoms

Reservation R-101 từ đối tác ghi 7 units, nhưng object ingest ghi 0; không có exception. Một payload thiếu số lượng cũng cho kết quả tương tự. Inventory có nguy cơ cấp phát vượt tồn kho thực sự còn khả dụng.

## 2. Evidence

So sánh `R-100` (control) với `R-101` và `R-102`: JSON raw có sự khác biệt về field; giá trị object sau deserialize thay đổi trước khi ghi ledger. Điều này loại trừ giả thuyết primary về SQL transaction, cache hay network retry trong fixture.

## 3. Root cause

`JsonSerializerDefaults.Web` hỗ trợ camelCase và case-insensitive matching, **không** tự chuyển snake_case `reserved_units` sang `ReservedUnits`. Mặc định, property JSON không map bị bỏ qua và property `int` không có giá trị được gán giữ mặc định `0`. Vì vậy ingest thành công về mặt cú pháp nhưng sai contract nghiệp vụ.

## 4. Why the fix works

Reference dùng `required` cho các field bắt buộc, `JsonUnmappedMemberHandling.Disallow` để phát hiện field không được hỗ trợ, camelCase contract rõ ràng, kiểm tra version, identity và quantity. `reservedUnits:0` tường minh vẫn hợp lệ, khác hoàn toàn với việc thiếu field. Copy [reference code](ReservationReader.cs) sang `starter/ReservationReader.cs` **chỉ để đối chiếu sau khi tự sửa**.

## 5. How to verify

Chạy `./verify.ps1` (hoặc `dotnet run --project ./checks/Checks.csproj -- --verify`). Kỳ vọng 8/8 PASS và exit code 0. Tests link trực tiếp vào `starter/ReservationReader.cs`, không link vào solution. Kiểm tra cả v1 hợp lệ, zero tường minh, rename, missing, negative, unsupported version, unknown field và blank identity.

## 6. Alternative fixes

- Parse `JsonDocument` và validate field whitelist / presence / type bằng tay; kiểm soát tốt nhưng code dài và dễ bỏ sót.
- Dùng JSON Schema validation ở ingress (thư viện được pin version) rồi deserialize; hữu ích nếu nhiều contract và nhiều client, đổi lại thêm dependency.
- Khi đối tác chính thức hỗ trợ v2, tạo DTO v2 và chuyển đổi có version discriminator; không âm thầm đổi policy của v1.

## 7. Wrong or tempting fixes

- `if (ReservedUnits == 0) throw`: từ chối cả phiếu có 0 tường minh và không phân biệt missing.
- Thêm `PropertyNameCaseInsensitive=true`: không giải quyết dấu gạch dưới.
- Thay tên property DTO theo payload mới: có thể làm hỏng v1 và che khuất versioning.
- Catch exception rồi dùng 0: biến dữ liệu không hợp lệ thành thành công giả.
- Sửa dữ liệu ledger bằng SQL thủ công mà không sửa ingress: sự cố tái diễn ở batch kế tiếp.

## 8. Production implications

Đưa payload lỗi vào quarantine / dead-letter với correlation ID; alert theo tỷ lệ rejected schema, supplier version, số lượng 0 bất thường. Reconcile những reservation đã ingest trong khoảng thay đổi, kiểm soát idempotency khi replay, tránh log raw PII/secret. Đánh giá tác động trước khi cho phép xuất kho tiếp.

## 9. Trade-offs

Strict unknown-member policy bắt lỗi drift sớm nhưng có thể từ chối field mở rộng vô hại. Với đối tác có contract extensible, nên cho phép extension field có namespace/allowlist rõ ràng và vẫn bắt buộc field cốt lõi. Không tự động chấp nhận schemaVersion=2 khi chưa có DTO/migration test.

## 10. What a Senior engineer should notice

HTTP 200 và không có exception không đồng nghĩa semantic correctness. Cần phân biệt **wire schema**, **domain invariants**, **ingress rejection policy**, **data repair** và **rollout compatibility**. Một fix đủ tốt phải kèm regression tests, cảnh báo drift và kế hoạch đối soát dữ liệu đã bị ảnh hưởng.
