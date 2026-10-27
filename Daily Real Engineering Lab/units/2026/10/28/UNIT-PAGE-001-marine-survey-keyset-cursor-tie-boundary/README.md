# UNIT-PAGE-001 — Bản ghi khảo sát biển biến mất giữa các trang API

## Mục tiêu
Tự reproduce lỗi mất dữ liệu khi đồng bộ một feed lớn theo trang, phân tích bằng chứng và sửa hợp đồng phân trang mà không thay đổi dữ liệu nguồn. Luyện tư duy về tính đầy đủ, thứ tự ổn định và regression testing.

## Bối cảnh thực tế
Ứng dụng giám sát rạn san hô nhận nhiều quan sát cùng thời điểm từ thiết bị ngoài khơi. API trả từng trang theo thứ tự thời gian; đối tác ghép các trang để lập báo cáo. Khi kích thước trang thay đổi, một số quan sát biến mất dù tất cả request đều thành công.

## Bạn cần làm gì
1. Chạy reproduction gốc và ghi số lượng, thứ tự ID nhận được.
2. Đề xuất ít nhất hai giả thuyết, kiểm tra fixture và đường đi của cursor.
3. Chỉ chỉnh `starter/CursorPager.cs` để mọi bản ghi xuất hiện đúng một lần, theo thứ tự ổn định.
4. Chạy verification trên chính file đã sửa; thử thêm tình huống nhiều bản ghi có cùng thời điểm.
5. Đọc reference solution sau khi tự hoàn thành.

## Yêu cầu môi trường
.NET SDK 8.x (hoặc SDK mới hơn có thể build `net8.0`), PowerShell, không cần Docker, database, cloud hoặc NuGet package bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
`REPRODUCTION_CONFIRMED` xác nhận fixture baseline tái hiện mất bản ghi; đây không phải kiểm tra fix.

## Những gì cần quan sát
- So sánh số lượng ID trong nguồn và trong các trang nhận được.
- Xác định các ID nào bị bỏ qua và có liên hệ gì với ranh giới trang.
- Kiểm tra việc thay đổi `pageSize` có làm thay đổi tính đầy đủ không.
- Ghi lại thứ tự, cursor của từng lần gọi và kiểm tra xem có duplicate không.

Bằng chứng: [incident](evidence/incident.md). Ghi chép: [my-investigation](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [solution/README.md](solution/README.md) và [reference code](solution/CursorPager.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
40 phút · L1 / D3 · Validation: static-review (chưa chạy .NET build/tests).
