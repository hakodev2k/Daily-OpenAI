# UNIT-CS-013 — String identity contract in a lookup

## Mục tiêu
Điều tra một lỗi C# trong đó cùng một business identifier cho kết quả lookup khác nhau tùy cách viết hoa/thường.

## Bối cảnh thực tế
Một service import mapping từ external system vào dictionary. Dữ liệu upstream xem identifier là case-insensitive, nhưng một số record hợp lệ không tìm thấy mapping sau khi được nhập từ nguồn khác.

## Bạn cần làm gì
Reproduce vấn đề, xác định contract identity của identifier, sửa `starter/` để lookup tuân thủ contract đó và verify cả positive lẫn negative cases.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy starter với các identifier tương đương theo business contract nhưng khác casing.

## Những gì cần quan sát
- Record nào lookup thành công/thất bại.
- Việc biến đổi casing trước lookup có thực sự là một identity policy ổn định hay không.
- Sau fix, identifier không liên quan vẫn phải bị từ chối.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem thư mục `hints/` theo thứ tự.

## Reference Solution
`solution/README.md` — spoiler.

## Expected Results
Trước fix có ít nhất một false miss. Sau fix, các casing hợp lệ resolve cùng mapping nhưng identifier khác không được match.

## Estimated Time
25–40 phút.