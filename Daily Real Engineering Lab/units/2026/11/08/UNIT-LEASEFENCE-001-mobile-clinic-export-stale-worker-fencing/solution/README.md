# Reference Solution — Spoiler

## Root cause
Kiểm tra lease một lần lúc bắt đầu không bảo vệ side effect. Sau khi generation 42 đã được cấp, worker A generation 41 vẫn có thể replace cùng object và ghi status COMPLETED. Đây là stale-owner write, không phải lỗi timeout đơn thuần.

## Invariant
Mỗi job có generation tăng đơn điệu do một authority cấp atomically. Một commit chỉ được chấp nhận nếu generation của request đúng generation hiện tại và job chưa được hoàn tất bởi generation cao hơn. Không dựa vào đồng hồ local.

## Giao thức đề xuất
1. Acquire/renew lease trong DB transaction với server time và tăng generation khi chuyển owner; lưu `job.current_generation`.
2. Worker xuất kết quả ra object **immutable** theo `jobId/generation/contentHash`; upload chưa công bố không phải kết quả chính thức.
3. Trong một DB transaction, compare-and-swap: `UPDATE jobs SET published_key=@key, published_generation=@g, status='COMPLETED' WHERE id=@id AND current_generation=@g AND (published_generation IS NULL OR published_generation<=@g)`. Kiểm tra affected rows = 1; đồng thời kiểm tra content hash/idempotency key để retry cùng generation không đổi nội dung.
4. Consumer chỉ đọc object qua `published_key` trong DB. Không cho worker replace public object trực tiếp.
5. Object staging mồ côi được garbage collect sau retention window và audit; không xóa bản đang được tham chiếu.
6. Retry: nếu generation cũ thì reject; nếu cùng generation và cùng hash thì trả lại kết quả idempotent; cùng generation khác hash là conflict cần điều tra.

## Replay
09:00:38 B ghi staging v2; 09:00:39 DB publish generation 42. 09:00:46 A có thể upload staging v1 nhưng CAS với generation 41 trả về 0 rows; v2 vẫn được publish. C5 không có public pointer cho upload chưa commit. C7 DB authority dùng server clock. C8 generation không giảm.

## Wrong fixes
- Tăng lease TTL: chỉ giảm xác suất, không bảo đảm an toàn.
- Kill worker A: mạng partition không bảo đảm có thể kill tức thì.
- Redis lock không fencing: vẫn có stale side effect nếu downstream không kiểm tra generation.
- Last-write-wins theo timestamp: clock skew và arrival order không phản ánh quyền sở hữu.
- Chỉ kiểm tra lease trước upload: race giữa check và publish.

## Trade-offs
DB pointer + immutable object thêm orphan cleanup, transaction load và hai bước publish nhưng tách upload chậm khỏi commit nguyên tử. Nếu downstream storage hỗ trợ conditional versioning/fencing, có thể thực thi token ngay tại write boundary. Cần phân biệt at-least-once execution với exactly-once externally visible publication.

## Verification và vận hành
Chạy C1–C8 bằng state-machine replay. Instrument `lease_generation`, `commit_rejected_stale`, `publish_conflict`, `orphan_staging_count`, `published_generation`. Chaos test pause A, expire lease, B commit, resume A; kiểm tra pointer vẫn trỏ generation 42. Rollout theo canary, giữ reader đọc pointer, giám sát tỷ lệ stale rejects trước khi mở rộng.