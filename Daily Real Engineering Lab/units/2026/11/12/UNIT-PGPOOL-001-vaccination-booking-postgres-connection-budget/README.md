# UNIT-PGPOOL-001 — Khi autoscaling làm vượt ngân sách PostgreSQL connections

## Mục tiêu
Đưa ra capacity decision cho ASP.NET Core + PostgreSQL dựa trên bằng chứng, giới hạn shared resources và SLO.

## Bối cảnh thực tế
Cổng đăng ký tiêm chủng cấp quận chạy trên Kubernetes. Trong đợt mở lịch, HPA tăng từ 4 lên 18 pods. HTTP gateway vẫn nhận yêu cầu, nhưng lỗi kết nối database tăng; booking thành công giảm dù CPU trên từng pod giảm.

## Bạn cần làm gì
Chạy replay, thu thập evidence, tính connection budget, so sánh ít nhất hai phương án, chỉnh `workspace/plan.json`, hoàn thành `workspace/adr.md`, rồi verify.

## Yêu cầu môi trường
PowerShell 7+. Không cần PostgreSQL thực; mô hình upper-bound không phải throughput benchmark.

## Chạy nhanh
```powershell
pwsh ./run.ps1
pwsh ./verify.ps1
```

## Cách reproduce vấn đề
Chạy `pwsh ./run.ps1`, so sánh baseline, burst và candidate, ghi lại số connections và headroom.

## Những gì cần quan sát
Phân biệt pool upper-bound với connections active trong snapshot; ghi giả thuyết và phép đo cần thiết để xác định bottleneck.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix và nêu trade-off.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md), [Hint 02](hints/hint-02.md), [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference decision](solution/README.md).

## Expected Results
Cấu hình không vượt shared DB connection budget, có headroom, overload guardrail và kế hoạch load test.

## Estimated Time
50 phút.
