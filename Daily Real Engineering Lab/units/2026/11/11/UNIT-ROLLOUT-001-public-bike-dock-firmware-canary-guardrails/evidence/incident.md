# Synthetic incident evidence

- 09:00: rollout v3.8 tới 100 trạm: 80 urban, 20 riverside.
- 10:00: fleet dashboard tính failure rate trên tất cả 200.000 unlock attempts.
- 10:05: 14 support tickets từ 20 trạm riverside v3.8; 2 tickets từ 80 trạm urban v3.8.
- 10:08: CPU, memory và HTTP 5xx trên central API không tăng; đây chỉ là negative evidence, không chứng minh firmware an toàn.
- 10:10: đội rollout đề xuất mở rộng tới 50% vì fleet-wide failure rate thấp.
- Chưa có dữ liệu sau rollback, chưa có telemetry về radio link quality; không suy diễn nguyên nhân vật lý.
- Business SLO: failed unlock <= 1% trong mỗi cohort có >= 1.000 attempts/60 phút; > 2% là hard stop. Cohort nhỏ hơn ngưỡng phải kéo dài quan sát, không mặc định pass.
