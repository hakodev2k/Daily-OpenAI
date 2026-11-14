# Reference solution — chỉ xem sau khi tự làm

## Tính toán
Error budget rate = 0.001. Burn = errors / requests / 0.001.
- A: 5m 50, 1h 4.167, 6h 1.429. Policy cũ page vì 5% nhưng chỉ có một lỗi trong 20 request; không đủ bằng chứng page.
- B: 5m 60, 1h 5.333, 6h 0.972. Sự cố nghiêm trọng nhưng cửa sổ 1h có thể chưa phản ánh spike mới. Với dữ liệu snapshot hiện có, policy hai cửa sổ 5m/1h >=14.4 **không page**: đây là false negative quan trọng. Cần cửa sổ thời gian tiến triển hoặc fast-burn detection bổ sung có giới hạn.
- C: 5m 7.5, 1h 7.5, 6h 7.5. Degradation kéo dài; policy cũ không page, nhánh slow-burn 30m/6h >=6 sẽ page khi có 30m evidence.
- D: mọi cửa sổ burn khoảng 0.333. Healthy.
- E: 5m 40, 1h 25, 6h 25. Nếu dùng burn đơn thuần sẽ page vì lưu lượng rất thấp; cần phân luồng theo cohort/branch, traffic floor, absolute-error và synthetic probes.

## Một phương án policy có thể bảo vệ được
1. SLI phân theo route và branch; dashboard global cùng cohort. Mẫu số request phải phù hợp với hành vi người dùng.
2. Fast burn: page khi burn >=14.4 đồng thời ở 5m và 1h, **và** đủ mẫu (ví dụ 1000 request/5m). Không coi threshold này là định luật; kiểm nghiệm bằng traffic thực tế.
3. Slow burn: page khi burn >=6 đồng thời ở 30m và 6h, với đủ request. Dữ liệu không có 30m nên không được tuyên bố đã verify nhánh này.
4. Ticket: burn >=3 ở 1h và 6h khi đủ traffic, hoặc synthetic probes thất bại nhiều lần trong cohort lưu lượng thấp.
5. Incident B là trường hợp khó: tại snapshot 1h có burn 5.333, nên fast page chưa bật dù 5m burn=60. Có thể bổ sung guardrail page khi 5m burn >=50, errors >=100, requests >=1000, lặp lại hai evaluation liên tiếp; điều này page B nhưng không A/E. Guardrail là phương án có trade-off, cần backtest chống spike ngắn.
6. Group/dedupe theo service, route, region; cooldown có điều kiện recovery, không tắt cảnh báo khi đang vượt ngưỡng. Escalation theo tác động người dùng và thời gian.
7. Shadow-mode 7 ngày, đánh giá precision, recall, latency, page count, on-call feedback; rollout theo route, rollback policy trong cấu hình; không sửa SLI định nghĩa âm thầm.

## Wrong fixes
- **Chỉ hạ ngưỡng 5m**: tăng page giả khi traffic thấp.
- **Chỉ dùng 6h**: trễ phát hiện spike lớn.
- **Loại mọi cohort traffic thấp**: có thể bỏ sót branch bị outage hoàn toàn.
- **Chỉ dùng burn rate không kiểm tra số mẫu**: A và E bị đánh thức không cần thiết.
- **Tuyên bố multi-window luôn phát hiện B**: không đúng với snapshot đã cho; cần tính ngưỡng cụ thể.

## Verification
Đối chiếu 15 dòng với `scripts/replay.ps1`, xác nhận A không page, B page nhờ guardrail, C page khi có 30m evidence, D không page, E route sang cohort-specific investigation/ticket. Với C, kết luận chỉ là dự đoán dựa trên tính ổn định của các cửa sổ hiện có, không phải runtime verification của cửa sổ 30m. Phải thu thập dữ liệu rolling thực để đo detection latency.
