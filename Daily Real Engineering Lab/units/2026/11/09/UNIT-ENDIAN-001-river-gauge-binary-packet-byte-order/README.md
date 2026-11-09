# UNIT-ENDIAN-001 — Giải mã telemetry trạm đo sông

## Mục tiêu
Điều tra sai lệch dữ liệu khi dịch vụ .NET đọc gói nhị phân 8 byte; phân biệt quan sát, giả thuyết và kết luận bằng fixture có thể lặp lại.

## Bối cảnh thực tế
Một gateway nhận bản tin từ trạm quan trắc mực nước. Station ID và sample sequence có vẻ hợp lệ nhưng nhiệt độ đo được đôi khi vô lý. Hệ thống không được tự động loại bỏ frame hợp lệ chỉ vì một trường số có vẻ bất thường.

## Bạn cần làm gì
Chạy fixture, ghi lại raw bytes và kết quả thực tế; đưa ra ít nhất hai giả thuyết, kiểm tra từng giả thuyết; sửa decoder trong `starter/RiverFrameDecoder.cs`; verify cả giá trị âm, ranh giới số và độ dài frame.

## Yêu cầu môi trường
.NET SDK 8.x; PowerShell. Không cần cloud, database, mạng hoặc NuGet package bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```
`reproduce.ps1` kiểm tra triệu chứng trên bản starter nguyên gốc; `verify.ps1` chạy mã learner chỉnh sửa và sẽ FAIL trước khi sửa.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trước khi chỉnh code. So sánh output cho normal, negative và boundary; kiểm tra station/sequence có bị sai theo không.

## Những gì cần quan sát
Ghi lại bytes ở offsets 0–7, số đo nhiệt độ được decoder trả về, kết quả mong đợi và kiểm tra độ dài. Không sửa các fixture để làm test xanh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và phân tích](solution/README.md)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
40 phút.
