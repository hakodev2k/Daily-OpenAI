# UNIT-SB-001 — Message Lock Renewal Boundary

## Mục tiêu
Điều tra một worker xử lý message có hành vi xử lý trùng khi công việc kéo dài, thu thập evidence, sửa learner code và chứng minh side effect chỉ xảy ra đúng theo contract mong muốn.

## Bối cảnh thực tế
Một document-conversion worker nhận job từ queue. Phần lớn job hoàn thành nhanh, nhưng file lớn mất lâu hơn. Production ghi nhận một số document được convert hai lần và downstream audit xuất hiện hai completion records dù publisher chỉ gửi một job.

## Bạn cần làm gì
1. Chạy starter và reproduce incident.
2. Ghi timeline, delivery count và side effects vào `workspace/my-investigation.md`.
3. Đưa ra ít nhất 2 hypothesis trước khi sửa.
4. Sửa code trong `starter/` để xử lý đúng lifecycle của message và side effect.
5. Chạy `verify.ps1`.
6. Sau đó mới đọc reference solution.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription; queue được mô phỏng local và deterministic.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy một job có thời gian xử lý dài hơn normal path và kiểm tra số lần side effect được ghi.

## Những gì cần quan sát
- cùng một logical message xuất hiện bao nhiêu delivery
- thời điểm mỗi delivery bắt đầu/kết thúc
- số completion side effects
- processor có còn quyền hoàn tất delivery tại thời điểm kết thúc hay không

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
[Spoiler — chỉ mở sau khi tự điều tra](solution/README.md)

## Expected Results
Trước fix, deterministic scenario tạo nhiều hơn một completion side effect cho một logical job. Sau fix, learner implementation phải giữ completion side effect đúng một lần trong scenario và không báo thành công cho delivery đã mất quyền sở hữu.

## Estimated Time
45–75 phút.