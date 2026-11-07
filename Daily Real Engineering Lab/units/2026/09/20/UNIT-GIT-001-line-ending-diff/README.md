# UNIT-GIT-001 — Invisible Line-Ending Diff

## Mục tiêu
Điều tra một thay đổi source rất nhỏ nhưng Git review lại hiển thị gần như toàn bộ file bị sửa.

## Bối cảnh thực tế
Một team .NET làm việc trên Windows và Linux. Developer chỉ sửa một giá trị trong file cấu hình, nhưng PR xuất hiện hàng chục dòng thay đổi, làm code review khó đọc và tăng nguy cơ bỏ sót thay đổi thật.

## Bạn cần làm gì
Reproduce hiện tượng trong repository lab, thu evidence từ Git, ghi hypothesis, sửa policy của repository và verify.

## Yêu cầu môi trường
- Git 2.40+
- PowerShell 7+

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script tạo một worktree lab tạm thời và mô phỏng thay đổi từ môi trường khác.

## Những gì cần quan sát
- `git diff --stat`
- số dòng Git xem là đã thay đổi
- nội dung text có thực sự thay đổi tương ứng với kích thước diff hay không
- các thuộc tính Git đang áp dụng cho file

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/.gitattributes`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Before: một semantic edit nhỏ tạo diff lớn bất thường. After: repository có policy rõ ràng và cùng loại edit chỉ tạo diff tập trung vào dòng thực sự thay đổi.

Nếu không reproduce được, chạy `git --version`, kiểm tra PowerShell 7+ và bảo đảm thư mục lab có quyền ghi file tạm.

## Estimated Time
30 phút.