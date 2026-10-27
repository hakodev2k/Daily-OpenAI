# UNIT-INIT-001 — Ruleset thẩm định bảo hiểm không phục hồi sau lỗi tạm thời

## Mục tiêu
Tái hiện sự cố phục hồi sau lỗi transient trong .NET, phân biệt tình trạng upstream với trạng thái process và sửa contract khởi tạo dữ liệu dùng chung mà không gây tải trùng.

## Bối cảnh thực tế
Dịch vụ thẩm định hợp đồng bảo hiểm tải ruleset từ đối tác trước khi tính eligibility. Lần đầu process khởi động, đối tác trả HTTP 503. Sau đó đối tác đã phục hồi nhưng mọi yêu cầu trong cùng process vẫn thất bại. Khởi động lại ứng dụng có thể hoạt động bình thường. Provider giả lập hoàn toàn local.

## Bạn cần làm gì
1. Chạy starter, ghi lại số lần provider được gọi và exception của hai request đầu.
2. Ghi ít nhất hai hypothesis: upstream tiếp tục lỗi hay trạng thái nội bộ không phục hồi.
3. Chỉ chỉnh sửa starter/RulesetSnapshotCache.cs; không sửa provider hoặc harness.
4. Giữ contract: lỗi transient có thể thử lại; sau thành công snapshot được tái sử dụng; concurrent cold callers không tạo nhiều fetch.
5. Chạy verification rồi mới đối chiếu reference solution.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1+ hoặc 7+. Không cần Docker, database, NuGet bên ngoài hay cloud.

## Chạy nhanh
Tại thư mục unit:

~~~powershell
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Script trả exit code 0 chỉ khi xác nhận đúng triệu chứng; lỗi build không phải reproduce thành công.

## Những gì cần quan sát
- Lần gọi đầu nhận lỗi 503 giả lập.
- Lần gọi tiếp theo có chạm tới provider không?
- Sau khi provider khỏe, snapshot có xuất hiện không?
- Khi nhiều request đồng thời đọc ruleset, có bao nhiêu lần fetch?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter/.
4. Verify bằng ./verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ mở sau khi đã tự điều tra và thử fix.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
40 phút · L1 / D3 · Validation: static-review (chưa chạy code trong môi trường tạo lab).
