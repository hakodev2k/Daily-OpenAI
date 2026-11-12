# UNIT-COOKIEHDR-001 — Cookie của cổng đặt vé bị mất sau gateway

## Mục tiêu
Điều tra một sai lệch hợp đồng HTTP response trong luồng đăng nhập và lưu lựa chọn ghế; phân biệt kết quả in log với những gì client có thể xử lý.

## Bối cảnh thực tế
Cổng bán vé nhà hát cộng đồng chuyển qua gateway mới. Người dùng đăng nhập thành công nhưng khi quay lại trang chọn ghế, một số client không giữ đủ trạng thái. Backend ghi nhận hai cookie dự kiến.

## Bạn cần làm gì
Chạy adapter mẫu, ghi lại số trường header và giá trị thực tế, đề xuất ít nhất hai giả thuyết, sửa `starter/Program.cs` và chạy kiểm chứng.

## Yêu cầu môi trường
.NET SDK 8.x; PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần Docker, cloud hay credentials.

## Chạy nhanh
`./run.ps1` hoặc `dotnet run --project starter/CookieLab.csproj`.

## Cách reproduce vấn đề
`./reproduce.ps1` chạy bản starter và xác nhận contract hiện tại thất bại; exit 0 nghĩa là đã reproduce thành công. Không chỉnh code trước khi reproduce.

## Những gì cần quan sát
So sánh số lượng trường `Set-Cookie` phát ra, chuỗi giá trị và `COOKIE_CONTRACT`. Phân biệt số cookie mong muốn với số trường HTTP được phát.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix ở starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
[Cảnh báo spoiler — lời giải](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md).

## Estimated Time
35–45 phút.
