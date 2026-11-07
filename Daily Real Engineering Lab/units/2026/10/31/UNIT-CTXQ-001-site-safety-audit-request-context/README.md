# UNIT-CTXQ-001 — Audit an toàn thiết bị: request thành công, worker mất thông tin tenant

## Mục tiêu
Tái hiện lỗi tại ranh giới giữa ASP.NET Core request và công việc xử lý trễ; đối chiếu bằng chứng ở thời điểm nhận request và thời điểm worker thực thi. Phân biệt lỗi xác thực, dữ liệu tenant, và vòng đời xử lý.

## Bối cảnh thực tế
Cổng cấp phép thiết bị an toàn tại công trường nhận yêu cầu kiểm tra từ nhiều nhà thầu. API trả trạng thái accepted và ghi một công việc audit để worker xử lý sau. Dashboard request báo thành công, nhưng một số bản ghi audit không có tenant. Một đợt tăng worker không cải thiện tình hình.

## Bạn cần làm gì
1. Chạy contract trên hai request thuộc tenant khác nhau và ghi lại trạng thái trước/sau khi drain queue.
2. Viết ít nhất ba giả thuyết; phân biệt lỗi ở request ingress, queue và worker.
3. Chỉ sửa `starter/AuditDispatchService.cs`. Không sửa contract hoặc biến mất lỗi bằng cách bỏ audit.
4. Verify tenant A/B không bị lẫn, thiếu tenant bị từ chối sớm và queue không giữ công việc không hợp lệ.
5. Sau khi tự sửa, so sánh reference solution và trade-offs.

## Yêu cầu môi trường
- .NET SDK 8, PowerShell 5.1+ hoặc PowerShell 7.
- Dùng ASP.NET Core shared framework có sẵn trong SDK; không cần NuGet bên ngoài, Docker, Azure hoặc database.

## Chạy nhanh
```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy trên starter gốc; exit **0** nghĩa là đã tái hiện triệu chứng dự kiến. Sau khi sửa `starter/AuditDispatchService.cs`, chạy `verify.ps1`; exit **0** nghĩa là tất cả invariant được kiểm tra trên chính starter đã sửa.

## Những gì cần quan sát
- Request có claim tenant hợp lệ trước khi trả accepted không?
- Công việc audit được tạo và thực thi ở thời điểm nào?
- Kết quả khi hai tenant gửi request liên tiếp khác gì khi một request gửi đơn lẻ?
- Lỗi được phát hiện ở ingress hay chỉ khi worker drain queue?

Dùng [evidence pack](evidence/incident-log.md) và [investigation workspace](workspace/my-investigation.md) để lưu giả thuyết.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi đã tự điều tra.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
60–75 phút · L3 / D4.
