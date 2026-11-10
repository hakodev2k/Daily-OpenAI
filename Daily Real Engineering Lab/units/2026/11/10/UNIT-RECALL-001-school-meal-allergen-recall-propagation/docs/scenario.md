# Scenario

- Supplier Registry là nguồn có thẩm quyền; trạng thái lot có revision tăng đơn điệu. RECALLED là terminal.
- Kitchen read model nhận sự kiện at-least-once, p99 delay có thể 6 phút; event có thể duplicate hoặc đảo thứ tự.
- Picklist tạo lúc 09:00; dispatch 09:15–09:35; xe rời kho lúc 09:40.
- Hiện POST /dispatch chỉ đọc projection cache.
- Mục tiêu p95 dưới 250ms, availability 99.9%; safety quan trọng hơn availability. Khi không chứng minh được an toàn phải HOLD.
- Registry có thể timeout 90 giây. Có quy trình thay thế nguyên liệu thủ công.

## Requirement change
Supplier có thể recall 400 lots cùng lúc, projector trễ 10 phút. Không được dispatch lot đã recall. Phân tích cả thời điểm check và thời điểm side effect vật lý: có thể có TOCTOU.

## Deliverables
Sơ đồ data flow, safety invariant, 2–3 phương án, lựa chọn có trade-off, failure policy, migration, rollback, metrics, kiểm thử A–G.
