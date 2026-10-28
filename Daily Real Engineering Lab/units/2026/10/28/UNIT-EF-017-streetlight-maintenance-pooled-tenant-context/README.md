# UNIT-EF-017 — Dashboard bảo trì đèn đường trả work order của tenant khác

## Mục tiêu
Điều tra một sự cố cô lập dữ liệu trong dịch vụ .NET dùng EF Core: tái hiện triệu chứng, thu thập bằng chứng theo thứ tự request, kiểm tra giả thuyết, sửa lỗi trong vùng code được phép và xác nhận regression contract.

## Bối cảnh thực tế
Một SaaS quản lý bảo trì đèn đường phục vụ nhiều đơn vị vận hành. Sau đợt tối ưu throughput, dashboard của một đơn vị đôi lúc hiển thị work order của đơn vị khác. Không có exception, truy vấn nhanh và dữ liệu gốc trong database vẫn đúng. Đội vận hành cần một bản sửa có thể kiểm chứng trước khi mở lại dashboard.

## Bạn cần làm gì
1. Chạy baseline và ghi lại tenant được yêu cầu, mã work order trả về, thứ tự request và kết quả các assertions.
2. Đưa ra ít nhất ba giả thuyết: dữ liệu seed, điều kiện lọc, vòng đời object hoặc logic phân giải tenant; tìm evidence để bác bỏ từng giả thuyết.
3. Chỉ sửa **starter/WorkOrderReader.cs**. Không sửa harness, model, expected data hay reference solution.
4. Chạy verify và chứng minh cả tenant có dữ liệu lẫn tenant rỗng đều được cô lập; không được giải quyết bằng cách trả danh sách rỗng cho mọi request.
5. Sau khi tự sửa, đọc reference solution và đánh giá các trade-offs khi dùng EF Core trong multi-tenant SaaS.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1 hoặc 7, kết nối NuGet để restore Microsoft.EntityFrameworkCore.Sqlite 8.0.10 lần đầu. SQLite chạy in-memory; không cần Docker, Azure, tài khoản hay API key.

## Chạy nhanh
~~~powershell
./reproduce.ps1
~~~

## Cách reproduce vấn đề
Từ thư mục unit, chạy **./reproduce.ps1**. Script chỉ báo **REPRODUCTION_CONFIRMED** khi harness build và chạy được nhưng các isolation assertions của starter thất bại với exit code dự kiến. Build error không được tính là reproduce thành công.

## Những gì cần quan sát
- Request nào trả sai dữ liệu? Thứ tự request có quan trọng không?
- Kết quả có thay đổi khi gọi tenant chưa có work order?
- Database có thực sự chứa bản ghi bị gán nhầm tenant hay không?
- Không có SQL exception và latency thấp giúp loại trừ những giả thuyết nào?
- Vì sao một lần chạy đơn lẻ có thể không phát hiện vấn đề?

Xem [incident evidence](evidence/incident.md), sau đó ghi vào [investigation workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi đã thử sửa:** [giải thích](solution/README.md) · [reference code](solution/WorkOrderReader.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
60 phút · L3 / D4 · Validation: **static-review** (chưa chạy .NET build hoặc tests khi xuất bản).
