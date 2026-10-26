# UNIT-PAR-001 — OCR hồ sơ cấp phép: batch đúng dữ liệu nhưng trễ SLA

## Mục tiêu
Điều tra throughput của batch I/O; phân biệt bottleneck, đo concurrency thực tế, sửa và xác minh giới hạn downstream.

## Bối cảnh thực tế
Cổng dịch vụ công xử lý 24 hồ sơ qua OCR đối tác. Dữ liệu đủ và không có lỗi nhưng batch thường trễ SLA. Hợp đồng đối tác cho phép tối đa 8 request in-flight mỗi worker; vượt giới hạn có thể bị từ chối. SRE đã truyền --max-concurrency 8 nhưng hiệu năng không cải thiện.

## Bạn cần làm gì
1. Chạy starter và thu thập requestedConcurrency, effectiveConcurrency, maxActive, elapsedMs.
2. Ghi ít nhất hai hypothesis vào [workspace](workspace/my-investigation.md) trước khi đọc implementation.
3. Chỉ sửa starter/Program.cs, giữ nguyên JSON metrics và hợp đồng CLI.
4. Kiểm tra cấu hình 3, 8, 16: mỗi hồ sơ đúng một lần và không vượt capacity.
5. Chạy verify.ps1, sau đó mới xem reference.

## Yêu cầu môi trường
.NET SDK 8+, PowerShell. Không cần NuGet, Docker, Azure hay OCR thật.

## Chạy nhanh
~~~powershell
./run.ps1
./reproduce.ps1
~~~
Linux/macOS: dotnet run --project harness/Verifier.csproj -- reproduce starter/PermitOcrBatch.csproj

## Cách reproduce vấn đề
reproduce.ps1 kiểm tra starter gốc với yêu cầu concurrency 8. Exit 0 nghĩa là **đã reproduce được triệu chứng**, không phải đã sửa xong.

## Những gì cần quan sát
- OCR mô phỏng có thời gian chờ I/O ổn định.
- maxActive khác với concurrency được yêu cầu.
- Thời gian batch không tương xứng với năng lực downstream.
- Tăng concurrency vô hạn không được chấp nhận.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút.
