# UNIT-SQL-001 — Transaction Retry Boundary and Duplicate Side Effects

## Mục tiêu
Điều tra một production incident xảy ra khi transient database failure kích hoạt retry quanh một workflow có cả database state và side effect bên ngoài.

## Bối cảnh thực tế
Dịch vụ finalize invoice thỉnh thoảng ghi nhận một invoice hợp lệ nhưng khách hàng nhận hai notification. Log cho thấy request gặp lỗi transient và được retry.

## Bạn cần làm gì
Reproduce incident, lập timeline từng attempt, xác định evidence nào thuộc database và evidence nào thuộc side effect, ghi hypothesis, sửa starter rồi verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy workload mô phỏng một outcome không rõ ràng ở attempt đầu tiên và retry toàn workflow.

## Những gì cần quan sát
Đếm số invoice committed, số notification được gửi và thứ tự event theo từng attempt. Phân biệt lỗi quan sát bởi caller với trạng thái thực tế của từng resource.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự làm](solution/README.md)

## Expected Results
Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
70 phút.