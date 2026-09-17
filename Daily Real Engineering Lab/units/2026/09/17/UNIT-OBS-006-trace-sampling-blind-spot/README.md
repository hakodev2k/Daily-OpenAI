# UNIT-OBS-006 — Incident hiếm nhưng trace gần như không bao giờ giữ lại request lỗi

## Mục tiêu
Phân tích một observability design decision khi sampling strategy làm mất phần lớn evidence của một failure hiếm nhưng quan trọng.

## Bối cảnh thực tế
Một checkout API xử lý lưu lượng lớn. Team giảm telemetry volume để kiểm soát chi phí. Sau đó xuất hiện một lỗi khoảng 0.2% request, nhưng dashboard chỉ cho thấy error count tổng hợp; rất ít distributed trace của request lỗi còn tồn tại để điều tra dependency path.

## Bạn cần làm gì
1. Đọc evidence và constraints.
2. Viết ít nhất 3 hypothesis về lý do trace evidence không đại diện cho failure population.
3. Đánh giá sampling boundary hiện tại.
4. Đề xuất strategy giữ được evidence cần thiết mà không gửi 100% telemetry.
5. Nêu trade-off về cost, bias, cardinality và operational complexity.
6. So sánh với reference solution.

## Yêu cầu môi trường
Không cần cloud account. Lab dùng evidence tĩnh và calculation nhỏ.

## Chạy nhanh
Đọc `evidence/incident.md`, sau đó ghi phân tích vào `workspace/my-investigation.md`.

## Những gì cần quan sát
- Traffic volume và error rate.
- Sampling decision diễn ra trước hay sau khi biết outcome.
- Tỷ lệ trace lỗi kỳ vọng còn lại sau sampling.
- Metrics và traces trả lời các câu hỏi khác nhau như thế nào.

## Quy tắc làm lab
1. Evidence trước.
2. Hypothesis trước recommendation.
3. Định lượng trade-off.
4. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Reference Solution — chỉ xem sau khi đã tự phân tích](solution/README.md)

## Expected Results
Bạn phải đưa ra một strategy có thể bảo toàn diagnostic value cho rare failures trong giới hạn telemetry budget, đồng thời giải thích bias và giới hạn của strategy.

## Estimated Time
45–60 phút.