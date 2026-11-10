# UNIT-UTF8CHUNK-001 — Bản tin cứu hộ ven biển bị hỏng ký tự khi đọc luồng phân mảnh

## Mục tiêu
Điều tra lỗi tính toàn vẹn văn bản trong một pipeline nhận bản tin UTF-8 qua mạng; chứng minh và sửa lỗi bằng regression test ở nhiều ranh giới dữ liệu.

## Bối cảnh thực tế
Trung tâm cứu hộ nhận bản tin tiếng Việt từ thiết bị radio gateway. Khi tải ít, bản tin chính xác. Khi proxy chia payload thành các đoạn nhỏ, một số dấu tiếng Việt biến thành ký tự thay thế, dù số byte và checksum đầu vào không thay đổi.

## Bạn cần làm gì
Chạy harness, lưu lại bản tin nhận được và bằng chứng byte, đưa ít nhất hai giả thuyết, sửa phần xử lý ở `starter/`, rồi chạy kiểm tra hồi quy.

## Yêu cầu môi trường
.NET SDK 8.x (không cần NuGet bên ngoài), PowerShell 7 hoặc Windows PowerShell.

## Chạy nhanh
```powershell
dotnet run --project starter/RescueDecoder.csproj -- reproduce
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script xác nhận rằng starter tái hiện lỗi và trả exit code 0 khi lỗi được tái hiện đúng.

## Những gì cần quan sát
So sánh payload UTF-8 gốc, chuỗi hiển thị, số ký tự thay thế và kết quả khi thay đổi kích thước phân mảnh. Không dựa vào thời gian chạy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem lời giải sau khi tự điều tra](solution/README.md).

## Expected Results
Trước: chế độ reproduce xác nhận sai khác ở ranh giới chunk. Sau: chế độ verify phải PASS với chunk size 1, 2, 3, 4, 5, 7, 64 và cả dữ liệu không hợp lệ.

## Estimated Time
50–70 phút (L3 / D4).
