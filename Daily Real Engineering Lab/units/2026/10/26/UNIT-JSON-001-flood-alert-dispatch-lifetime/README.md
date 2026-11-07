# UNIT-JSON-001 — Flood Alert Dispatch: Accepted But Never Delivered

## Mục tiêu
Điều tra một lỗi xử lý dữ liệu JSON giữa bước tiếp nhận và bước dispatch; phân biệt lỗi dữ liệu đầu vào, lỗi hàng đợi và lỗi vòng đời dữ liệu. Đưa ra fix nhỏ, có regression verification.

## Bối cảnh thực tế
Dịch vụ cảnh báo ngập ven biển nhận bulletin JSON từ hai trạm, ghi nhận **accepted=2**, nhưng downstream không gửi được cảnh báo nào. Bộ kiểm tra JSON độc lập vẫn chấp nhận cả hai bulletin. Sự cố chỉ xuất hiện khi payload được xử lý ở giai đoạn dispatch sau bước intake.

## Bạn cần làm gì
1. Reproduce; ghi lại `intake.accepted`, `control.validated`, `dispatch.failed`, `summary`.
2. Đưa ra ít nhất hai giả thuyết cạnh tranh và cách kiểm chứng.
3. Sửa **chỉ trong `starter/`** để hai cảnh báo được dispatch đúng mà không kéo dài vòng đời tài nguyên không cần thiết.
4. Chạy verify và ghi evidence vào [workspace/my-investigation.md](workspace/my-investigation.md).

## Yêu cầu môi trường
- .NET SDK 8.0 (không cần Docker, database, Azure, NuGet bên thứ ba).
- PowerShell 7+ hoặc Windows PowerShell; terminal đặt tại thư mục unit.

## Chạy nhanh
```powershell
dotnet --version
./run.ps1
./reproduce.ps1
```
Nếu PowerShell execution policy không cho chạy file, dùng `pwsh -File ./reproduce.ps1` trong môi trường được phép hoặc chạy trực tiếp `dotnet run --project ./starter/FloodAlertLab.csproj -- --assert-broken`.

## Cách reproduce vấn đề
Từ bản `starter/` chưa sửa, chạy `./reproduce.ps1`. Script trả về thành công **chỉ khi tái hiện đúng triệu chứng ban đầu**, không có nghĩa đã fix. Nếu đã chỉnh code, dùng Git để xem lại bản starter gốc trước khi reproduce.

## Những gì cần quan sát
- Intake chấp nhận bao nhiêu bulletin? Bộ kiểm tra độc lập xác nhận bao nhiêu JSON hợp lệ?
- Loại exception tại dispatch là gì? Lỗi có xảy ra với cả hai ID không?
- Có bản tin nào được deliver không? Trạng thái sau fix có giữ đúng thứ tự, ID, zone, severity không?
- Không sử dụng kết quả `reproduce.ps1` làm bằng chứng đã sửa thành công.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`; đây là kiểm tra **learner-edited starter**, không phải solution.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution — chỉ đọc sau khi tự điều tra](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45–60 phút · L2 Investigation · D4 · validationMode: **static-review** (chưa chạy .NET trong môi trường tạo lab).
