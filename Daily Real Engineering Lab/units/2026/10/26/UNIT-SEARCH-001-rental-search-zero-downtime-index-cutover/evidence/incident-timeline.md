# Synthetic incident timeline — lần thử thay schema trước đó

> Evidence mô phỏng để điều tra. Không có khách hàng thật, credential hay dữ liệu nhạy cảm.

| UTC | Signal | Observation |
|---|---|---|
| 08:00 | deploy | Mapping change bắt đầu trên index đang phục vụ query |
| 08:08 | search p95 | 180 ms → 470 ms, CPU search nodes 83% |
| 08:13 | search 5xx | 0.03% → 2.1%; một số truy vấn tiện nghi mới trả về rỗng |
| 08:18 | source updates | PostgreSQL tiếp tục 350 events/s; CDC lag 22 phút |
| 08:25 | incident response | Rollback application flag; search p95 còn 410 ms |
| 08:44 | data audit | 9/10,000 sampled active listings không xuất hiện trong kết quả tương ứng |
| 09:02 | operations | Trạng thái index ổn định dần; 5xx về baseline |
| 09:30 | review | Quyết định dừng migration và lập ADR trước lần tiếp theo |

## Telemetry được cung cấp

- Source PostgreSQL write latency p95 giữ trong mức bình thường (12–19 ms).
- API gateway p95 tăng theo search dependency; không có deployment frontend cùng thời điểm.
- Một số query cũ vẫn thành công, một số query theo field mới không ổn định.
- Document count index và source gần bằng nhau, nhưng sample correctness không đạt.

## Những gì chưa biết

- Truy vấn bị ảnh hưởng có liên quan đến shard nào?
- Có sự kiện cập nhật đến trong lúc copy snapshot hay không?
- Mapping v1 và v2 có thể phục vụ đồng thời trên một index hay không?
- Nếu quay lại v1 sau cutover, index v1 có còn nhận các thay đổi mới không?

Hãy ghi giả thuyết, phương pháp xác nhận và quyết định cần evidence nào trước khi chọn giải pháp.
