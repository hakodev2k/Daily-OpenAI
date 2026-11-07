# Evidence — incident pack (dữ liệu giả lập)

## Timeline
- 08:00: tăng số cảm biến tại 3 depot mới.
- 08:20: active series trên metric backend tăng nhanh; CPU của API vẫn bình thường.
- 08:35: dashboard regional counts trả kết quả chậm; event log vẫn nhận đầy đủ.
- 09:00: team on-call yêu cầu giữ phân tích theo region, depot, outcome và truy vết từng sensor khi cần.

## Production constraints
- 120.000 cảm biến, 250 depot, 4 region, 150 tenant.
- Khoảng 1,5 triệu evaluation events/ngày; số request identifier gần như duy nhất.
- Budget metric backend: 5.000 active series cho họ metric này, truy vấn p95 dưới 3 giây.
- On-call cần aggregate theo region/depot/outcome trong 30 ngày.
- Forensic theo sensor/request phải tra cứu được từ **event log có kiểm soát truy cập**, giữ 7 ngày; không yêu cầu dùng metric labels để drill down tới request.
- Tenant isolation và giới hạn truy cập dữ liệu là bắt buộc.

## Local fixture contract
- 2 region × 3 depot/region × 4 sensor/depot × 3 outcome × 5 request/sensor/outcome = **360 events**.
- Mỗi cặp depot/outcome tương ứng 18 operational groups.
- Local budget: **36 series** (thu nhỏ để kiểm chứng cơ chế).
- Fixture deterministic, không mô phỏng thời gian retention hay chi phí cloud thực.

## Noise
- Có một lần tăng network RTT khoảng 10 ms trong cùng khung giờ; chưa có bằng chứng cho thấy nó làm tăng số active series.
- API error rate ổn định.

## Acceptance
Giữ đủ dimension cần cho vận hành; local fixture không vượt budget; giải thích riêng phương án forensic log, giới hạn truy cập, retention và production rollout.
