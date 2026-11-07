# UNIT-SEGMENT-001 — Khung telemetry phao biển bị lệch dữ liệu

## Mục tiêu
Thực hành điều tra byte-level wire contract, quan sát dữ liệu nguồn và xây dựng regression test trong .NET.

## Bối cảnh thực tế
Gateway nhận frame từ phao đo độ mặn: 2 byte độ dài big-endian rồi đến payload. Khi upstream dùng nguyên buffer, hệ thống hoạt động; khi xử lý một phần buffer, một số frame bị gateway từ chối vì payload không khớp. Không có exception trong encoder.

## Bạn cần làm gì
Chạy starter, thu thập hex output, đưa ra ít nhất hai hypothesis, sửa `starter/TelemetryFrameWriter.cs` mà không thay đổi protocol, rồi chạy verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+ (Windows); không cần Docker, Azure, database hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Trên starter gốc, `./reproduce.ps1` phải trả exit code 0 nếu lỗi đã tái hiện. `./run.ps1` in ra hex frame cho từng fixture. `./verify.ps1` kiểm tra chính mã learner sửa và trả exit code khác 0 khi chưa đạt.

## Những gì cần quan sát
So sánh độ dài frame, header hai byte và payload hex với expected fixture; phân nhóm fixture PASS/FAIL, tìm sự khác biệt trong cách gọi encoder. Ghi nhận khả năng gửi byte không thuộc message.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trên starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) và [mã tham khảo](solution/TelemetryFrameWriter.cs) chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Thành công nghĩa là byte-level contract và không rò dữ liệu ngoài message đều đúng.

## Estimated Time
35–45 phút, L1 / D4.
