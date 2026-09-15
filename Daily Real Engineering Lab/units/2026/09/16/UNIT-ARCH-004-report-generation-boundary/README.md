# UNIT-ARCH-004 — Chọn execution boundary cho report generation

## Mục tiêu
Luyện engineering judgment khi một chức năng report đang tăng tải và team cần quyết định giữ xử lý trong HTTP request hay chuyển sang một execution model khác.

## Bối cảnh thực tế
Một internal operations portal cho phép export báo cáo CSV. Hiện tại report nhỏ hoàn thành trong vài giây. Business dự kiến một số tenant sẽ tăng dữ liệu đáng kể trong 6 tháng tới. Team đề xuất ba hướng: tiếp tục synchronous HTTP, chạy background worker trong cùng application, hoặc tách sang queue + worker riêng.

Không có phương án mặc định đúng. Bạn cần chọn boundary phù hợp với constraints hiện tại thay vì chọn kiến trúc phức tạp nhất.

## Bạn cần làm gì
1. Đọc constraints và incident evidence trong `scenario.md`.
2. Viết decision vào `workspace/my-decision.md` trước khi xem solution.
3. Chọn một phương án chính và nêu rõ điều kiện khiến bạn đổi quyết định.
4. So sánh latency, reliability, deployment, retry, cancellation, observability, cost và operational burden.
5. Sau đó đọc `solution/README.md` để so sánh reasoning.

## Yêu cầu môi trường
Không cần runtime hoặc cloud account. Đây là Design Decision Lab dựa trên constraints.

## Chạy nhanh
Mở `scenario.md`, sau đó điền `workspace/my-decision.md`.

## Cách reproduce vấn đề
Không áp dụng runtime reproduction. Evidence trong scenario mô phỏng workload và failure history mà team đang quan sát.

## Những gì cần quan sát
- Report hiện tại và report dự kiến khác nhau ở đâu về duration và resource usage.
- Failure nào cần retry và failure nào không nên retry tự động.
- User có cần giữ HTTP connection mở cho tới khi report hoàn thành không.
- Operational complexity nào team thực sự có khả năng vận hành.

## Quy tắc làm lab
1. Đọc evidence trước.
2. Ghi assumptions.
3. Chọn decision.
4. Nêu trade-offs và trigger để revisit.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints.md` nếu bị kẹt.

## Reference Solution
`solution/README.md` — một phương án defensible, không phải kiến trúc duy nhất đúng.

## Expected Results
Bạn phải có một decision có thể bảo vệ bằng constraints, cùng các rejection reasons cho phương án còn lại và các measurable triggers để đánh giá lại sau này.

## Estimated Time
45–60 phút.