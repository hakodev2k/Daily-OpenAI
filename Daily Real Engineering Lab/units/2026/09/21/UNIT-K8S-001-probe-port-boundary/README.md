# UNIT-K8S-001 — Probe Port Boundary

## Mục tiêu
Điều tra một deployment Kubernetes trong đó application process chạy ổn định nhưng Pod không bao giờ được đưa vào tập endpoint phục vụ traffic.

## Bối cảnh thực tế
Warehouse API vừa được chuyển từ local container sang Kubernetes. Log khởi động không có exception, container không restart, nhưng Service không gửi request tới Pod mới và rollout không đạt trạng thái Ready.

## Bạn cần làm gì
Dùng starter và evidence để tái hiện trạng thái deployment, lập hypothesis về boundary giữa application và Kubernetes, sau đó sửa cấu hình deployment trong `starter/` và chạy verification.

## Yêu cầu môi trường
- PowerShell 7+
- Không cần Kubernetes cluster; lab dùng validator cục bộ để mô phỏng contract cần điều tra.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Application được cấu hình lắng nghe ở đâu.
- Probe được cấu hình gửi request tới đâu.
- Trạng thái Ready mô phỏng và tác động tới Service traffic.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Chỉ sửa `starter/deployment.yaml`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Reference Solution — chỉ xem sau khi tự điều tra](solution/README.md)

## Expected Results
Trước khi sửa, validator phải xác nhận Pod không đạt readiness contract. Sau khi sửa, `verify.ps1` phải báo `PASS` mà không thay đổi application listener.

## Estimated Time
40 phút.
