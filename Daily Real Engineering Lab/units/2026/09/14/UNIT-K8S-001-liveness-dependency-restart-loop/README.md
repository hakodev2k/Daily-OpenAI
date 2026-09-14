# UNIT-K8S-001 — Dependency outage biến thành restart loop vì health probe

## Mục tiêu

Điều tra một production incident nơi ứng dụng vẫn có thể chạy nhưng một dependency tạm thời unavailable làm deployment liên tục bị restart, khiến recovery chậm hơn và traffic dao động mạnh.

## Bối cảnh thực tế

Một Inventory API chạy trên Kubernetes. Khi downstream catalog service gặp sự cố ngắn, pod của Inventory API bắt đầu restart liên tục. CPU và memory bình thường, process không crash vì unhandled exception, nhưng số restart tăng nhanh và service mất khả năng phục vụ ổn định.

Starter cung cấp một ASP.NET Core service nhỏ và script mô phỏng cách orchestrator phản ứng với health endpoint. Không cần cluster Kubernetes để reproduce cơ chế cốt lõi.

## Bạn cần làm gì

- Reproduce restart amplification.
- Thu thập evidence từ HTTP status và restart counter.
- Ghi ít nhất 2 hypotheses trước khi sửa.
- Xác định contract phù hợp cho process health và traffic eligibility.
- Sửa `starter/Program.cs`.
- Chạy `verify.ps1` để xác nhận behavior sau fix.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-K8S-001-liveness-dependency-restart-loop"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ:

1. khởi động starter service,
2. chuyển fake dependency sang trạng thái unavailable,
3. kiểm tra endpoint health mà orchestrator dùng để quyết định restart,
4. mô phỏng nhiều vòng restart khi endpoint tiếp tục báo unhealthy.

## Những gì cần quan sát

- Process có thể khởi động bình thường.
- Khi fake dependency unavailable, health endpoint dùng cho restart trả về non-success status.
- Mỗi lần process được khởi động lại, dependency vẫn unavailable nên cùng điều kiện lặp lại.
- Restart không làm dependency bên ngoài hồi phục.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- Dependency unavailable làm endpoint dùng cho restart trả về failure.
- Simulator ghi nhận nhiều restart liên tiếp.

**After**

- Process-health endpoint vẫn success khi process còn khỏe.
- Traffic-readiness endpoint phản ánh dependency unavailable.
- Simulator không còn restart process chỉ vì dependency bên ngoài tạm thời down.

Nếu không reproduce được, kiểm tra port `5088` có đang bị process khác sử dụng không.

## Estimated Time

45–60 phút.
