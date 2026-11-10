# UNIT-ROLLOUT-001 — Firmware rollout: Canary có thật sự an toàn?

## Mục tiêu
Thiết kế quyết định rollout dựa trên cohort evidence, đo được blast radius và rollback, thay vì chỉ nhìn health toàn hệ thống.

## Bối cảnh thực tế
Một thành phố vận hành 2.000 trạm khóa xe đạp. Firmware v3.8 được phát hành theo từng đợt. Dashboard báo fleet success 99,4%, nhưng tổng đài ghi nhận nhiều trạm khu vực ven sông không trả xe đúng hạn. Đội vận hành cần quyết định **go / hold / rollback** trước khi mở rollout lên 100%.

## Bạn cần làm gì
1. Phân tích bộ dữ liệu trong [evidence/rollout.csv](evidence/rollout.csv) và [evidence/incident.md](evidence/incident.md).
2. Đề xuất tối thiểu 3 hypothesis; xác định metric nào đủ/không đủ để ra quyết định.
3. Viết ADR tại [workspace/decision.md](workspace/decision.md) với rollout stages, guardrails, rollback trigger, owner và cách kiểm chứng.
4. Đối chiếu với [expected-results/acceptance.md](expected-results/acceptance.md) rồi mới mở reference.

## Yêu cầu môi trường
Không cần cloud hoặc tài khoản Azure. Dùng Excel/PowerShell hoặc text editor để tính tỷ lệ từ CSV; mọi số liệu là synthetic.

## Chạy nhanh
```powershell
Import-Csv ./evidence/rollout.csv | Format-Table
```

## Cách reproduce vấn đề
Tính success rate theo toàn fleet, sau đó tính riêng cho từng cohort (firmware version × station group) trên cùng bảng dữ liệu. So sánh hai quyết định rollout.

## Những gì cần quan sát
Quan sát sample size, thời điểm đo, cohort exposure, failed unlock attempts và khác biệt giữa fleet aggregate với nhóm được cập nhật. Không dùng một tỷ lệ tổng hợp làm bằng chứng duy nhất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix/decision.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
[Acceptance criteria](expected-results/acceptance.md)

## Estimated Time
65–85 phút.
