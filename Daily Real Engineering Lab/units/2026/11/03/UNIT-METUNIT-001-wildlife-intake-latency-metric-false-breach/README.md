# UNIT-METUNIT-001 — Dashboard báo động độ trễ dù API cứu hộ vẫn nhanh

## Mục tiêu
Điều tra telemetry không khớp request trace, sửa instrumentation và xây dựng regression test cho metric contract.

## Bối cảnh thực tế
API đặt lịch tiếp nhận động vật hoang dã vừa triển khai instrumentation mới. Dashboard cảnh báo p95 vượt SLO 0,75 giây nhưng trace và phản hồi người dùng vẫn bình thường. Nhóm cần bằng chứng trước khi rollback. Toàn bộ dữ liệu đều mô phỏng.

## Bạn cần làm gì
1. Reproduce và ghi UNIT, SAMPLES, P95_SECONDS, BREACH.
2. Đưa ra ít nhất hai giả thuyết; đối chiếu trace độc lập với metric.
3. Sửa starter/LatencyRecorder.cs, không đổi ngưỡng SLO, fixture hay tests.
4. Chạy verify.ps1 trên learner-editable starter; ghi lại kết quả và phương án rollout.
5. So sánh reference solution sau khi tự hoàn thành.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET 8 SDK. Không cần Azure, Docker hay package ngoài SDK.

## Chạy nhanh
~~~powershell
.\setup.ps1
.\run.ps1
.\reproduce.ps1
~~~
Tương đương: dotnet run --project starter/MetricLab.csproj

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên starter chưa sửa. Script chỉ xác nhận triệu chứng cũ. Sau khi sửa, dùng verify.ps1 thay vì cố làm reproduce pass.

## Những gì cần quan sát
So sánh trace độc lập trong evidence/incident-timeline.md với instrument metadata, measurement, p95 và alert rule. Ghi bằng chứng ủng hộ/bác bỏ từng giả thuyết. Không kết luận nguyên nhân chỉ từ một cảnh báo.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md), [Hint 02](hints/hint-02.md), [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự thử.

## Expected Results
Trước sửa: reproduce.ps1 thấy BREACH=true; verify.ps1 fail. Sau sửa: verify.ps1 pass trên starter, run.ps1 thấy BREACH=false với cùng dữ liệu. Xem expected-results.md.

## Estimated Time
65 phút (reproduce 15, investigation 20, fix/test 20, trade-offs 10).
