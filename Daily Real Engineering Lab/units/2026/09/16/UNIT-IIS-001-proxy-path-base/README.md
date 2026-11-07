# UNIT-IIS-001 — Application path changes behind IIS

## Mục tiêu
Điều tra một deployment behavior khi ASP.NET Core application được publish dưới một application path thay vì website root.

## Bối cảnh thực tế
Một internal portal chạy đúng khi developer truy cập trực tiếp ở local. Sau khi deploy dưới một path con trên IIS, trang HTML vẫn mở nhưng một số URL do application tạo ra trỏ sai vị trí và trả 404.

## Bạn cần làm gì
1. Chạy starter simulation.
2. So sánh URL được tạo ở root hosting và sub-path hosting.
3. Ghi hypothesis về request path components.
4. Sửa starter để URL generation tôn trọng hosting boundary.
5. Chạy verification rồi mới đọc solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy deterministic URL-generation cases cho root path và application sub-path.

## Những gì cần quan sát
- Case nào tạo URL sai?
- Phần nào của request context khác giữa hai hosting topology?
- Fix có giữ đúng behavior khi chạy ở root không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints/` theo thứ tự.

## Reference Solution
`solution/README.md` — spoiler.

## Expected Results
Before: sub-path case tạo URL thiếu application prefix. After: cả root và sub-path cases đều tạo URL đúng contract.

## Estimated Time
30–45 phút.