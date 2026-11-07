# UNIT-DOTNET-016 — Correlation Context Survives Into Detached Work

## Mục tiêu
Điều tra một .NET service nơi background work đôi khi ghi log với correlation context của request đã kết thúc, rồi sửa mà không làm mất correlation trong request flow hợp lệ.

## Bối cảnh thực tế
Một API nhận request và khởi động một tác vụ phụ mô phỏng việc gửi analytics. Request log đúng correlation id. Tuy nhiên log từ tác vụ phụ xuất hiện sau khi request hoàn tất và vẫn mang context của request cũ. Khi nhiều request chạy liên tiếp, telemetry trở nên khó tin cậy.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis về cách execution context đi qua asynchronous boundaries.
3. Xác định boundary nào nên giữ context và boundary nào không nên giữ.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra code bạn đã sửa.
6. Sau đó mới đọc reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter ở chế độ kiểm tra deterministic và xác nhận rằng detached work quan sát được correlation value vốn thuộc request scope.

## Những gì cần quan sát
- Correlation value bên trong request flow.
- Correlation value trong detached work sau khi request scope logic đã kết thúc.
- Sự khác nhau giữa lifetime của operation business và lifetime của execution context.

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
[Spoiler — chỉ xem sau khi tự điều tra](solution/README.md)

## Expected Results
**Before:** request flow có correlation đúng, nhưng detached work vẫn quan sát correlation của request.

**After:** request flow vẫn giữ correlation; detached work không thừa hưởng request-scoped correlation ngoài boundary đã định.

## Estimated Time
35–50 phút.