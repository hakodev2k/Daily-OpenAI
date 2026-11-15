# UNIT-URIBASE-001 — Đường dẫn API đối tác đổi bất ngờ sau migration

## Mục tiêu
Điều tra sai lệch endpoint routing khi nâng cấp partner API và xác định contract URL an toàn.

## Bối cảnh thực tế
Dịch vụ tra cứu tuyến xe liên tỉnh chuyển từ API v1 sang v2. Một số request vẫn nhận 404 từ gateway cũ, trong khi endpoint khác hoạt động. Log ghi đầy đủ host và path, không có network outage.

## Bạn cần làm gì
Reproduce từ fixture, ghi bảng bằng chứng cho bốn endpoint, đề xuất hai giả thuyết, sửa workspace trong `starter/Program.cs`, bổ sung ít nhất hai regression cases và verify. Không thay host hoặc API version để che triệu chứng.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell 7; không cần Internet, database hay API key.

## Chạy nhanh
`pwsh -File scripts/run.ps1`

## Cách reproduce vấn đề
`pwsh -File scripts/reproduce.ps1` phải trả exit 0 khi xác nhận mismatch ban đầu; `pwsh -File scripts/verify.ps1` phải thất bại trước khi sửa.

## Những gì cần quan sát
So sánh từng ACTUAL/EXPECTED path, host và số mismatch. Kiểm tra cả hành vi khi input chứa URL tuyệt đối hoặc đi ra ngoài API prefix.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
40 phút.
