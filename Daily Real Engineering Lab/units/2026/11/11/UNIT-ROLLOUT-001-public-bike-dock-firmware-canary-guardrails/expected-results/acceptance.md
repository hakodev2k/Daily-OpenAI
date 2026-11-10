# Acceptance / self-check

- [ ] Tính đúng aggregate 1.170 / 200.000 = 0,585%.
- [ ] Tính đúng v3.8 riverside 180 / 2.000 = 9%; v3.8 urban 40 / 8.000 = 0,5%.
- [ ] Không suy luận v3.8 gây lỗi radio khi chưa có kiểm chứng.
- [ ] Chọn HOLD hoặc ROLLBACK cho cohort riverside; không mở rộng toàn fleet.
- [ ] Nêu stage 1% → 5% → 20% → 50% → 100%, có observation window, cohort quotas và owner.
- [ ] Guardrail per-cohort, sample-size floor, error budget, customer-impact metric, independent stop switch.
- [ ] Rollback có timeout, kiểm chứng phiên bản đang chạy, và test sau rollback.
- [ ] Phân biệt evidence hiện có, hypothesis và telemetry cần bổ sung.
- [ ] Xem xét offline stations, staged activation, configuration/firmware compatibility và tránh blast radius lan rộng.
