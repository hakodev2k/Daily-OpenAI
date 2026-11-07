# UNIT-PLAY-001 — Flaky UI Test After DOM Replacement

## Mục tiêu
Điều tra một E2E test cho admin dashboard chạy ổn phần lớn thời gian nhưng thỉnh thoảng thất bại khi thao tác ngay sau khi dữ liệu được refresh.

## Bối cảnh thực tế
Team có một Playwright-like test kiểm tra thao tác Approve order. CI ghi nhận failure không đều: UI vẫn hiển thị đúng order, nhưng action đôi lúc không tác động được vào row mà test vừa quan sát.

## Bạn cần làm gì
1. Chạy starter và reproduce failure deterministic.
2. Ghi lại timeline giữa việc tìm target, refresh UI và action.
3. Đưa ra ít nhất hai hypothesis.
4. Sửa learner-editable code trong `starter/` mà không thêm arbitrary sleep.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần browser thật; lab dùng deterministic simulator mô phỏng lifecycle của DOM element và locator semantics.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Thời điểm target được resolve.
- Thời điểm UI refresh hoàn tất.
- Identity/version của node trước và sau refresh.
- Action đang nhắm tới target nào.

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
Before: deterministic reproduction cho thấy action thất bại sau một UI refresh dù business row vẫn tồn tại.

After: verification pass qua nhiều refresh cycle mà không dùng fixed delay và vẫn thao tác đúng order.

Nếu không reproduce được, xác nhận đang chạy `reproduce.ps1` từ root của unit và .NET 8 SDK khả dụng.

## Estimated Time
45 phút.