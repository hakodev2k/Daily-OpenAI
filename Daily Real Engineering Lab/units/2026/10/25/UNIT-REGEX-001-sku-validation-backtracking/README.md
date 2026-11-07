# UNIT-REGEX-001 — SKU import: kiểm tra dữ liệu bị treo ở một đầu vào bất thường

## Mục tiêu
Điều tra sự cố hiệu năng trong validation dữ liệu đầu vào của một pipeline import, phân biệt dữ liệu sai hợp lệ với lỗi vận hành, và chứng minh bản sửa bằng kiểm thử hồi quy.

## Bối cảnh thực tế
Nhà cung cấp gửi danh mục hàng nghìn SKU. SKU hợp lệ chỉ gồm chữ in hoa A–Z và chữ số 0–9, dài 1–2048 ký tự. Phần lớn bản ghi được xử lý tức thì; đôi khi một SKU không hợp lệ khiến tác vụ import báo timeout, dù máy không bị quá tải.

## Bạn cần làm gì
1. Chạy bản starter, quan sát đầu vào thông thường và đầu vào bất thường.
2. Ghi ít nhất hai giả thuyết trước khi xem hints.
3. Khoanh vùng thời gian xử lý validation; phân biệt việc từ chối dữ liệu với lỗi timeout.
4. Sửa **starter/SkuValidator.cs** mà không nới lỏng grammar SKU.
5. Chạy verify trên **starter đã sửa**; đối chiếu reference solution sau cùng.

## Yêu cầu môi trường
.NET SDK 10 (khuyến nghị 10.0.100 hoặc feature band tương thích), PowerShell 7 hoặc Windows PowerShell; không cần database, cloud hay NuGet bên ngoài. File global.json có chính sách roll-forward trong cùng major version.

## Chạy nhanh
Từ thư mục unit:
~~~powershell
./setup.ps1
./run.ps1
~~~
Hoặc dùng CLI:
~~~powershell
dotnet run --project ./starter/RegexLab.csproj -- --demo
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Script chỉ thành công khi **starter gốc** tái hiện triệu chứng timeout dự kiến; đây **không phải** kiểm tra bản sửa. Khi đã sửa starter, chạy verify thay vì reproduce.

## Những gì cần quan sát
- SKU thông thường được chấp nhận hay từ chối?
- SKU sai cú pháp thông thường trả kết quả gì?
- Đầu vào dài gần hợp lệ trả về false hay ném exception?
- Một bản ghi có thể ảnh hưởng thời gian của cả batch như thế nào?
- Nếu không tái hiện: xác nhận đang dùng starter gốc, .NET SDK 10, và xem thời gian timeout của môi trường; không tăng timeout để coi là đã sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
⚠️ Spoiler: chỉ xem sau khi đã điều tra và tự sửa: [Giải pháp tham khảo](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).
Lệnh **./verify.ps1** phải exit 0 và hiển thị 10/10 kiểm thử PASS, bao gồm đầu vào bất thường; không được chỉ kiểm tra solution project.

## Estimated Time
40 phút. Ghi nhận tại [workspace/my-investigation.md](workspace/my-investigation.md).
