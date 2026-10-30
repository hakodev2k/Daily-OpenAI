# Synthetic incident evidence

| Mốc | Sự kiện | Quan sát |
|---|---|---|
| T0 08:00 | Đặt `zone_id NOT NULL` trong DB thử nghiệm | V1 insert chỉ có text bị từ chối |
| T1 08:05 | Bỏ constraint; V1 và V2 cùng chạy | Nhiều tuyến chưa có zone_id |
| T2 08:08 | Backfill đọc route 1205 | V1 đổi zone trước khi batch ghi |
| T3 08:12 | Backfill xử lý `Old Town` | Có hai canonical zones, thử nghiệm tự chọn dòng đầu |
| T4 08:20 | Rollback V2 về V1 | Cập nhật chỉ ghi zone_id không hiển thị ở V1 |

Baseline: write p95 110 ms, DB CPU 65%. Backfill không throttle: CPU 91%, p95 440 ms trong 8 phút. Tỷ lệ dispatch sai cho tuyến chưa xác minh phải bằng 0.

**Evidence tasks:** chỉ ra failure mode của từng T0–T4; giải thích vì sao mapping duy nhất 98,67% chưa đủ để contract; phân biệt lỗi schema compatibility, concurrent write và data quality. CSV chỉ là sample, không đại diện toàn bộ 12 triệu hàng.
