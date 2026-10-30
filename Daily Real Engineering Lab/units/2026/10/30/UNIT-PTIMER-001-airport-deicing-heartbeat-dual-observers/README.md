# UNIT-PTIMER-001 — Hai màn hình giám sát mất nhịp heartbeat của xe chống băng

## Mục tiêu
Tái hiện một lỗi khi hai observer cùng theo dõi nhịp trạng thái, thu thập bằng chứng, đưa ra giả thuyết, sửa và kiểm chứng hợp đồng concurrent/cancellation.

## Bối cảnh thực tế
Tại sân bay, màn hình điều phối và màn hình an toàn phải thấy cùng chuỗi heartbeat ID của đội xe chống băng. Một màn hình chạy bình thường khi kiểm thử riêng; khi cả hai kết nối thì một màn hình báo lỗi. Hủy giám sát ở một màn hình không được dừng màn hình còn lại.

## Bạn cần làm gì
1. Reproduce hai contract lỗi, ghi exception và thời điểm xuất hiện.
2. Viết ít nhất hai hypothesis, kiểm tra trước khi xem hints.
3. Thiết kế phân phối cùng heartbeat ID tới nhiều observer.
4. Chỉ sửa `starter/HeartbeatCoordinator.cs`, giữ nguyên public API.
5. Verify, giải thích hậu quả nếu observer xử lý quá chậm.

## Yêu cầu môi trường
.NET 8 SDK/runtime (hoặc SDK mới hơn hỗ trợ build `net8.0` và .NET 8 runtime), PowerShell; không cần database, Docker, Azure hoặc package ngoài SDK.

## Chạy nhanh
```powershell
cd "Daily Real Engineering Lab/units/2026/10/30/UNIT-PTIMER-001-airport-deicing-heartbeat-dual-observers"
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy starter nguyên bản. Exit code 0 ở bước này nghĩa là **đã tái hiện đúng hai failure**, không phải ứng dụng đã đúng. Sau khi sửa dùng `./verify.ps1`.

## Những gì cần quan sát
Exception của từng observer, thời điểm đăng ký, chuỗi heartbeat ID, tính độc lập của cancellation. Xem [incident evidence](evidence/incident-log.md) và ghi nhận tại [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự sửa:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` chạy code bạn sửa trong `starter/`, không chạy reference.

## Estimated Time
40 phút.

