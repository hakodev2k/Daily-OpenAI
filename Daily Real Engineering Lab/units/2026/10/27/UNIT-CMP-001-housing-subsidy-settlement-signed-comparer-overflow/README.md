# UNIT-CMP-001 — Thứ tự đối soát trợ cấp nhà ở sai ở dữ liệu biên

## Mục tiêu

Tự reproduce, điều tra và sửa một sai lệch thứ tự dữ liệu trong pipeline đối soát .NET. Rèn kỹ năng xác định hợp đồng sắp xếp, thu thập bằng chứng từ fixtures, kiểm tra điều kiện biên và thiết kế regression tests.

## Bối cảnh thực tế

Một batch đối soát trợ cấp nhà ở tổng hợp các khoản điều chỉnh có dấu: khoản hoàn trả âm, khoản truy thu dương. Hệ thống xuất danh sách theo DeltaMinorUnits tăng dần; nếu bằng nhau, Reference phải tăng dần theo thứ tự ordinal. Partner phát hiện vài batch có thứ tự khác với hợp đồng dù các batch nhỏ thông thường vẫn đúng.

## Bạn cần làm gì

1. Chạy bản starter chưa sửa, ghi lại thứ tự thực tế và kỳ vọng của ba nhóm dữ liệu.
2. Đưa ra ít nhất hai giả thuyết, phân biệt lỗi dữ liệu đầu vào và lỗi xử lý.
3. Chỉ chỉnh sửa code dưới starter/ để khôi phục đúng hợp đồng; không thay expected fixtures.
4. Chạy verify và ghi kết quả vào workspace/my-investigation.md.
5. Sau khi hoàn thành, mới đọc reference solution và so sánh trade-offs.

## Yêu cầu môi trường

- .NET SDK 10.0.100 (hoặc bản cùng feature band phù hợp), PowerShell 7 hoặc Windows PowerShell.
- Không cần Docker, NuGet bên ngoài, tài khoản cloud hay database.

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/10/27/UNIT-CMP-001-housing-subsidy-settlement-signed-comparer-overflow"
./setup.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề

Chạy ./reproduce.ps1 trên starter nguyên bản. Script chỉ báo REPRODUCED khi chương trình phát hiện vi phạm ở fixture boundary và trả exit code 2 như dự kiến. Có thể chạy ./run.ps1 để xem cả ba fixture; exit code khác 0 ở starter là dấu hiệu có lỗi cần điều tra, không phải lỗi setup.

## Những gì cần quan sát

- Thứ tự expected và actual của từng fixture; so sánh ordinary, boundary và tie.
- Phân biệt thứ tự tăng dần theo số với thứ tự khi hai giá trị bằng nhau.
- Xác định tính chất của dữ liệu nào khiến lỗi chỉ xuất hiện ở một số batch.
- Không sửa expected fixture để làm test xanh.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ đọc sau khi tự sửa:** [Reference Solution](solution/README.md).

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

35–50 phút (L1 / D3).
