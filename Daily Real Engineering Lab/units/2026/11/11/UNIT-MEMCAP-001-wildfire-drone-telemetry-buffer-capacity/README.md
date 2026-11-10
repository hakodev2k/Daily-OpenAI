# UNIT-MEMCAP-001 — Drone telemetry: frame dài hơn payload

## Mục tiêu
Điều tra sai lệch giữa số byte được ghi và số byte được gửi qua mạng, dựa trên bằng chứng wire-level; sửa lỗi và bảo toàn hợp đồng frame nhị phân.

## Bối cảnh thực tế
Đội cứu hỏa dùng drone truyền khung telemetry nhiệt độ tới trạm chỉ huy. Gói ngắn 5 byte được gateway ghi nhận thành 256 byte. Parser loại bỏ gói vì độ dài không khớp header. Chỉ một số loại gói gặp lỗi.

## Bạn cần làm gì
1. Chạy chương trình và lưu lại số byte, hex dump, kết quả parser.
2. Đưa ra ít nhất hai giả thuyết trước khi đọc hints.
3. Sửa `starter/Program.cs` sao cho đúng wire contract, kể cả payload rỗng và payload gần ngưỡng buffer.
4. Chạy verify để kiểm tra code bạn sửa.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; không cần dịch vụ ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet run --project starter/TelemetryLab.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy starter với chế độ `reproduce` và kỳ vọng phát hiện sai lệch. Exit code 0 của script nghĩa là **đã tái hiện được lỗi**, không phải đã sửa.

## Những gì cần quan sát
So sánh `payloadLength`, `frameLength`, `wireLength`, vị trí các byte 00 và thông báo từ receiver. Lưu output trước và sau khi sửa. Không suy luận từ mỗi exit code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
Trước: receiver báo frame không hợp lệ cho payload ngắn. Sau: ba kích thước payload 0, 5, 200 byte đều hợp lệ, header và nội dung chính xác, không có byte thừa.

## Estimated Time
45 phút (L2 / D4).
