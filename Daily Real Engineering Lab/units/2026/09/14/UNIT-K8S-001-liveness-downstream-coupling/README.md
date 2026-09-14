# UNIT-K8S-001 — Probe báo lỗi và workload bị restart dây chuyền

## Mục tiêu

Điều tra một production incident trong đó workload vẫn chạy được nhưng một outage ngắn ở dependency làm orchestrator liên tục coi instance là không còn khỏe, dẫn tới restart và làm giảm capacity toàn hệ thống.

## Bối cảnh thực tế

Một ASP.NET Core service chạy sau Kubernetes. Khi downstream catalog tạm thời unavailable trong khoảng ngắn, nhiều pod đồng loạt bị đánh dấu unhealthy. Trong incident thật, restart count tăng nhanh trong khi process không crash, CPU/memory bình thường và service có thể phục hồi ngay khi dependency trở lại.

Starter mô phỏng health endpoints và một downstream dependency có thể bật/tắt trạng thái để bạn quan sát contract mà orchestrator sẽ nhìn thấy.

## Bạn cần làm gì

- Chạy starter và reproduce trạng thái dependency outage.
- Ghi lại status code của các probe endpoints trước và trong outage.
- Đưa ra hypothesis tại sao một dependency outage có thể biến thành restart storm.
- Sửa starter sao cho probe contract phản ánh đúng mục đích vận hành của từng probe.
- Chạy `verify.ps1` để xác nhận hành vi sau fix.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-K8S-001-liveness-downstream-coupling"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ:

1. chạy starter app local
2. kiểm tra hai health endpoints khi dependency bình thường
3. chuyển fake dependency sang trạng thái unavailable
4. kiểm tra lại hai endpoints
5. in ra bằng chứng mà một probe controller sẽ quan sát

## Những gì cần quan sát

- Process starter vẫn tồn tại trong toàn bộ quá trình.
- Business dependency có thể chuyển sang unavailable mà app không crash.
- Một hoặc nhiều health endpoints đổi sang HTTP 503 trong outage.
- Hãy xác định status nào nên làm workload bị loại khỏi traffic và status nào thực sự nên dẫn tới restart process.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- Dependency outage làm contract probe hiện tại có thể khiến một process vẫn sống bị xem là cần restart.

**After**

- Outage của dependency vẫn được phản ánh để instance không nhận traffic mới.
- Process-health signal vẫn ổn định khi chính process còn hoạt động bình thường.
- Khi dependency phục hồi, instance có thể quay lại phục vụ mà không cần restart để chữa một lỗi bên ngoài process.

## Estimated Time

40–60 phút.
