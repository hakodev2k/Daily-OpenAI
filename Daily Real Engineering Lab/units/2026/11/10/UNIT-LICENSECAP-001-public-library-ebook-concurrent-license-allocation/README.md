# UNIT-LICENSECAP-001 — Thư viện số: cấp quyền mượn ebook khi yêu cầu đồng thời và retry

## Mục tiêu
Thiết kế và kiểm chứng một cơ chế cấp quyền mượn ebook đáp ứng giới hạn bản quyền, chịu được request đồng thời và các tình huống mất kết nối.

## Bối cảnh thực tế
Liên minh thư viện công cộng vận hành API cho 12 chi nhánh, với nhiều ứng dụng web/mobile cùng gửi yêu cầu mượn. Một tựa sách chỉ có 2 bản quyền cho mượn đồng thời. Trong đợt mở đăng ký, dashboard cho thấy 3 lượt mượn thành công trong khi hợp đồng chỉ cho phép 2.

## Bạn cần làm gì
1. Đọc [requirements](requirements.md) và [incident evidence](evidence/timeline.md).
2. Xây dựng ít nhất 3 hypothesis; xác định invariant nghiệp vụ và giới hạn hệ thống.
3. Điền [architecture decision](workspace/decision.md) với ít nhất 2 phương án.
4. Chạy tabletop theo [verification cases](verification/cases.md) và ghi kết quả.
5. Chỉ sau khi tự quyết định, mở hints và reference solution.

## Yêu cầu môi trường
Markdown editor và khả năng lập luận về SQL transaction, API và concurrency. Không cần cloud, Docker hay database để chạy tabletop. Các timeline là dữ liệu giả lập cố định.

## Chạy nhanh
Mở `evidence/timeline.md`, lần lượt replay các mốc T0–T7. Với mỗi mốc, ghi active loans, remaining capacity và API response trong `workspace/decision.md`.

## Cách reproduce vấn đề
Dùng timeline A với 2 license và 3 request đến gần đồng thời; tính kết quả theo contract hiện tại. Sau đó replay timeline B khi client retry sau timeout.

## Những gì cần quan sát
So sánh số loan active, số quyền sử dụng được cấp, số lần billing/license ledger ghi nhận và response trả về cho cùng một operation. Phân biệt response thành công với trạng thái commit thực tế.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc architecture decision.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/01.md)
- [Hint 2](hints/02.md)
- [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Reference architecture](solution/reference.md) — chỉ mở sau khi hoàn thành decision và verification.

## Expected Results
Trước: bảng replay có thể vượt 2 active loans hoặc tạo 2 loan cho cùng retry. Sau: mọi interleaving giữ invariant capacity, cùng operation trả cùng kết quả; timeout không làm mất khả năng reconciliation.

## Estimated Time
80 phút (L6 / D4).
