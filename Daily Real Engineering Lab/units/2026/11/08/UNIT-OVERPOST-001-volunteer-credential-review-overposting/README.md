# UNIT-OVERPOST-001 — Hồ sơ tình nguyện viên tự thay đổi trạng thái xét duyệt

## Mục tiêu
Điều tra ranh giới tin cậy của API cập nhật hồ sơ và viết regression test cho hành vi không được phép.

## Bối cảnh thực tế
Cổng tình nguyện viên thành phố cho phép sửa tên và số điện thoại. Trạng thái xét duyệt chỉ do nhân viên phụ trách quản lý. Một số hồ sơ xuất hiện trạng thái đã duyệt ngay sau thao tác tự cập nhật.

## Bạn cần làm gì
Reproduce, thu thập hai output, ghi ít nhất hai giả thuyết, điều tra đường đi dữ liệu và sửa mã trong `starter/Program.cs`.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần database, mạng hoặc credential.

## Chạy nhanh
Từ thư mục unit: `pwsh -File ./scripts/setup.ps1` rồi `pwsh -File ./scripts/run.ps1`. Có thể thay `pwsh` bằng `powershell` trên Windows.

## Cách reproduce vấn đề
`pwsh -File ./scripts/reproduce.ps1`. Exit 0 nghĩa là đã tái hiện đúng triệu chứng; không có nghĩa code đúng.

## Những gì cần quan sát
So sánh hai dòng `ordinary`, `adversarial` với trạng thái gốc `Pending`. Đánh dấu trường client được phép thay đổi và trường chỉ thuộc server.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `pwsh -File ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước sửa: cập nhật thông thường thành công nhưng payload bất thường thay đổi `ReviewStatus`. Sau sửa: cả hai cập nhật giữ `Pending` và vẫn sửa được tên, số điện thoại; verify exit 0.

## Estimated Time
45 phút.
