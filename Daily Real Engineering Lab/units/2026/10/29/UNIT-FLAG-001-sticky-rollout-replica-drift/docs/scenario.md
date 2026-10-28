# Incident dossier — Municipal Water Portal

Đợt canary rollout biểu đồ mới có khiếu nại tính năng bật ở lần đăng nhập đầu và tắt ở lần kế. Hai replica cùng feature name và rollout target 20%. Load balancer chuyển request giữa instance.

## Evidence giả lập

- 09:10 Replica A, tenant=north-district, subject=resident-0021, chart-v2=enabled.
- 09:11 Replica B, cùng identity, chart-v2=disabled.
- 09:15 Dashboard ~20% enabled trên request, không breakdown theo user.
- 09:17 Database latency bình thường, không thấy HTTP 5xx.

Đây là tình huống mô phỏng; fixture là nguồn evidence có thể tái lập.

## Constraints

Không thêm DB assignment, cloud hoặc package ngoài. Cohort phải ổn định qua restart và replica, dùng trusted identity, 0–10.000 basis points. Production algorithm change cần kế hoạch migration/rollback.
