# UNIT-DDD-001 — Domain Invariant Boundary

## Mục tiêu

Phân tích một luồng reservation có kết quả đúng khi chạy tuần tự nhưng có thể vi phạm business invariant khi nhiều request diễn ra đồng thời. Đề xuất một thiết kế bảo vệ invariant với transaction/concurrency boundary rõ ràng và giải thích trade-off.

## Bối cảnh thực tế

Một inventory service giữ số lượng hàng khả dụng và nhận reservation từ nhiều checkout node. Hệ thống hiện tại đọc trạng thái, kiểm tra điều kiện rồi ghi reservation bằng các bước riêng. Test tuần tự đều pass, nhưng production đôi lúc ghi nhận tổng lượng reserved lớn hơn stock thực tế.

## Bạn cần làm gì

1. Đọc scenario và evidence.
2. Ghi ít nhất ba hypothesis có thể giải thích symptom.
3. Xác định invariant nào thực sự cần được bảo vệ và boundary nào phải chịu trách nhiệm cho nó.
4. Đề xuất một phương án chính và ít nhất một phương án thay thế.
5. Phân tích consistency, contention, retry, scalability và operational complexity.
6. So sánh với reference solution sau khi hoàn thành quyết định của bạn.

## Yêu cầu môi trường

Không yêu cầu dịch vụ cloud hay database. Đây là design-decision lab; bạn chỉ cần editor Markdown.

## Chạy nhanh

Mở `scenario.md`, `evidence.md`, sau đó điền `my-decision.md`.

## Cách reproduce vấn đề

Dùng timeline trong `evidence.md` để mô phỏng hai reservation cạnh tranh trên cùng SKU. Kiểm tra liệu từng request có thể quan sát trạng thái hợp lệ tại thời điểm validation nhưng kết quả cuối cùng lại vi phạm business invariant hay không.

## Những gì cần quan sát

- Invariant nào bị phá vỡ.
- Validation và state transition hiện nằm ở những boundary nào.
- Những giả định nào chỉ đúng khi request chạy tuần tự.
- Phương án nào yêu cầu coordination và coordination đó xảy ra ở đâu.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Đề xuất decision.
4. Tự challenge decision bằng failure/concurrency cases.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

⚠️ Spoiler: [Reference Solution](solution/README.md)

## Expected Results

Bạn phải đưa ra một consistency boundary có thể giải thích được vì sao invariant vẫn đúng dưới concurrent requests, đồng thời nêu rõ trade-off và failure handling.

## Estimated Time

45–75 phút.