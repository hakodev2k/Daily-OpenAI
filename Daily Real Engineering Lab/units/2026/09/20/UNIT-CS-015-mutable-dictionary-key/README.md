# UNIT-CS-015 — Mutable Dictionary Key Breaks Lookup

## Mục tiêu
Điều tra một lỗi lookup khó hiểu trong C# collection bằng runtime evidence thay vì đoán từ output.

## Bối cảnh thực tế
Pricing service giữ rule đã compile trong memory để tránh tính lại. Sau khi một request chuẩn hóa key trước bước lookup thứ hai, service báo cache miss dù entry vừa được thêm và `Count` vẫn bằng 1.

## Bạn cần làm gì
Chạy starter, ghi lại triệu chứng, đưa ra ít nhất hai hypothesis, sửa code để key lookup có contract ổn định, rồi chạy verification.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script chỉ PASS khi starter còn biểu hiện failure được thiết kế cho lab.

## Những gì cần quan sát
- `Dictionary.Count` trước và sau thay đổi state.
- Kết quả `ContainsKey` trước và sau thay đổi.
- Giá trị equality/hash evidence được starter in ra.
- Sự khác nhau giữa object đang tồn tại trong collection và khả năng tìm lại object đó.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Sau fix, lookup trước và sau bước chuẩn hóa phải ổn định, `Count` không tăng ngoài ý muốn và verification phải PASS.

## Estimated Time
30 phút