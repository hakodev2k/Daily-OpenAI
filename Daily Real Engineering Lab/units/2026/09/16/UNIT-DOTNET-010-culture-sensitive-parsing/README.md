# UNIT-DOTNET-010 — Culture-sensitive parsing at an integration boundary

## Mục tiêu
Điều tra một lỗi dữ liệu phụ thuộc runtime culture trong .NET và làm rõ contract tại integration boundary.

## Bối cảnh thực tế
Một scheduled import nhận file giá từ đối tác. Cùng một file chạy đúng trên máy developer nhưng cho kết quả khác trên một host có regional settings khác. Không có exception rõ ràng; một số giá trị vẫn parse thành công nhưng mang giá trị sai.

## Bạn cần làm gì
1. Chạy starter với hai culture được cung cấp.
2. So sánh output và xác định dữ liệu nào không ổn định.
3. Ghi hypothesis trước khi sửa.
4. Sửa `starter/` để parsing tuân theo contract của file thay vì ambient machine settings.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy cùng một input dưới hai runtime culture và in normalized result để bạn đối chiếu.

## Những gì cần quan sát
- Cùng một input có tạo cùng domain value hay không.
- Trường hợp nào fail rõ ràng và trường hợp nào nguy hiểm hơn vì vẫn parse thành công.
- Machine culture có phải là một phần hợp lệ của file contract hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints.md` theo thứ tự.

## Reference Solution
`solution/README.md` — spoiler.

## Expected Results
Trước fix, kết quả phụ thuộc culture của process. Sau fix, cùng input tạo cùng kết quả trên các culture được kiểm tra và invalid input vẫn bị từ chối.

## Estimated Time
30–45 phút.