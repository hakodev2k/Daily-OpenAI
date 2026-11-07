# UNIT-UTF8-001 — Bản tin cứu hộ biển bị méo chữ khi nhận JSONL streaming

## Mục tiêu
Điều tra tính toàn vẹn dữ liệu văn bản của JSON Lines khi một .NET receiver nhận cùng payload từ gateway qua các lần đọc có kích thước khác nhau.

## Bối cảnh thực tế
Trung tâm điều phối cứu hộ ven biển nhận hai bản tin JSONL có mô tả tiếng Việt và biểu tượng hiện trường. Gateway xác nhận byte payload đúng. Receiver vẫn parse được hai JSON object, nhưng có phiên hiển thị sai nội dung của bản tin đầu tiên, ảnh hưởng phân loại sự cố.

## Bạn cần làm gì
1. Reproduce, ghi output và đưa ra tối thiểu hai giả thuyết.
2. So sánh byte nguồn, ranh giới các lần đọc, record framing và nội dung sau parse.
3. Chỉ sửa `starter/Utf8LineReader.cs`, giữ nguyên harness/fixture.
4. Verify mã bạn sửa, rồi mới đối chiếu reference.

## Yêu cầu môi trường
.NET 10 SDK, PowerShell; không cần Docker, Azure, tài khoản cloud hay NuGet ngoài framework.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
```
Có thể chạy `dotnet run --project baseline/Utf8StreamLab.csproj -- --reproduce` nếu PowerShell ExecutionPolicy chặn script.

## Cách reproduce vấn đề
`./reproduce.ps1` chạy baseline bất biến, exit 0 khi quan sát được triệu chứng dự kiến. `./run.ps1` chạy starter mà bạn có thể chỉnh sửa.

## Những gì cần quan sát
Đếm record, so sánh expected/actual text, đếm ký tự thay thế; thay đổi phân mảnh mà không đổi payload; kiểm tra cuối luồng khi nhận dữ liệu chưa hoàn chỉnh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Chỉ xem sau khi tự sửa](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `./verify.ps1` kiểm tra mã learner trong `starter/`, không thay bằng reference.

## Estimated Time
50–65 phút. [Ghi điều tra](workspace/my-investigation.md).
