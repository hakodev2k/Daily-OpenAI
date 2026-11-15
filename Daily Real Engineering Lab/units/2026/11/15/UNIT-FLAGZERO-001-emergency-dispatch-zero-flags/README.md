# UNIT-FLAGZERO-001 — Quyền xem hồ sơ điều phối cứu hộ

## Mục tiêu
Điều tra sai lệch authorization policy của C# flags enum bằng deterministic fixture và regression verification.

## Bối cảnh thực tế
Sau đợt nâng cấp bảng phân quyền, nhân viên trực ca chưa được phân công nhìn thấy một hồ sơ trong màn hình kiểm soát truy cập. Những nhóm được cấp quyền cụ thể vẫn có vẻ hoạt động bình thường.

## Bạn cần làm gì
Reproduce, lưu evidence, nêu ít nhất hai hypotheses, sửa `starter/Program.cs` mà không mở rộng quyền, rồi verify.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7. Không cần database hay dịch vụ trả phí.

## Chạy nhanh
Tại thư mục unit: `pwsh -File scripts/run.ps1`.

## Cách reproduce vấn đề
`pwsh -File scripts/reproduce.ps1` trả exit 0 khi tái hiện đúng triệu chứng. `pwsh -File scripts/verify.ps1` trả exit khác 0 trên starter chưa sửa.

## Những gì cần quan sát
So sánh ACTUAL và EXPECTED cho bốn ca; ghi lại ca sai, phân biệt sai policy với lỗi hiển thị. Đừng đọc solution trước khi điều tra.

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
Starter có một mismatch; sau sửa cả bốn ca đúng và không mở rộng quyền.

## Estimated Time
35 phút.
