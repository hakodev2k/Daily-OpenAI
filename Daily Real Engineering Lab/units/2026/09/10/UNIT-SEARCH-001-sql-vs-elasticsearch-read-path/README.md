# UNIT-SEARCH-001 — SQL hay Elasticsearch cho Support Case Search?

## Mục tiêu

Rèn luyện khả năng chọn read path dựa trên constraints thực tế thay vì mặc định thêm một search engine vì truy vấn hiện tại bắt đầu chậm.

## Bối cảnh thực tế

Một internal support portal lưu case trong PostgreSQL. Người dùng cần lọc theo status, assignee, created date, customer, đồng thời tìm text trong subject/body. Dữ liệu hiện tại khoảng 2 triệu case, tăng ~70.000 case/tháng. p95 của màn hình search đã tăng lên 1,4–2,1 giây ở giờ cao điểm. Một đề xuất yêu cầu đưa toàn bộ read path sang Elasticsearch; đề xuất khác muốn tiếp tục tối ưu SQL.

Business yêu cầu:

- p95 < 500 ms cho truy vấn phổ biến;
- dữ liệu mới phải xuất hiện trong kết quả trong tối đa 10 giây;
- filter theo tenant và permission phải chính xác;
- team có 5 backend developers, không có search/platform team riêng;
- downtime cho migration phải gần bằng 0;
- chi phí vận hành bổ sung cần có lý do rõ ràng.

## Bạn cần làm gì

1. Phân loại workload: exact filters, sorting, pagination, free-text search, permission filtering.
2. Liệt kê evidence còn thiếu trước khi quyết định kiến trúc.
3. Đề xuất ít nhất 3 phương án khả thi.
4. Chọn một phương án cho giai đoạn hiện tại và giải thích trade-offs.
5. Xác định điều kiện cụ thể khiến bạn đổi quyết định sau này.
6. Ghi quyết định vào `workspace/my-decision.md` trước khi xem reference solution.

## Yêu cầu môi trường

Không cần cloud account hay Elasticsearch instance. Đây là Design Decision Lab dựa trên constraints và evidence.

## Chạy nhanh

Đọc `docs/scenario.md`, sau đó hoàn thành `workspace/my-decision.md`.

## Cách reproduce vấn đề

Không áp dụng cho design lab. Hãy coi số liệu và constraints trong scenario là incident evidence ban đầu cần phân tích.

## Những gì cần quan sát

- Workload không chỉ gồm full-text search.
- Latency hiện tại chưa chứng minh nguyên nhân nằm ở database engine.
- Một hệ thống search riêng tạo thêm consistency, authorization, indexing, monitoring và operational concerns.
- SLA freshness 10 giây cho phép một số mô hình eventual consistency nhưng không tự động làm chúng trở thành lựa chọn tốt nhất.

## Quy tắc làm lab

1. Không chọn technology trước khi liệt kê constraints và evidence thiếu.
2. Phân biệt symptom, bottleneck đã được chứng minh và assumption.
3. So sánh ít nhất ba phương án.
4. Viết migration/rollback implications.
5. Chỉ sau đó mới xem solution.

## Hints

Không có hint trực tiếp. Nếu bí, hãy bắt đầu bằng câu hỏi: “Nếu bỏ full-text khỏi workload, phần còn lại có thực sự cần search engine riêng không?”

## Reference Solution

> Reference Solution — inspect only after attempting your own decision.

- [Một quyết định có thể bảo vệ được](solution/README.md)

## Expected Results

Một câu trả lời tốt phải chỉ ra evidence cần thu thập, boundary của từng option, consistency/authorization implications, operational cost và trigger để revisit quyết định.

## Estimated Time

45–60 phút.
