# UNIT-DELIM-001 — Campus Shuttle: Bảng giờ xuất bến bị lệch trường

## Mục tiêu
Điều tra lỗi import dữ liệu đối tác từ triệu chứng hiển thị sai, thiết kế regression contract cho trường tùy chọn và trường bắt buộc, rồi chứng minh bản sửa không làm hỏng bản ghi hợp lệ.

## Bối cảnh thực tế
Đội vận hành xe shuttle của một trường đại học nhận dữ liệu xuất bến từ nhà cung cấp theo bản ghi phân tách bằng dấu `|`. Mỗi bản ghi có đúng năm vị trí theo thứ tự: `serviceId|platform|note|destination|status`. `note` có thể rỗng; bốn trường còn lại bắt buộc có giá trị. Dấu `|` không được phép xuất hiện bên trong giá trị.

Bảng điện tử hiển thị đúng các chuyến có ghi chú lên xe. Một số chuyến không có ghi chú lại hiện tên điểm đến ở ô ghi chú, còn ô trạng thái không khớp dữ liệu gốc. Import không báo lỗi.

## Bạn cần làm gì
1. Chạy hệ thống mẫu và lưu lại ít nhất hai bản ghi input/output.
2. Đưa ra ít nhất hai hypothesis có thể giải thích sự khác biệt.
3. Chạy bộ kiểm tra hợp đồng; ghi nhận case nào thất bại và vì sao.
4. Chỉnh **chỉ** `starter/DepartureParser.cs`, không sửa `harness/` hay `baseline/`.
5. Verify bản sửa với dữ liệu hợp lệ, thiếu trường, trường bắt buộc rỗng và thừa trường.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7; .NET SDK 8+.
- Không cần database, Docker, NuGet package ngoài hay Azure subscription.
- Có thể chạy lệnh từ bất kỳ thư mục nào sau khi đã `cd` tới thư mục unit.

## Chạy nhanh
```powershell
dotnet --version
./scripts/run.ps1
./scripts/reproduce.ps1
```

Nếu PowerShell chặn script, gọi trực tiếp `dotnet run --project baseline/Baseline.csproj -- --check` để quan sát baseline, và `dotnet run --project starter/Starter.csproj -- --check` để kiểm tra phần bạn sửa.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Script chạy **baseline bất biến** và chỉ thành công khi thấy `CONTRACT_FAIL` do sai hợp đồng; lỗi build hoặc lỗi môi trường không được coi là reproduce thành công. Chạy `./scripts/run.ps1` để quan sát bản ghi trên console.

## Những gì cần quan sát
- So sánh từng trường đầu vào với `note`, `destination`, `status` trong output.
- Đối chiếu các trường hợp có và không có ghi chú.
- Đếm số lượng case `CONTRACT_PASS` và `CONTRACT_FAIL`; chú ý cả trường hợp input sai định dạng nhưng vẫn được chấp nhận.
- Ghi lại output trước khi thay đổi mã nguồn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./scripts/verify.ps1` — script này kiểm tra **mã bạn sửa**, không kiểm tra reference solution.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi đã tự sửa và verify. Có thể kiểm tra riêng bằng `./scripts/verify-reference.ps1`.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
35 phút. Ghi quá trình điều tra trong [workspace](workspace/my-investigation.md).
