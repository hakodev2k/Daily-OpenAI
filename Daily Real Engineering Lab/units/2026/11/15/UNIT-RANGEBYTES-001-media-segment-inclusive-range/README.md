# UNIT-RANGEBYTES-001 — Tải video theo byte range: ghép đoạn bị lặp byte

## Mục tiêu
Điều tra lỗi dữ liệu nhị phân khi client tải nhiều đoạn và ghép lại; phân biệt correctness của payload với HTTP response metadata.

## Bối cảnh thực tế
Cổng lưu trữ video từ camera bảo tồn động vật cho phép tải tiếp các đoạn nhỏ sau khi mạng gián đoạn. File hoàn chỉnh đôi khi khác checksum của bản gốc dù mỗi response đều báo thành công.

## Bạn cần làm gì
Chạy replay, ghi lại độ dài và checksum từng đoạn, đề xuất ít nhất hai giả thuyết, sửa `starter/Program.cs` và chạy verification.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell; không cần Docker, database, NuGet ngoài SDK hoặc cloud.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter chưa sửa, kiểm tra tính toàn vẹn sau khi ghép ba đoạn. Lưu output trước khi thay đổi code.

## Những gì cần quan sát
Độ dài payload, độ dài từng segment, checksum, giá trị Content-Range; thử cả đoạn một byte và đoạn cuối file.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md)

## Expected Results
Trước: ít nhất một case ghép đoạn khác dữ liệu gốc; verify thất bại. Sau: tất cả đoạn ghép đúng thứ tự và đúng byte, boundary cases đều PASS.

## Estimated Time
45 phút.
