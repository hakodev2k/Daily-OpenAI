# Reference Solution — chỉ xem sau khi đã tự thử

## Symptoms

Artifact manifest có `templates/invoice.html`, nhưng runtime lookup bằng path cấu hình không tìm thấy file trong môi trường có filesystem case-sensitive.

## Evidence

- File có mặt trong artifact.
- Working directory không thay đổi.
- Lookup key khác artifact entry ở casing của directory và filename.
- Failure tái hiện ổn định với `StringComparer.Ordinal`.

## Root cause

Cấu hình dùng `Templates/Invoice.html` trong khi artifact thực tế chứa `templates/invoice.html`. Windows thường che giấu loại lỗi này vì filesystem phổ biến trên Windows lookup filename không phân biệt casing; Linux thường coi casing là một phần của path contract.

## Vì sao fix hoạt động

Sửa `configuredPath` trong `starter/Program.cs` thành:

```csharp
var configuredPath = "templates/invoice.html";
```

Path runtime lúc này khớp chính xác artifact entry, nên behavior không còn phụ thuộc OS.

## Cách verify

Chạy:

```powershell
./scripts/verify.ps1
```

Expected: process exit code `0` và in `PASS`.

## Alternative fixes

- Generate path từ một shared constant/manifest thay vì duplicate string giữa build config và runtime config.
- Thêm deployment test chạy artifact trên Linux CI runner/container.
- Validate configured resource paths khi application startup.

## Wrong / Tempting fixes

- Chuyển lookup sang case-insensitive: có thể che mismatch và tạo behavior khác filesystem thật.
- Copy thêm cả hai biến thể filename: làm artifact khó kiểm soát và không xử lý nguyên nhân.
- Catch lỗi rồi fallback về embedded template: chỉ hợp lý nếu đó là product requirement rõ ràng, không phải default fix.

## Production implications

Cross-platform deployment cần coi filename/path giống API contract. Các assumption được Windows che giấu nên được kiểm tra trong CI bằng environment gần production.

## Trade-offs

Startup validation tăng độ tin cậy nhưng làm startup fail sớm nếu artifact/config lệch. Với resource bắt buộc, fail-fast thường tốt hơn để lỗi xuất hiện ngay lúc deploy thay vì request đầu tiên.

## Senior engineer nên nhận ra

Một lỗi chỉ xuất hiện sau Windows → Linux migration không đồng nghĩa Linux có bug. Trước tiên hãy liệt kê các environment semantics khác nhau: path casing, separator, permissions, working directory, timezone, locale và native dependencies; sau đó dùng evidence để loại trừ từng hypothesis.
