# UNIT-JOPT-001 — Museum condition JSON export

## Mục tiêu

Điều tra lỗi xuất JSON chỉ xảy ra khi một tiến trình phục vụ liên tiếp hai kiểu báo cáo; phân biệt vấn đề dữ liệu với vấn đề vòng đời cấu hình, sửa lỗi mà không phá JSON contract.

## Bối cảnh thực tế

Bộ phận bảo tồn hiện vật xuất JSON compact cho cảm biến và JSON dễ đọc cho chuyên viên. Khi chạy từng yêu cầu độc lập, báo cáo hợp lệ; khi worker lần lượt phục vụ compact rồi pretty, lượt thứ hai trả lỗi. Nhân viên không nhận được báo cáo độ ẩm trước giờ kiểm kê.

## Bạn cần làm gì

1. Reproduce và ghi exception, thứ tự request, output lượt đầu.
2. Đưa ra ít nhất hai hypothesis và một phép thử phân biệt.
3. Sửa trong <code>starter/ConditionReportExporter.cs</code>, không sửa harness.
4. Verify chuỗi compact → pretty → compact → pretty và JSON contract.

## Yêu cầu môi trường

.NET SDK 8+, PowerShell; target net8.0, không cần database, Docker hoặc NuGet bên ngoài SDK.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
dotnet --version
./run.ps1
~~~

Trên Linux/macOS:

~~~bash
dotnet run --project starter/ConditionLab.csproj -- --reproduce
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Exit code 0 nghĩa là **đã thấy đúng lỗi baseline**; exit code khác 0 là không thấy lỗi mong đợi hoặc lỗi bất thường. Chạy trước khi sửa starter.

## Những gì cần quan sát

- Lượt đầu có JSON hợp lệ không? Exception phát sinh ở lượt nào?
- Khi giữ nguyên process và đổi thứ tự format, hành vi khác thế nào?
- Dữ liệu và camelCase property name có còn chính xác không?
- Hai loại format có ảnh hưởng lẫn nhau sau khi sửa không?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Verify.
5. Sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md)

~~~powershell
./verify.ps1
~~~

Verification chạy **mã starter do người học sửa**, không chạy reference solution.

## Estimated Time

35–45 phút.
