# UNIT-STREAMCURSOR-001 — Checkpoint của feed khảo sát bị lệch

## Mục tiêu
Điều tra mất bản ghi khi tiếp tục import theo checkpoint, phân biệt vị trí logic và vị trí I/O.

## Bối cảnh thực tế
Worker .NET nhập dữ liệu đo đạc từ một feed CSV. Khi job bị chia batch để tránh chiếm tài nguyên, dashboard thiếu bản ghi dù không có exception.

## Bạn cần làm gì
Chạy batch 1 rồi batch 2, ghi lại bằng chứng, đề xuất và triển khai sửa lỗi trong `starter/Program.cs`.

## Yêu cầu môi trường
.NET SDK 8; PowerShell; không cần Docker, cloud hoặc NuGet bên ngoài.

## Chạy nhanh
`./reproduce.ps1` (Windows PowerShell: `.\\reproduce.ps1`).

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` và so sánh danh sách kết quả với bốn dòng input trong code.

## Những gì cần quan sát
Số dòng nhận được ở từng batch, giá trị checkpoint, vị trí resume, thứ tự ID và sự khác nhau giữa lượng dữ liệu đầu vào/đầu ra.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem sau khi tự sửa](solution/README.md)

## Expected Results
Starter có thể báo thiếu bản ghi; sau sửa, hai batch trả đủ A001–A004 đúng thứ tự và verify PASS.

## Estimated Time
45 phút.
