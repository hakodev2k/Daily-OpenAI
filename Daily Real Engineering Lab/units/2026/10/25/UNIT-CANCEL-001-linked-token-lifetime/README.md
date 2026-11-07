# UNIT-CANCEL-001 — Cancellation Bị Mất Khi Scanner Vẫn Đang Chạy

## Mục tiêu
Điều tra lifecycle của một request ASP.NET Core gọi dịch vụ quét tài liệu bên ngoài, dựa trên evidence để xác định vì sao thao tác downstream không kết thúc sau khi client ngắt kết nối hoặc host bắt đầu shutdown. Thực hành refactor mà vẫn giữ nguyên API contract và kiểm thử failure path.

## Bối cảnh thực tế
Cổng xử lý hồ sơ bảo hiểm gửi tài liệu tới một scanner bên thứ ba. Khi người dùng đóng trang hoặc service nhận tín hiệu shutdown, request ở tầng ngoài đã dừng nhưng một số scan vẫn treo trong worker. CPU thấp; log cho thấy callback cancellation đã được phát ở request boundary, còn downstream task tiếp tục chờ. Incident này được mô phỏng hoàn toàn local, không gửi dữ liệu ra ngoài.

## Bạn cần làm gì
1. Chạy `reproduce.ps1` trên starter nguyên bản và ghi lại hai loại cancellation signal.
2. Đọc `evidence/incident-notes.md`, viết tối thiểu ba hypothesis vào `workspace/my-investigation.md`.
3. Kiểm tra contract giữa service và scanner; chỉ sửa `starter/DocumentValidationService.cs`.
4. Chạy `verify.ps1` trên code đã sửa. Đảm bảo cả caller cancellation, host shutdown và normal completion đều đúng.
5. Sau khi hoàn tất mới xem reference solution và đánh giá trade-offs.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET SDK 8.0+ có thể build `net8.0`; không cần Docker, database, cloud hay NuGet bên thứ ba.

## Chạy nhanh
Từ thư mục unit:

```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1
powershell -ExecutionPolicy Bypass -File .\reproduce.ps1
powershell -ExecutionPolicy Bypass -File .\run.ps1
# Sửa starter/DocumentValidationService.cs sau khi điều tra
powershell -ExecutionPolicy Bypass -File .\verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` build và chạy probe trên **starter nguyên bản**. Probe khởi tạo một scan đang chờ, phát lần lượt tín hiệu hủy từ caller và host trong hai trường hợp độc lập, quan sát trạng thái token và task, rồi tự giải phóng fake scanner để tránh orphan task. Kết quả `EXPECTED_REPRODUCED` là bằng chứng lỗi được tái hiện thành công, không phải fix đã đạt.

## Những gì cần quan sát
- `caller` và `host` có cùng triệu chứng không?
- Token mà fake scanner nhận có phản ánh tín hiệu hủy từ boundary không?
- Task downstream có kết thúc do cancellation hay chỉ kết thúc khi fake scanner được giải phóng?
- Khi không có cancellation, receipt có còn được trả về đúng không?
- Vì sao tăng timeout hoặc chỉ kiểm tra `IsCancellationRequested` ở request boundary chưa chứng minh downstream đã dừng?

Xem [scenario](docs/scenario.md), [incident notes](evidence/incident-notes.md), và [expected before](expected-results/before.md). Dữ liệu incident là mô phỏng, không phải kết quả thực thi tại thời điểm tạo lab.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence.
3. Thử fix trong `starter/`.
4. Verify trên chính starter.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [solution/README.md](solution/README.md). Sau khi tự sửa, có thể chạy `verify.ps1 -Reference` để đối chiếu riêng; không thay thế bước verify starter.

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md).

## Estimated Time
45–60 phút · Difficulty D4 · Maturity L4 (refactoring workshop).
