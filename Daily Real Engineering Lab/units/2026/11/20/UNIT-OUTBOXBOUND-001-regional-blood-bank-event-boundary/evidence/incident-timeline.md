# Evidence packet — dữ liệu giả lập, không phải telemetry thực
## Timeline A
08:00:00.100 API ghi `Released` vào SQL Server.
08:00:00.112 SQL COMMIT thành công; HTTP 200.
08:00:00.118 Pod bị kill trong lúc chuẩn bị publish.
08:01:00.000 DB: Released/version 9; broker: không có EventId E-91; shipment: chưa có.

## Timeline B
08:04:00.200 SQL COMMIT Released/version 10.
08:04:00.230 Broker nhận E-92; ACK bị mất do network reset.
08:04:01.000 API retry publish E-92; broker ghi nhận thêm một delivery.
08:04:01.400 Consumer tạo shipment S-42; crash trước broker ACK.
08:04:05.000 Broker redeliver E-92; consumer cố tạo shipment S-43.

## Timeline C — nhiễu
p95 HTTP latency 110 ms, CPU 19%, SQL deadlocks 0; không đủ để kết luận event delivery an toàn.
Consumer A xử lý version 12 trước version 11 cho cùng BloodUnitId.
Cần xác định sự khác biệt giữa ordering guarantee ở broker và invariant ở DB consumer.

## Nhiệm vụ
Ghi rõ trạng thái trước/sau mỗi crash, nguyên nhân khả dĩ, dữ liệu cần thu thập để phân biệt mất publish với consumer lag, và điểm nào có thể kiểm tra lại an toàn.
