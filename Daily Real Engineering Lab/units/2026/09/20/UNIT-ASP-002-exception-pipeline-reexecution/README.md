# UNIT-ASP-002 — Exception Pipeline Re-executes Request Work

## Mục tiêu
Điều tra hành vi error path trong ASP.NET Core request pipeline và sửa lỗi khiến một request thất bại tạo nhiều side effect hơn mong đợi.

## Bối cảnh thực tế
Một Customer Support API ghi audit cho mỗi request. Endpoint `/tickets/fail` cố ý mô phỏng lỗi nghiệp vụ. Monitoring cho thấy client chỉ gửi một request nhưng audit sink đôi lúc ghi hai entry có cùng request identifier.

## Bạn cần làm gì
1. Chạy starter và reproduce hiện tượng.
2. Quan sát request path, response và audit output.
3. Ghi ít nhất hai hypothesis trước khi sửa.
4. Sửa `starter/` để mỗi logical request chỉ tạo đúng một audit entry, kể cả khi đi qua error handling path.
5. Chạy `verify.ps1` để kiểm tra success path và failure path.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` build và chạy một deterministic in-process scenario. Script phải xác nhận rằng một failed logical request tạo số audit entry không đúng contract.

## Những gì cần quan sát
- Có bao nhiêu logical request được gửi.
- Endpoint nào được thực thi trong normal path và error path.
- Audit sink nhận bao nhiêu entry cho cùng request identifier.
- Success path có hành vi khác failure path hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: failure path tạo hai audit entry cho một request identifier; success path tạo một entry.

After: cả success và failure path đều tạo đúng một audit entry cho mỗi logical request, trong khi failure vẫn được chuyển thành response 500 có kiểm soát.

## Estimated Time
45 phút.
