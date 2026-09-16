# UNIT-BG-001 — Background worker xử lý đúng lượt đầu nhưng hỏng ở lượt sau

## Mục tiêu
Điều tra một `BackgroundService` có hành vi ổn ở lần xử lý đầu nhưng state và dependency behavior trở nên bất thường ở các vòng xử lý tiếp theo.

## Bối cảnh thực tế
Một worker đồng bộ catalog chạy liên tục trong cùng ASP.NET Core host. Sau deploy, batch đầu hoàn tất, nhưng các batch sau bắt đầu quan sát dữ liệu cũ và số object được giữ sống tăng dần. Restart service làm triệu chứng biến mất tạm thời.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trước khi sửa.
3. Xác định lifecycle boundary phù hợp cho một vòng xử lý background.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra behavior và lifecycle property.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy worker simulation qua nhiều batch và kiểm tra kết quả quan sát được giữa các batch.

## Những gì cần quan sát
- Batch sau có nhìn thấy state mới hay không.
- Một dependency instance tồn tại qua bao nhiêu processing cycle.
- Restart process thay đổi triệu chứng như thế nào.

Không kết luận root cause chỉ từ một metric; hãy liên hệ lifecycle với processing boundary.

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
[Reference Solution](solution/README.md) — spoiler, chỉ mở sau khi đã thử sửa.

## Expected Results
Trước fix, nhiều processing cycle chia sẻ lifecycle không phù hợp với unit-of-work mong muốn. Sau fix, mỗi cycle có boundary độc lập và batch sau quan sát state mới.

## Estimated Time
35–50 phút.