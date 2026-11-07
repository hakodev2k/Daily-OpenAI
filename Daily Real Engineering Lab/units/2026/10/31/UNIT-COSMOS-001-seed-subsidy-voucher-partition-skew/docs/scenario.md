# Scenario và ràng buộc thiết kế

Một chương trình hỗ trợ giống cây trồng xử lý redemption voucher tại 40 hợp tác xã. Một hợp tác xã đầu mối phát sinh khoảng 90% lưu lượng trong khung 15 phút; hệ thống cần bảo đảm không đổi trùng voucher khi client retry sau timeout.

## Synthetic fixture

- 1.000 voucher ID khác nhau, `SEED-0000` đến `SEED-0999`.
- 900 sự kiện đầu thuộc `COOP-HOT`; 100 sự kiện còn lại chia cho 10 hợp tác xã.
- Tất cả thuộc `R1` và ngày `2026-10-31`.
- 8 bucket logic dùng để minh họa phân phối; giới hạn synthetic là **không bucket nào quá 22%**.
- Một retry cho `SEED-0042` xuất hiện sau cùng. Đây là một request lặp lại, **không** tính là voucher mới.
- API nhận voucher ID để kiểm tra redemption, không được thêm lookup bắt buộc trước point read.
- Có dashboard theo cooperative/day, có thể chấp nhận read model cập nhật chậm tối đa 30 giây.
- Nhóm vận hành có 4 người, thời gian migration 6 tuần, không được ngừng redemption.

## Bài tập

1. Chạy policy hiện tại; lập bảng giả thuyết (skew, thiếu tổng RU, truy vấn cross-partition, retry).
2. So sánh `cooperative`, `region-day`, `voucher-shard`, `attempt-shard` theo phân phối, retry, point read và truy vấn dashboard.
3. Chọn một chính sách đáp ứng acceptance checks, sửa `starter/policy.json`, chạy verify.
4. Phác thảo API contract cho `Redeem(voucherId, cooperativeId, requestId)` và `Get(voucherId)`, giải thích duplicate cùng voucher ở hai replica.
5. Đề xuất migration không downtime: backfill, đối soát, chuyển đọc/ghi, rollback, metrics, giới hạn chi phí.
6. Phân biệt rõ kết luận từ simulator với điều phải kiểm chứng bằng Cosmos DB metrics / load test thật.

## Acceptance

- `maxBucketShare <= 0.22` trên 1.000 voucher gốc.
- Retry cùng voucher phải định tuyến đến cùng logical partition.
- Điểm đọc theo voucher ID không cần secondary lookup đồng bộ.
- Đề xuất cách chặn redemption trùng **tại authoritative write boundary**, không dựa vào cache.
- Dashboard và migration có phương án rõ ràng, không coi synthetic buckets là physical partitions.

Không có một thiết kế cloud duy nhất luôn tối ưu; chứng minh lựa chọn dưới ràng buộc này.
