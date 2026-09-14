# UNIT-ARCH-003 — Thiết kế audit retention boundary

## Mục tiêu

Đưa ra quyết định lưu trữ lịch sử thay đổi cho dữ liệu nghiệp vụ cần truy vết 7 năm mà không làm mô hình transactional chính trở nên khó vận hành.

## Bối cảnh thực tế

Một hệ thống B2B cần lưu lịch sử thay đổi trạng thái đơn hàng phục vụ audit. Hiện tại bảng `Orders` chỉ giữ trạng thái hiện tại. Business yêu cầu tra cứu ai thay đổi gì, khi nào, giá trị trước/sau và giữ dữ liệu 7 năm. Team đang cân nhắc SQL temporal tables, audit table riêng, hoặc append-only audit store.

## Bạn cần làm gì

1. Đọc constraints trong `docs/scenario.md`.
2. Ghi assumptions và decision criteria vào `workspace/my-decision.md`.
3. So sánh ít nhất 3 phương án.
4. Chọn một phương án cho hiện tại và nêu điều kiện khiến bạn đổi quyết định.
5. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

Không cần runtime. Đây là Design Decision Lab.

## Chạy nhanh

Bắt đầu từ `docs/scenario.md`.

## Cách reproduce vấn đề

Không áp dụng. Bài toán là lựa chọn data-retention architecture theo constraints thực tế.

## Những gì cần quan sát

- write-path coupling
- queryability của lịch sử
- retention và purge
- schema evolution
- operational complexity
- cost
- khả năng reconstruct timeline

## Quy tắc làm lab

1. Ghi assumptions.
2. Xác định decision criteria.
3. So sánh alternatives.
4. Chọn và bảo vệ quyết định.
5. Xem reference solution sau cùng.

## Hints

Nếu bí, hãy bắt đầu bằng câu hỏi: lịch sử audit có phải là cùng một model và cùng một lifecycle với transactional state hiện tại hay không?

## Reference Solution

> Reference Solution — inspect only after attempting your own decision.

[Reference Solution](solution/README.md)

## Expected Results

Một quyết định có thể bảo vệ bằng constraints, bao gồm trade-offs về consistency, retention, queryability, cost và operational burden.

## Estimated Time

45–60 phút.
