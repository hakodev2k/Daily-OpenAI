# UNIT-CS-016 — Batch Worker Processes the Wrong Slot

## Mục tiêu
Điều tra một batch dispatcher tạo đủ worker nhưng các worker không xử lý đúng work item đã được lên kế hoạch.

## Bối cảnh thực tế
Một media service nhận danh sách file và tạo nhiều `Task` để xử lý song song. Hệ thống ghi nhận đủ số worker, nhưng khi tất cả worker bắt đầu cùng lúc thì batch thất bại trước khi hoàn tất.

## Bạn cần làm gì
Reproduce lỗi, thu thập evidence về giá trị mà từng worker quan sát, đưa ra ít nhất 2 hypotheses, sửa code trong `starter/`, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ để dùng scripts tiện lợi

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
1. Chạy `./reproduce.ps1`.
2. Xác nhận process kết thúc với failure như script mong đợi.
3. Ghi lại số work items, số workers và giá trị slot/index xuất hiện trong exception/output.

## Những gì cần quan sát
- Bao nhiêu worker được tạo trước khi chúng được phép chạy.
- Mỗi worker đọc giá trị slot/index ở thời điểm nào.
- Giá trị đó có nằm trong range của input hay không.
- Failure có thay đổi khi thay đổi số lượng input hay không.

Không giải thích nguyên nhân chỉ từ exception. Hãy ghi evidence và hypotheses trước khi sửa.

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
[Spoiler — chỉ mở sau khi đã verify](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
40 phút.