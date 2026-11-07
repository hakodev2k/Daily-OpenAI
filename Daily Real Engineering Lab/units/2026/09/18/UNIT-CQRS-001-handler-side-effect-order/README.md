# UNIT-CQRS-001 — Command Side Effect Before Durable State

## Mục tiêu
Điều tra một command handler có hai tác động ra bên ngoài và tạo trạng thái mâu thuẫn khi một bước thất bại.

## Bối cảnh thực tế
Employee access workflow ghi nhận yêu cầu rồi gửi notification. Support nhận ticket: người dùng đã nhận thông báo xác nhận nhưng portal không có access request tương ứng. Happy path luôn hoạt động.

## Bạn cần làm gì
Chạy reproduction, thu thập event timeline, viết hypotheses, sửa learner-editable handler trong `starter/`, rồi chạy verification bao gồm happy path và failure path.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell. Lab dùng deterministic in-memory collaborators, không cần database hay message service.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Thứ tự các event trong timeline.
- Trạng thái persisted khi repository mô phỏng failure.
- Notification nào đã được phát trước khi command kết thúc.
- Sự khác nhau giữa happy path và failure path.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: failure path để lại notification không tương ứng với durable state.

After: failure path không công bố success sai; happy path vẫn lưu request và phát đúng một notification.

## Estimated Time
50 phút.