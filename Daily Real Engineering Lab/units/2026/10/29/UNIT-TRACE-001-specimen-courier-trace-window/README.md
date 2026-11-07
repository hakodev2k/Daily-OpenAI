# UNIT-TRACE-001 — Trace điều phối mẫu xét nghiệm kết thúc trước khi chuyển phát hoàn tất

## Mục tiêu
Đọc timeline `System.Diagnostics.Activity` và xác minh telemetry có đo đủ vòng đời một tác vụ bất đồng bộ hay không.

## Bối cảnh thực tế
Dịch vụ điều phối mẫu xét nghiệm gửi yêu cầu tới courier. API trả `accepted`, nhưng dashboard cho thấy span `dispatch` có thời gian rất ngắn, dù gateway vẫn đang xử lý. Đội vận hành không tin được latency end-to-end.

Dữ liệu hoàn toàn synthetic; chạy local, không cần Azure, Docker hoặc collector.

## Bạn cần làm gì
Chạy baseline, ghi lại thứ tự START/STOP của hai span, nêu ít nhất hai giả thuyết rồi sửa **chỉ** `starter/DispatchService.cs`. Không sửa probe để ép PASS.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1+ hoặc 7+, không cần NuGet ngoài.

## Chạy nhanh
```powershell
.\setup.ps1
.\run.ps1
.\reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` xác nhận triệu chứng gốc qua một gateway có gate điều khiển thứ tự thực thi. PASS ở đây nghĩa là lỗi đã được tái hiện, **không** phải đã sửa.

## Những gì cần quan sát
- Thứ tự START/STOP của `dispatch` và `gateway-send`.
- Quan hệ `TraceId` và `ParentSpanId`.
- Vì sao kết quả `accepted` chưa đủ để kết luận telemetry đúng?
- Span cần mô tả khoảng công việc nào?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Chỉ sửa `starter/DispatchService.cs`; giữ nguyên probe và gateway.
4. Chạy `.\verify.ps1` trên code đã sửa, kiểm tra cả thứ tự và parent-child.
5. Chỉ sau đó xem reference.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự thử:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
35 phút — L1 / D3.
