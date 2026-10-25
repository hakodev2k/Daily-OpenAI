# UNIT-ARCH-007 — Hồ sơ đã xoá nhưng vẫn xuất hiện trên cổng tra cứu

## Mục tiêu

Thực hành vai trò Solution Architect: phân tích một sự cố xuyên nhiều kho dữ liệu, thiết kế một hợp đồng xoá dữ liệu có thể kiểm chứng và đưa ra Architecture Decision Record (ADR) với ràng buộc latency, availability, retention, chi phí và vận hành. Không có một kiến trúc duy nhất đúng.

## Bối cảnh thực tế

CivicArchive vận hành cổng tra cứu hồ sơ công khai cho 120 đơn vị hành chính. Người dân gửi yêu cầu gỡ một hồ sơ có thông tin cá nhân. Bộ phận hỗ trợ xác nhận yêu cầu đã hoàn tất, nhưng vài phút sau người dùng khác vẫn tìm được hồ sơ và mở tệp đính kèm. Đội vận hành báo mọi dịch vụ đều có log xử lý thành công.

## Bạn cần làm gì

Đọc [scenario và constraints](docs/scenario.md), [timeline](evidence/incident-timeline.md), [bản đồ dữ liệu](evidence/data-flow.md) và [acceptance cases](evidence/acceptance-cases.md). Ghi một ADR trong [workspace](workspace/my-decision.md): xác định trust boundaries, giả thuyết, thiết kế luồng từ request tới xác nhận, failure recovery, trade-offs và cách chứng minh không còn truy cập công khai sau xác nhận. So sánh ít nhất ba phương án.

## Yêu cầu môi trường

Chỉ cần Markdown editor; không cần Azure subscription, database hay Docker. Dữ liệu evidence là synthetic. Không chạy thao tác xoá thật.

## Chạy nhanh

1. Đọc bốn tài liệu đầu vào theo thứ tự ở trên.
2. Tự vẽ sequence diagram, hoàn thành ADR và test matrix trong workspace.
3. Chấm bản thiết kế bằng acceptance cases.
4. Chỉ sau đó đọc reference solution.

## Cách reproduce vấn đề

Đây là architecture case study, không có executable failure. Tái hiện trên giấy bằng cách nối từng sự kiện trong incident timeline với luồng dữ liệu hiện tại; chỉ ra ở bước nào trạng thái người dùng quan sát được khác với trạng thái hệ thống đã xác nhận.

## Những gì cần quan sát

- Thời điểm support trả lời khác thời điểm public search và attachment thực sự ngừng trả dữ liệu.
- Mỗi thành phần báo thành công theo tiêu chí riêng; kết quả end-to-end có thể khác.
- Phân biệt dữ liệu gốc, bản sao phục vụ tìm kiếm, cache, link truy cập và backup.
- Không suy luận rằng một HTTP 200 hoặc một sự kiện đã được publish chứng minh mọi bản sao đã xử lý.

## Quy tắc làm lab

1. Viết symptom và hypothesis trước khi chọn giải pháp.
2. Đặt invariant có thể đo được.
3. So sánh phương án và failure paths.
4. Kiểm tra tình huống restore, retry, legal hold và sự cố vùng.
5. Chỉ sau đó xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Một thiết kế tham chiếu và các trade-offs](solution/README.md) — chỉ xem sau khi tự hoàn thành ADR.

## Expected Results

Một ADR có ranh giới nhất quán rõ ràng, sequence diagram, state machine, phương án khôi phục sau lỗi, kiểm soát public visibility, test matrix và tiêu chí rollback. Xem [rubric](expected-results/after.md).

## Estimated Time

70–90 phút. Bài mở rộng: [Senior Challenge](extensions/senior-challenge.md).
