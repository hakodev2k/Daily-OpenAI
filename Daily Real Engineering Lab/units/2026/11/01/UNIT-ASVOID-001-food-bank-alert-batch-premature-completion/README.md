# UNIT-ASVOID-001 — Lô cảnh báo kho lạnh được đánh dấu hoàn tất quá sớm

## Mục tiêu

Điều tra một batch dispatch trong .NET có trạng thái hoàn tất không khớp với số cảnh báo đã được gateway xác nhận. Tự tái hiện, thu thập bằng chứng, sửa implementation và kiểm chứng cả completion lẫn failure propagation.

## Bối cảnh thực tế

Một food bank khu vực gửi cảnh báo nhiệt độ cho ba kho lạnh. Job chạy định kỳ, ghi trạng thái `COMPLETED` khi hàm dispatch trả về. Trong ca trực gần đây, dashboard báo batch hoàn tất nhưng số thông báo được gateway ghi nhận vẫn bằng 0 tại thời điểm đó. Một lần retry có thể khiến người vận hành hiểu sai tình trạng gửi.

## Bạn cần làm gì

1. Chạy bản starter và ghi lại thời điểm batch báo hoàn tất so với các lần gửi.
2. Đưa ra ít nhất hai giả thuyết (gateway chậm, trạng thái batch sai, lỗi xử lý bất đồng bộ...).
3. Sửa `starter/DispatchService.cs` sao cho kết quả `Task` phản ánh công việc thực sự, đồng thời lỗi gateway phải được quan sát bởi caller.
4. Chạy kiểm chứng trên **starter đã sửa**, không dùng reference solution làm đối tượng test.

## Yêu cầu môi trường

- Windows PowerShell 5.1 hoặc PowerShell 7+.
- .NET SDK 8.0.x (không cần NuGet package bên ngoài, Docker hoặc Azure).
- Chạy lệnh từ thư mục unit này.

## Chạy nhanh

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```

Lệnh `verify.ps1` **phải FAIL trên starter ban đầu**; sau khi bạn sửa, nó phải PASS. `reproduce.ps1` là kiểm tra triệu chứng ban đầu, không phải bài kiểm tra sau sửa.

## Cách reproduce vấn đề

```powershell
./scripts/reproduce.ps1
```

Harness giữ gateway ở một cổng chờ có kiểm soát. Không phụ thuộc vào tốc độ mạng hoặc `Task.Delay` ngẫu nhiên. Đọc ba trường `dispatchTaskCompleted`, `sentBeforeRelease`, `sentAfterRelease`.

## Những gì cần quan sát

- Khi gateway chưa được phép hoàn tất, `Task` đại diện cho batch đang ở trạng thái nào?
- Sau khi gateway được mở, số lần gửi hoàn tất có khớp với ba cảnh báo không?
- Khi gateway phát sinh lỗi, caller có nhận được lỗi qua `Task` không?
- Phân biệt **đã khởi động gửi** và **đã hoàn tất gửi**.

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

**Spoiler:** Chỉ đọc sau khi đã tự điều tra và sửa: [Giải pháp tham chiếu](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)
- [Bằng chứng sự cố mô phỏng](evidence/incident-timeline.md)
- [Mẫu ghi chép điều tra](workspace/my-investigation.md)

## Estimated Time

35–45 phút. Độ khó D3; hoạt động L1. Bản phát hành được **static-review**; mã nguồn chưa được thực thi bởi tác vụ tạo lab.
