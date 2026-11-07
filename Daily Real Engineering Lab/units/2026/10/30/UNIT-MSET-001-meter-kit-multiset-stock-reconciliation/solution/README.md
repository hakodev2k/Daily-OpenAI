# Reference Solution — xem sau khi tự điều tra

## Symptoms
Một SKU có số lượng thiếu/thừa nhưng báo cáo chênh lệch rỗng.

## Evidence
`[KIT-A,KIT-A,KIT-B]` đối chiếu `[KIT-A,KIT-B]` cần thiếu một `KIT-A`. Ba `KIT-X` đối chiếu một cần thiếu hai; hai `KIT-Z` quét ngoài manifest cần thừa hai.

## Root cause
`Enumerable.Except` là phép hiệu **set**: nó không giữ số lần xuất hiện của cùng SKU. Nếu SKU tồn tại ở cả hai danh sách, số lượng lệch có thể bị che giấu.

## Why the fix works
[Program.cs](Program.cs) đếm số occurrence của phía đối chiếu bằng `Dictionary<string,int>`; mỗi occurrence của phía nguồn tiêu thụ tối đa một lần. Những đơn vị không ghép được là chênh lệch. Chạy hai chiều để có `Missing` và `Unexpected`, dùng `StringComparer.Ordinal`.

## How to verify
Sửa `starter/Program.cs` bằng giải pháp riêng rồi chạy `./verify.ps1`. Script này chỉ kiểm tra starter, không chạy reference. Có thể kiểm tra reference riêng bằng `dotnet run --project ./solution/InventoryReconciliation.csproj -- verify`.

## Alternative fixes and trade-offs
`GroupBy` + `Count` phù hợp báo cáo tổng hợp; `Dictionary` giữ thứ tự occurrence và O(n+m) kỳ vọng, O(u) bộ nhớ với u SKU khác nhau. Xóa từng occurrence từ `List` đơn giản cho batch nhỏ nhưng có thể O(n×m). Với dữ liệu ở DB, `GROUP BY SKU` hữu ích nếu bảo đảm snapshot nhất quán.

## Wrong / tempting fixes
`Distinct` làm mất thêm thông tin số lượng. `Contains` chỉ kiểm tra tồn tại. `OrderBy` trước `Except` không biến set thành multiset. Scale worker hay thêm cache không giải quyết lỗi tính đúng.

## Production implications
Phải phân biệt SKU với serial của từng bộ; duplicate event do retry khác với hai bộ vật lý chung SKU. Chốt manifest version, batch scan, normalization contract và transaction khi cập nhật tồn kho.

## What a Senior engineer should notice
Làm rõ mô hình dữ liệu trước khi chọn thuật toán, xác định tính nhất quán và fixture regression cho mọi biên. Đo quy mô trước khi tối ưu. Validation mode: static-review; không tuyên bố đã build/chạy.
