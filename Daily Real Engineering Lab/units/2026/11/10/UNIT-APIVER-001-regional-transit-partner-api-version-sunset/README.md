# UNIT-APIVER-001 — Quyết định sunset API thanh toán vé khi đối tác chưa nâng cấp

## Mục tiêu
Đưa ra quyết định triển khai và retirement cho một API có nhiều consumer không đồng bộ về năng lực nâng cấp; bảo vệ giao dịch vé và lập tiêu chí go/no-go dựa trên bằng chứng.

## Bối cảnh thực tế
Nền tảng đối soát vé xe buýt cung cấp `POST /v1/fare/confirm` cho các kiosk và nhà vận hành. Phiên bản v2 có contract khác và cần nâng cấp client. Đội sản phẩm muốn tắt v1 trong 21 ngày để giảm chi phí hỗ trợ. Một số đối tác chưa xác nhận kế hoạch chuyển đổi. Tắt sai thời điểm có thể khiến vé đã quét nhưng không được ghi nhận.

## Bạn cần làm gì
1. Đọc [dữ liệu tình huống](evidence/partner-readiness.md), phân loại dữ liệu đáng tin và chưa đủ tin.
2. Viết ADR trong [workspace](workspace/my-decision.md): ít nhất 3 phương án, rủi ro, owner và tiêu chí go/no-go.
3. Xây bảng cutover theo tuần; xử lý tình huống đối tác offline 14 ngày rồi gửi lại giao dịch.
4. Tự đánh giá theo [rubric](docs/verification-rubric.md), sau đó mới mở lời giải.

## Yêu cầu môi trường
Chỉ cần Markdown editor, Git và PowerShell (tuỳ chọn). Không cần Azure, Docker hay subscription.

## Chạy nhanh
Mở `evidence/partner-readiness.md`, điền `workspace/my-decision.md` và kiểm tra bằng `docs/verification-rubric.md`.

## Cách reproduce vấn đề
Tại mốc T+21, giả định route v1 bị tắt hoàn toàn; lần lượt replay ba trường hợp K01, K03 và K04 trong bảng evidence. Ghi các request nào không thể hoàn tất, khả năng retry và tác động nghiệp vụ. Không giả định traffic 0 trong một ngày có nghĩa là không còn consumer.

## Những gì cần quan sát
So sánh traffic theo đối tác, thời gian offline, lỗi hợp đồng, tỷ lệ migrate, yêu cầu audit và tính khả thi của rollback. Phân biệt yêu cầu kỹ thuật và áp lực deadline.

## Quy tắc làm lab
1. Reproduce trước bằng timeline.
2. Ghi hypothesis.
3. Thử đề xuất quyết định.
4. Verify bằng rubric và scenario replay.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ được](solution/README.md). Không phải kiến trúc đúng duy nhất.

## Expected Results
Một ADR chỉ rõ decision, trade-offs, owner, deadline, rollback trigger, consumer-specific acceptance, cách chứng minh không mất hoặc ghi trùng giao dịch.

## Estimated Time
75 phút.
