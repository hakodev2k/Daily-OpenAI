# UNIT-CS-015 — Mutable Dictionary Key

## Mục tiêu
Điều tra một lỗi lookup khó hiểu trong code C# dùng hash-based collection, thu thập evidence, đưa ra hypothesis, sửa đúng invariant và verify regression.

## Bối cảnh thực tế
Một service tính promotion giữ trạng thái rule đã evaluate trong bộ nhớ. Rule được tìm thấy ngay sau khi nạp, nhưng sau một bước cập nhật trạng thái hợp lệ trong cùng workflow, lookup có thể trả về `false` dù object vẫn xuất hiện khi enumerate collection.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi evidence và ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Sửa code trong `starter/` mà không thay collection bằng linear scan.
4. Chạy verify để chứng minh lookup ổn định trước và sau business update.
5. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.0.x.
- PowerShell 7+ hoặc Windows PowerShell.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script build và chạy starter ở chế độ reproduction, sau đó yêu cầu symptom dự kiến phải xuất hiện.

## Những gì cần quan sát
- Kết quả lookup trước và sau bước cập nhật rule.
- Số phần tử khi enumerate collection.
- Các giá trị identity/equality diagnostics starter in ra.
- Có hay không sự khác nhau giữa membership lookup và enumeration.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ mở sau khi tự điều tra](solution/README.md)

## Expected Results
Trước khi sửa, reproduction phải chứng minh một rule vẫn được enumerate nhưng lookup không còn tìm thấy nó sau business update. Sau khi sửa, `./verify.ps1` phải pass và lookup phải ổn định qua cùng workflow.

## Estimated Time
45 phút.