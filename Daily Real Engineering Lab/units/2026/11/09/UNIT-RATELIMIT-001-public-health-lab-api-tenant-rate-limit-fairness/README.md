# UNIT-RATELIMIT-001 — Thiết kế quota công bằng cho API trả kết quả xét nghiệm

## Mục tiêu
Đưa ra quyết định kỹ thuật có thể kiểm chứng khi nhiều tenant chia sẻ một API, cân bằng công bằng, độ trễ, độ tin cậy và chi phí.

## Bối cảnh thực tế
Một hệ thống y tế vùng cung cấp API cho phòng khám khẩn cấp và các đối tác đồng bộ dữ liệu định kỳ. Khi đối tác chạy import lớn, phòng khám báo không thể nhận kết quả đúng thời hạn. Hệ thống vẫn có CPU và database headroom ở vài thời điểm. Nhóm vận hành đang đề xuất tăng số replica hoặc nới quota toàn cục.

## Bạn cần làm gì
1. Đọc [workload](evidence/workload.md) và phân biệt dấu hiệu với giả thuyết.
2. Chọn ít nhất hai phương án trong [decision brief](docs/decision-brief.md), tính ngân sách và mô phỏng các mốc tải.
3. Ghi ADR trong [workspace](workspace/decision.md), kèm các bất biến và kế hoạch rollout/rollback.
4. Tự chấm theo [acceptance](docs/acceptance.md), sau đó mới mở reference solution.

## Yêu cầu môi trường
Không cần cloud hay database. Markdown editor và máy tính cầm tay đủ để chạy tabletop replay.

## Chạy nhanh
Mở `evidence/workload.md`; đi lần lượt từng cửa sổ 10 giây. Đánh dấu request được chấp nhận, bị từ chối và lý do. Tính riêng từng tenant.

## Cách reproduce vấn đề
Dùng quy tắc baseline trong evidence và bảng arrival. Xác định số request phòng khám bị từ chối tại cửa sổ cao điểm; không thay đổi giả định giữa các phương án.

## Những gì cần quan sát
Tỷ lệ 429 theo tenant, capacity còn trống, mức độ burst, khả năng xác định nguyên nhân từ metrics và tác động đến mục tiêu dịch vụ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử thiết kế và phản biện.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ được](solution/README.md). Không phải đáp án duy nhất.

## Expected Results
Một ADR nêu rõ quota, admission rules, failure behavior, metrics, fairness và cách xác minh bằng workload replay; không chấp nhận giải pháp chỉ tăng replica.

## Estimated Time
65 phút.
