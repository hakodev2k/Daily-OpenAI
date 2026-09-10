> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Starter preload đầy đủ Order graph rồi thực hiện filtered Include trong cùng DbContext, nhưng collection cuối cùng vẫn chứa cả line `Closed`.

## Evidence

- Store có 2 line `Open` và 1 line `Closed`.
- Bước preload đưa cả 3 line vào ChangeTracker.
- Query sau có filter nhưng entity graph trả về vẫn có 3 line.

## Root cause

EF Core navigation fixup kết nối các entity đã được tracking vào navigation tương ứng. Với tracking query, filtered Include không phải là một hàng rào loại bỏ các related entity đã có trong ChangeTracker. Vì toàn bộ OrderLine đã được preload, graph sau cùng được fixup lại với các entity đó.

## Why the fix works

Reference solution dùng `AsNoTracking()` cho filtered read. Kết quả query không được hợp nhất với graph đang tracking trong DbContext, nên collection phản ánh tập dữ liệu mà query thực sự materialize.

## How to verify

Chạy `verify.ps1` sau khi sửa `starter/`. Kết quả phải có `returnedLines=2`, không chứa `Closed`, và bước preload vẫn trả 3 line.

## Alternative fixes

- Dùng DbContext mới cho read model cần isolation khỏi tracking state trước đó.
- Projection trực tiếp sang DTO với `Select`, đặc biệt phù hợp endpoint read-only.
- Explicitly load collection với query riêng khi cần kiểm soát lifecycle và tracking rõ ràng.

## Wrong or misleading fixes

- Lọc collection chỉ sau khi response đã được materialize có thể che symptom nhưng vẫn giữ graph tracking rộng hơn cần thiết.
- Xóa bước preload chỉ làm mất điều kiện gây lỗi trong lab, không giải quyết contract khi code thực tế cần preload.
- Gọi `ChangeTracker.Clear()` bừa bãi có thể phá các entity đang cần tracking cho phần khác của unit-of-work.

## Production implications

Tracking state là một phần của semantics của query. Read endpoint hoặc read phase phức tạp nên có explicit tracking policy thay vì dựa vào default một cách ngầm định.

## Trade-offs

`AsNoTracking()` giảm tracking overhead và tránh fixup từ state cũ, nhưng không phù hợp nếu các entity đó phải được sửa và `SaveChanges()` trong cùng context. Projection thường rõ contract hơn nhưng có thêm mapping code.

## What a Senior engineer should notice

SQL/filter đúng chưa đủ để kết luận object graph cuối cùng đúng. Khi EF Core tracking được bật, phải reasoning cả query semantics lẫn state đã tồn tại trong ChangeTracker.
