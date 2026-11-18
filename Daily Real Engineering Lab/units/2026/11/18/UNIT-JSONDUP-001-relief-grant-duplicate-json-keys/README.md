# UNIT-JSONDUP-001 — Hồ sơ hỗ trợ y tế sai lệch giá trị sau import

## Mục tiêu
Điều tra hợp đồng JSON từ đối tác và chứng minh tính toàn vẹn dữ liệu.

## Bối cảnh thực tế
Một hồ sơ có giá trị khác nhau giữa raw payload và bản ghi nhập. Có thể lỗi nằm ở mapping, upstream hoặc serializer.

## Bạn cần làm gì
Reproduce, ghi hai hypotheses và bằng chứng, sửa starter/Program.cs, verify cả valid và adversarial input.

## Yêu cầu môi trường
.NET SDK 8, PowerShell, không cần cloud.

## Chạy nhanh
`./setup.ps1` và `./run.ps1`.

## Cách reproduce vấn đề
`./reproduce.ps1` chứng minh trạng thái starter vi phạm hợp đồng.

## Những gì cần quan sát
So sánh raw payload với giá trị sau import; phân biệt lỗi nhập liệu và lỗi xử lý.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/01.md), [Hint 02](hints/02.md), [Hint 03](hints/03.md).

## Reference Solution
**Spoiler:** [Reference](solution/README.md).

## Expected Results
Reproduce phát hiện payload mơ hồ được chấp nhận; verify chấp nhận valid payload và reject ambiguous payload.

## Estimated Time
45 phút.