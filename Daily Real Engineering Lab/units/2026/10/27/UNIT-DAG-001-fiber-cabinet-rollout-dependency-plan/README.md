# UNIT-DAG-001 — Kế hoạch rollout firmware báo thành công dù còn bước chưa chạy

## Mục tiêu

Điều tra một incident của dịch vụ điều phối firmware cho tủ cáp quang, tái hiện báo cáo thành công không tương ứng với số bước triển khai, đưa ra bản sửa và kiểm chứng bằng các fixture độc lập.

## Bối cảnh thực tế

Hệ thống NOC của một nhà mạng tạo kế hoạch triển khai theo thứ tự công việc: tải firmware, kiểm tra chữ ký, sao lưu cấu hình, cập nhật thiết bị, kiểm tra sức khỏe, thông báo hoàn tất. Sau một lần chỉnh sửa cấu hình, dashboard hiển thị **SUCCESS** nhưng nhiều tủ vẫn chạy firmware cũ. Không có exception trong log.

Bạn nhận một bản thu nhỏ của planner. Không cần kết nối thiết bị thật; toàn bộ dữ liệu là synthetic.

## Bạn cần làm gì

1. Chạy baseline và ghi nhận số bước được lên kế hoạch so với số bước được khai báo.
2. Đọc [incident log](evidence/incident-log.md), ghi ít nhất hai giả thuyết và bằng chứng phân biệt.
3. Chỉnh **chỉ** `starter/Planner.cs` để planner không báo thành công khi kế hoạch không hợp lệ hoặc không đầy đủ.
4. Giữ đúng thứ tự prerequisite; trả diagnostic có thể hành động; không thực thi một phần rollout lỗi.
5. Chạy verification trên starter đã sửa, sau đó mới đối chiếu reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0 hoặc mới hơn có khả năng build `net8.0`; không cần NuGet package bên thứ ba, database, Docker hoặc Azure.
- PowerShell 7 hoặc Windows PowerShell để chạy script; cũng có thể chạy trực tiếp `dotnet`.
- `global.json` xác định baseline SDK và chính sách roll-forward.

## Chạy nhanh

Tại thư mục unit:

```powershell
./reproduce.ps1
./run.ps1
./verify.ps1
```

Nếu không dùng PowerShell:

```bash
dotnet run --project baseline/Baseline.csproj -- reproduce
dotnet run --project starter/Starter.csproj -- demo
dotnet run --project starter/Starter.csproj -- verify
```

## Cách reproduce vấn đề

`reproduce.ps1` luôn chạy bản baseline **bất biến**, ngay cả sau khi bạn chỉnh starter. Script phải in `REPRODUCED` trên cấu hình incident. `run.ps1` chạy workspace hiện tại để bạn quan sát tiến triển.

## Những gì cần quan sát

- Có bao nhiêu bước trong cấu hình? Bao nhiêu bước thực sự xuất hiện trong kế hoạch?
- Dashboard giả lập báo trạng thái gì? Có exception không?
- Có khác biệt nào giữa cấu hình hợp lệ và cấu hình incident?
- Bước nào chưa xuất hiện, và cần thêm bằng chứng nào trước khi rollout thực?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra:** [Reference Solution](solution/README.md).

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md). Verification yêu cầu tính đúng đắn, tính đầy đủ, thông báo lỗi rõ ràng và thứ tự ổn định.

## Estimated Time

70 phút: 15 phút reproduce/evidence, 20 phút hypothesis và sửa, 20 phút verify, 15 phút đối chiếu và trade-offs.
