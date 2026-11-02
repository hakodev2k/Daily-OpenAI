# Scenario — firmware 4.8.2

Fleet có 24.000 solar inverter ở 6.000 site (4/site), ba vùng North, Central, South, mỗi vùng 8.000 thiết bị. Thiết bị chỉ có outbound polling; 30% thường offline trong cửa sổ một giờ. Fleet đang dùng firmware 4.7.x; 4.8.2 đã qua QA lab nhưng pilot hiện trường có tín hiệu bất thường.

08:00: mở pilot 240 thiết bị (80/vùng). 08:55: chốt snapshot. 09:05: yêu cầu mở wave 5%. Một nhóm nghi dashboard đang cộng sai offline; nhóm khác phát hiện ACK cũ đến sau ACK mới.

Bạn cần ra quyết định go/no-go, thiết kế control plane, state machine, safety gates, pause, recovery và observability có thể kiểm chứng bằng các sự kiện đính kèm.

## Change request lúc 10:00

Hoàn thành rollout cho 6.000 thiết bị ưu tiên trong 48 giờ. Business đồng ý chỉ đo tỷ lệ hoàn thành trên thiết bị online và đủ điều kiện; offline phải báo cáo riêng. Bạn phải tính capacity, sửa acceptance criteria hoặc chứng minh mục tiêu không khả thi.

## Deliverables

Architecture diagram, event/state contract, cohort plan, rollback/runbook, capacity math, threat model, ADR so sánh tối thiểu hai phương án và walkthrough A1–A6.
