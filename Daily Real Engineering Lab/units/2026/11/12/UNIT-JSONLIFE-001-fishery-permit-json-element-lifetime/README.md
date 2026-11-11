# UNIT-JSONLIFE-001 — Giấy phép thủy sản lỗi khi đối soát JSON

## Mục tiêu
Tự reproduce, thu thập exception, tìm nguyên nhân và sửa lỗi vòng đời dữ liệu trong pipeline .NET.

## Bối cảnh thực tế
Cổng đối tác gửi giấy phép dạng JSON. Parser ghi nhận thành công, nhưng bước đối soát sau đó không đọc được dữ liệu đã nhận.

## Bạn cần làm gì
Chạy starter, ghi lại call stack và hai giả thuyết; sửa `starter/Program.cs` để hai giấy phép được đối soát đúng.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell. Không cần dịch vụ ngoài hay NuGet package.

## Chạy nhanh
```powershell
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chỉ thành công khi tái hiện lỗi ban đầu. Sau khi sửa, dùng `verify.ps1` để kiểm tra starter đã chỉnh sửa.

## Những gì cần quan sát
Thời điểm parse, thời điểm đối soát, exception type, stack trace và quyền sở hữu dữ liệu giữa hai bước.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md)

## Expected Results
Trước sửa: reproduce exit 0, verify exit 2. Sau sửa: verify exit 0, `PASS: 2 permits`.

## Estimated Time
45 phút.
