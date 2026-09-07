# UNIT-EF-001 — Giá được sửa trong memory nhưng database không đổi

## Mục tiêu
Điều tra một luồng update bằng EF Core nơi code chạy không exception, object trong memory có giá mới nhưng lần đọc lại vẫn trả về giá cũ.

## Bối cảnh thực tế
Một internal catalog service cho phép operator cập nhật giá sản phẩm. Endpoint báo thành công và log hiển thị giá mới, nhưng request đọc lại ngay sau đó vẫn thấy giá ban đầu.

## Bạn cần làm gì
1. Chạy starter system và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Xác định evidence nào chứng minh update có hoặc không đi qua EF Core persistence boundary.
4. Sửa learner-editable code trong `starter/`.
5. Chạy `verify.ps1`.
6. Sau khi hoàn tất mới đọc reference solution và wrong fixes.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
Chạy starter và quan sát giá trong memory, số entity `Modified`, số row được `SaveChangesAsync` báo thay đổi và giá được query lại từ DbContext mới.

## Những gì cần quan sát
- Có exception hay không.
- Giá trị trong memory sau thao tác update.
- Số entity ở trạng thái `Modified`.
- Số row được `SaveChangesAsync` báo thay đổi.
- Giá trị đọc lại từ persistence boundary mới.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-1.md)
- [Hint 2](hints/hint-2.md)
- [Hint 3](hints/hint-3.md)

## Reference Solution
⚠️ Spoiler: chỉ mở sau khi đã tự verify.
- [Reference solution](solution/README.md)
- [Wrong fixes](solution/wrong-fixes.md)

## Expected Results
Sau khi sửa đúng, `verify.ps1` phải kết thúc với `PASS` và giá persisted là `129`.

## Estimated Time
30–45 phút.
