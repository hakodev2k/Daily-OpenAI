# UNIT-IMGREF-001 — Hai pod Ready nhưng dùng hai bản route engine khác nhau sau rollout

## Mục tiêu
Điều tra sự cố triển khai đa node, chứng minh nguyên nhân bằng dấu vết phiên bản thực chạy và thiết kế quy trình phát hành/rollback có thể kiểm chứng.

## Bối cảnh thực tế
Hệ thống lập tuyến thu gom rác tái chế có hai worker. Sau một đợt phát hành, cả hai worker đều báo Ready, dashboard hiển thị cùng cấu hình release, nhưng cùng một yêu cầu tính tuyến lại cho kết quả khác nhau tùy worker. Nhóm vận hành nghi ngờ thuật toán, dữ liệu tuyến, cache và quá trình rollout.

## Bạn cần làm gì
1. Chạy mô phỏng để tái hiện sự khác biệt giữa hai worker.
2. Ghi tối thiểu ba giả thuyết và phân loại bằng chứng hỗ trợ/phản bác.
3. Sửa duy nhất chính sách lập kế hoạch release trong `starter/DeploymentPlanner.cs`.
4. Chạy bộ kiểm tra trên chính mã đã sửa; chứng minh release mới và rollback đều xác định được bản thực thi.
5. Đề xuất telemetry tối thiểu để phát hiện sự cố tương tự trên Kubernetes thật.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8.x (SDK `8.0.100` trở lên trong feature band 8.x).
- Không cần Docker, Kubernetes cluster, registry hoặc Azure; mô phỏng chạy hoàn toàn local.

## Chạy nhanh
```powershell
cd 'Daily Real Engineering Lab/units/2026/11/01/UNIT-IMGREF-001-recycling-route-image-release-drift'
.\setup.ps1
.\run.ps1
```

## Cách reproduce vấn đề
```powershell
.\reproduce.ps1
```
Lệnh thành công khi tái hiện đúng triệu chứng ban đầu; đây KHÔNG phải kiểm tra bản sửa. Mở `evidence/incident-timeline.md` để đối chiếu dấu vết giả lập.

## Những gì cần quan sát
- Cùng một release được yêu cầu nhưng hai worker báo hai revision thực chạy.
- Worker vẫn Ready và request vẫn trả 200; sự khác biệt là kết quả nghiệp vụ.
- Bản phát hành, cấu hình mong muốn, revision thực chạy và hành vi rollback có thể không trùng nhau.
- Chỉ số CPU, Redis latency và readiness không tự chứng minh nguyên nhân.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/DeploymentPlanner.cs`.
4. Verify bằng `verify.ps1` trên starter đã chỉnh sửa.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
- **Spoiler:** [Reference Solution — chỉ đọc sau khi tự sửa](solution/README.md)

## Expected Results
- Trước sửa: `reproduce.ps1` xác nhận hai worker có revision khác nhau; `verify.ps1` thất bại.
- Sau sửa: `verify.ps1` PASS cho release mới, rollback, tính ổn định của release spec và đầu vào không hợp lệ.
- Xem [before](expected-results/before.md) và [after](expected-results/after.md) sau khi đã ghi bằng chứng.

## Estimated Time
65 phút: 15 phút reproduce/evidence, 15 phút giả thuyết, 20 phút sửa/verify, 15 phút trade-off.
