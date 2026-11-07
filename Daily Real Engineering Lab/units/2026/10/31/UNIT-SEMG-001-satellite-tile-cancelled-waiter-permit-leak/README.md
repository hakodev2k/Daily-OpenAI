# UNIT-SEMG-001 — Hàng đợi render bản đồ vệ tinh vượt giới hạn GPU sau yêu cầu bị hủy

## Mục tiêu

Điều tra một sự cố concurrency tại dịch vụ tạo map tile, phân biệt request bị hủy khi chờ và request đã được xử lý, bảo vệ bất biến tối đa một tác vụ GPU đồng thời.

## Bối cảnh thực tế

Dịch vụ xuất ảnh vệ tinh chạy các job render qua một GPU worker chỉ chịu được một tác vụ tại một thời điểm. Khi người dùng đóng tab, request đang chờ được hủy. Sau một đợt hủy, hệ thống thỉnh thoảng bắt đầu job thứ ba trong khi job đầu vẫn chạy; cuối batch có exception và dashboard báo capacity không nhất quán.

## Bạn cần làm gì

1. Chạy scenario gốc, ghi lại thứ tự vào vùng xử lý, tín hiệu hủy và exception.
2. Nêu ít nhất ba giả thuyết (race trong test, cơ chế giới hạn tài nguyên, lỗi worker).
3. Chỉ sửa code trong starter/TileGate.cs để giữ giới hạn đồng thời và không làm mất quyền hủy.
4. Chạy verification cho cả trường hợp hủy khi đang chờ, hủy sau khi vào vùng xử lý và hàng đợi bình thường.

## Yêu cầu môi trường

Windows PowerShell 7 hoặc Windows PowerShell 5.1 và .NET 8 SDK. Không cần GPU thật, Docker, database hay tài khoản cloud. Project target net8.0, không dùng NuGet ngoài framework.

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/10/31/UNIT-SEMG-001-satellite-tile-cancelled-waiter-permit-leak"
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Exit code 0 của reproduce nghĩa là harness đã chứng minh được triệu chứng **có lỗi**, không phải ứng dụng đã được sửa. Chạy trước khi thay đổi starter/.

## Những gì cần quan sát

- Job thứ ba có đi vào vùng xử lý khi job đầu chưa nhận tín hiệu kết thúc?
- Request thứ hai đã hủy có thực sự chạy callback không?
- Exception xuất hiện ở job nào và tại thời điểm nào?
- Một hàng đợi không có cancellation còn giữ thứ tự đúng không?

Đối chiếu với [evidence/expected-trace.md](evidence/expected-trace.md) và ghi giả thuyết vào [workspace/my-investigation.md](workspace/my-investigation.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng ./verify.ps1 (đọc code ở starter/).
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [solution/README.md](solution/README.md) và [solution/TileGate.cs](solution/TileGate.cs) — chỉ mở sau khi đã tự thử.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time

45–60 phút, L2 / D4. Có bài mở rộng dành cho Senior tại [extensions/senior-challenge.md](extensions/senior-challenge.md).
