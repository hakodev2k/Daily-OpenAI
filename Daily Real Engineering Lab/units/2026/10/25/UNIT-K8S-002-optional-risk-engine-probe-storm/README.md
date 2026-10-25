# UNIT-K8S-002 — Exam Session Service: Probe-Driven Availability Incident

## Mục tiêu
Điều tra sự cố availability của dịch vụ ASP.NET Core triển khai trên Kubernetes; từ evidence, đánh giá sự khác nhau giữa sức khỏe process, khả năng phục vụ request và tình trạng tính năng phụ trợ.

## Bối cảnh thực tế
Nền tảng thi trực tuyến nhận heartbeat của thí sinh. Core API ghi session vào primary store; optional risk-scoring engine cung cấp cảnh báo bổ sung. Trong một outage 4 nhịp của engine, dashboard ghi nhận pod restart và request bị từ chối dù core process và primary store vẫn healthy.

## Bạn cần làm gì
1. Chạy demo và reproduce, ghi số restart, dropped request và fallback.
2. Ghi ít nhất ba hypothesis và evidence bác bỏ hoặc xác nhận từng hypothesis.
3. Phân loại critical/optional dependencies theo hợp đồng session.
4. Sửa starter/ProbePolicy.cs; không sửa simulator hoặc test oracle.
5. Verify cả optional outage, primary-store outage và process failure.

## Yêu cầu môi trường
- .NET SDK 8.x, PowerShell; không cần Docker, Kubernetes cluster, Azure hoặc NuGet ngoài SDK.
- Simulator deterministic mô phỏng probe decisions, không phải Kubernetes thật.

## Chạy nhanh
Từ thư mục unit:

    ./scripts/setup.ps1
    ./scripts/run.ps1
    ./scripts/reproduce.ps1

Tương đương bằng .NET CLI:

    dotnet run --project starter/ProbeLab.csproj -- demo
    dotnet run --project starter/ProbeLab.csproj -- reproduce

## Cách reproduce vấn đề
reproduce.ps1 chỉ xác nhận triệu chứng gốc, exit 0 khi thấy restart và dropped request. Sau khi sửa, reproduce có thể exit 1 vì lỗi đã biến mất.

## Những gì cần quan sát
- Process và primary store đều healthy trong 8 nhịp.
- Trong 4 nhịp optional engine unavailable, session handler vẫn có degraded response.
- Lưu lượng bị drop và restart xảy ra đồng thời với sự cố của engine.
- verify.ps1 phải FAIL trước khi sửa và PASS sau khi learner sửa starter.

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
⚠️ Spoiler: [Chỉ mở sau khi tự điều tra và sửa](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- [Evidence](evidence/incident-timeline.md)

## Estimated Time
55 phút; ghi lại kết quả tại [workspace/my-investigation.md](workspace/my-investigation.md).
