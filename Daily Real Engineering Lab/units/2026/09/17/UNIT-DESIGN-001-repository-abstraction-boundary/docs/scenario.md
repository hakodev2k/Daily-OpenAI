# Scenario

## System
- ASP.NET Core monolith trên .NET 8.
- EF Core + SQL Server.
- Khoảng 80 tables hiện có.
- Module mới `Pricing Rules` dự kiến thêm khoảng 12 tables.
- Team 5 developers, release mỗi hai tuần.

## Workload của module mới
- CRUD cho rule definitions.
- Projection queries cho màn hình quản trị.
- Một số query cần filtering/sorting động.
- Optimistic concurrency khi chỉnh rule.
- Background reconciliation đọc và cập nhật nhiều records theo batch.
- Một use case cần transaction qua nhiều entity trong cùng database.

## Codebase hiện tại
- Một số module inject `DbContext` trực tiếp vào application service.
- Một số module dùng `IRepository<T>` generic.
- Một số module có specialized repositories cho aggregate/query phức tạp.
- Generic repository hiện đã có khoảng 14 methods và vẫn tiếp tục tăng.

## Constraints
- Không có kế hoạch thay SQL Server trong ít nhất 18 tháng.
- Team có thể chạy integration tests với SQL Server container trong CI.
- Không yêu cầu persistence-agnostic domain model tuyệt đối.
- Lead muốn convention đủ rõ để code review không tranh luận lại mỗi PR.
- Không được tạo abstraction chỉ vì khả năng giả định rằng database có thể thay trong tương lai.

## Options cần đánh giá
A. `DbContext` trực tiếp tại application layer với query/command code rõ ràng.

B. Generic `IRepository<T>` + Unit of Work cho mọi aggregate.

C. Specialized repositories chỉ ở những boundaries có domain/persistence semantics đáng kể, còn read projections có thể dùng query services hoặc `DbContext` theo convention.

Bạn có thể đề xuất option D nếu nó phù hợp constraints hơn.

## Decision record phải trả lời
- Boundary nằm ở đâu?
- Transaction ownership thuộc layer/component nào?
- Query projection được tổ chức thế nào?
- Test strategy là gì?
- Khi nào repository có giá trị thực?
- Khi nào repository chỉ che EF Core mà không giảm coupling hữu ích?
- Convention nào ngăn codebase trôi về nhiều style tùy ý?
- Signals nào buộc team revisit quyết định?