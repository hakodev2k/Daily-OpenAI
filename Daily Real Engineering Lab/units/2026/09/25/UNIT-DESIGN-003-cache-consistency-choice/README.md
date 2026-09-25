# UNIT-DESIGN-003 — Choose a Consistency Strategy for Product Availability

## Mục tiêu
Đưa ra một design decision có thể bảo vệ bằng evidence và constraints thay vì mặc định thêm cache.

## Bối cảnh thực tế
Product availability API phục vụ nhiều region. Read traffic cao, update theo burst. Business chấp nhận dữ liệu cũ tối đa 10 giây nhưng checkout phải kiểm tra authoritative state trước khi commit.

## Bạn cần làm gì
Đánh giá ít nhất ba phương án, ghi assumptions, failure modes, operational cost và chọn một phương án cho read path.

## Yêu cầu môi trường
Không cần service bên ngoài. Dùng worksheet trong repository.

## Chạy nhanh
Mở `workspace/decision.md` và hoàn thành decision matrix.

## Cách reproduce vấn đề
Dùng workload/constraints trong `evidence/constraints.md` để kiểm tra từng phương án trước burst update và dependency degradation.

## Những gì cần quan sát
Freshness, primary-store load, failure behavior, complexity và checkout correctness boundary.

## Quy tắc làm lab
1. Ghi assumptions trước.
2. So sánh ít nhất ba options.
3. Chọn decision và nêu điều kiện làm decision thay đổi.
4. Chỉ sau đó xem reference.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
Một decision có metrics/constraints, failure behavior và trade-offs rõ ràng.

## Estimated Time
60 phút.