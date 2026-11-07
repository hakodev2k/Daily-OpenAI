# UNIT-SEM-001 — GIS Tile Export: Cancellation Breaks the Worker Limit

## Mục tiêu
Điều tra một sự cố concurrency trong dịch vụ xuất bản đồ: khi request chờ xử lý bị hủy, giới hạn số tác vụ nặng đôi khi không còn được tôn trọng. Thu thập evidence, xác định invariant về capacity và kiểm chứng sửa lỗi bằng kịch bản có điều khiển, không phụ thuộc timing may rủi.

## Bối cảnh thực tế
Dịch vụ GIS của một tỉnh chỉ cho phép một tile renderer hoạt động đồng thời trên mỗi node để tránh cạn bộ nhớ. Sau đợt người dùng hủy hàng loạt export đang chờ, dashboard cho thấy hai tile render đồng thời dù cấu hình `MaxWorkers=1`. CPU, memory và request queue không có dấu hiệu lỗi infrastructure.

## Bạn cần làm gì
1. Reproduce sự cố bằng test harness và ghi lại trình tự A (đang xử lý), B (đang chờ rồi bị hủy), C (đến sau).
2. Đưa ra ít nhất ba hypothesis, phân biệt lỗi scheduler, token, trạng thái worker và capacity control.
3. Chỉnh sửa **chỉ `starter/WorkGate.cs`** để bảo vệ giới hạn concurrency, kể cả đường đi cancellation và exception.
4. Chạy verify trên chính starter đã sửa; bảo đảm work lỗi vẫn không khóa cổng vĩnh viễn.
5. Chỉ mở reference solution sau khi đã thử sửa và viết kết luận trong workspace.

## Yêu cầu môi trường
- .NET SDK 8.0.x (xem `global.json`); PowerShell 7+ hoặc Windows PowerShell.
- Không cần NuGet ngoài .NET SDK, Docker, database hay Azure.

## Chạy nhanh
Tại thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Hoặc:

```bash
dotnet run --project checks/ContractChecks.csproj -- demo
dotnet run --project checks/ContractChecks.csproj -- reproduce
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy **starter hiện tại** và exit 0 khi tái hiện được việc C bắt đầu trước khi A kết thúc, đồng thời cancellation/exception guardrails vẫn hoạt động. Sau khi sửa đúng, reproduce sẽ không còn pass; dùng `./scripts/verify.ps1` để đánh giá trạng thái đã sửa.

## Những gì cần quan sát
- A còn chạy khi C bắt đầu hay không? Không dùng latency ngẫu nhiên để kết luận.
- B nhận cancellation nào? C có chờ đủ điều kiện không?
- Hành vi sau exception từ renderer có khác sau cancellation trong queue?
- Đối chiếu [incident timeline](evidence/incident-log.md) với [investigation guide](docs/investigation-guide.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Chạy `./scripts/verify.ps1` trên code đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler — [solution/README.md](solution/README.md) chỉ để đối chiếu sau khi tự điều tra.

## Expected Results
- Before: reproduce exit 0, verify exit khác 0.
- After: verify exit 0, reproduce exit khác 0; C không vào khi A còn giữ capacity.
- Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
60 phút. Mở rộng production: [Senior challenge](extensions/senior-challenge.md).
