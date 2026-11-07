# UNIT-DESIGN-001 — Có nên giữ Repository abstraction cho module mới?

## Mục tiêu
Rèn luyện engineering judgment khi chọn persistence boundary cho một module ASP.NET Core + EF Core mới trong hệ thống đang có nhiều abstraction lịch sử.

## Bối cảnh thực tế
Một monolith nội bộ đang được mở rộng thêm module `Pricing Rules`. Codebase hiện có `DbContext`, generic repository, specialized repository và Unit of Work ở các khu vực khác nhau. Team muốn thống nhất cách làm cho module mới trước khi implementation bắt đầu.

Module dự kiến có khoảng 12 aggregate/table, phần lớn CRUD nhưng có vài query projection phức tạp, optimistic concurrency và một background reconciliation job. Team 5 người, release mỗi hai tuần, chưa có kế hoạch đổi database trong 18 tháng tới.

## Bạn cần làm gì
1. Đọc constraints trong `docs/scenario.md`.
2. Ghi assumptions và decision drivers vào `workspace/my-decision.md`.
3. Đánh giá ít nhất ba phương án persistence boundary.
4. Chọn một phương án cho module mới và mô tả rõ boundary của nó.
5. Xác định những trường hợp ngoại lệ mà team được phép dùng cách khác.
6. Định nghĩa signals khiến quyết định này cần được xem xét lại sau này.
7. Sau khi hoàn thành, so sánh với `solution/README.md`.

## Yêu cầu môi trường
Không cần runtime hoặc database. Đây là Design Decision Lab dựa trên constraints thực tế.

## Chạy nhanh
```text
README.md → docs/scenario.md → workspace/my-decision.md
```

## Cách reproduce vấn đề
Không có executable failure. Vấn đề cần tái hiện là decision pressure: cùng một module nhưng các thành viên đề xuất persistence boundaries khác nhau và mỗi lựa chọn tạo ra chi phí khác nhau cho query capability, testability, maintenance và coupling.

## Những gì cần quan sát
- Abstraction nào đang giải quyết requirement thật và abstraction nào chỉ được giữ vì convention.
- Query shape của module có phù hợp với một generic interface hay không.
- Test strategy có thực sự cần mock persistence hay có thể dùng relational integration tests.
- Transaction boundary và concurrency behavior nằm ở đâu.
- Chi phí vận hành và thay đổi code khi abstraction bị mở rộng theo thời gian.

## Hints
Không mở solution trước khi ghi decision drivers và trade-offs của riêng bạn.

## Reference Solution
Xem `solution/README.md` sau khi hoàn thành decision record.

## Expected Results
Không có một đáp án duy nhất. Kết quả tốt phải đưa ra lựa chọn phù hợp constraints, boundary rõ ràng, trade-offs cụ thể và revisit triggers có thể kiểm chứng.

## Estimated Time
45–60 phút.