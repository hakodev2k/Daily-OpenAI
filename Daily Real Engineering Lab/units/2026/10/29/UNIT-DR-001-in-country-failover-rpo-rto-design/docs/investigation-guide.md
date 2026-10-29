# Investigation guide

1. Vẽ đường đi của acknowledged write và nơi durable commit được xác nhận.
2. Tách zone outage, region outage, network partition và old-primary rejoin.
3. Lập ma trận A–E theo P1–P6: PASS/FAIL/UNKNOWN, kèm nguồn chứng cứ.
4. Tính khoảng vượt ngưỡng RPO của C3. Không dùng p95 thay upper bound.
5. Xác định dữ liệu phải kiểm tra trước promotion: last acknowledged commit, last durable replica position, fencing epoch.
6. Phân biệt RTO đến HTTP reachable và RTO đến safe writes resumed.
7. Đề xuất phép thử cho worst-case backlog, lost telemetry và network partition.
8. Ghi giả định có thể đảo ngược quyết định.
