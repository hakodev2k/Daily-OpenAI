# UNIT-CANCELCLASS-001 — Satellite Tracker: Cancellation bị ghi nhận nhầm thành timeout

## Mục tiêu
Điều tra sai lệch giữa tín hiệu vận hành và kết quả xử lý upload, phân biệt cancellation của caller với timeout của downstream mà không phá vỡ hợp đồng gửi dữ liệu.

## Bối cảnh thực tế
Một hệ thống theo dõi động vật hoang dã gửi telemetry qua satellite uplink. Dashboard đột ngột tăng số lượng timeout trong lúc nhân viên dừng các upload không còn cần thiết. Số lần gửi không tăng, nhưng báo cáo lỗi khiến đội vận hành kích hoạt cảnh báo sai.

## Bạn cần làm gì
Chạy ba tình huống, ghi bằng chứng, đề xuất ít nhất hai hypothesis, sửa **starter/Program.cs**, sau đó chạy verify. Không thay đổi bộ oracle của harness (expected status và attempts).

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; không cần Azure, Docker hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script trả về exit code 0 **chỉ khi** phát hiện sai lệch có chủ đích. Nếu script báo lỗi, kiểm tra .NET SDK và log của ba scenario.

## Những gì cần quan sát
So sánh `scenario`, `status`, `expected`, `attempts`; phân biệt tình huống caller chủ động dừng và tình huống uplink không hoàn thành. Ghi nhận loại exception quan sát được và trạng thái token tại thời điểm xử lý.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1` (exit 0 = pass).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results
Trước: có ít nhất một status không khớp expected; reproduction script pass khi thấy sai lệch. Sau: cả ba scenario đúng, mỗi scenario đúng một attempt; verify pass. Nếu không tái hiện, kiểm tra SDK 8 và chạy `dotnet run --project starter/TrackerLab.csproj -- reproduce`.

## Estimated Time
45 phút.
