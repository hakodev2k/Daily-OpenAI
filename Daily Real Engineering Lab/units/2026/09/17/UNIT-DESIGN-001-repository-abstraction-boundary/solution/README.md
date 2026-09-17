# Reference Solution — Spoiler

Đây là **một defensible solution**, không phải đáp án duy nhất.

## Decision
Với constraints hiện tại, chọn hybrid boundary gần Option C: không ép toàn bộ module qua generic repository. Cho phép EF Core `DbContext`/query services phục vụ projection-oriented reads theo convention; dùng specialized repository khi aggregate boundary hoặc persistence operation có semantics đáng kể cần một interface ổn định.

## Vì sao
EF Core đã cung cấp identity map/change tracking, query composition và unit-of-work-like `SaveChanges` semantics. Một generic repository có nhiều methods thường bắt đầu mirror ORM API, làm mất query expressiveness nhưng không tạo isolation có giá trị tương ứng.

Ngược lại, inject `DbContext` tùy ý ở mọi nơi cũng dễ làm transaction ownership và persistence logic lan rộng. Vì vậy boundary cần được quy định bằng architecture convention chứ không chỉ bằng số lượng interfaces.

## Proposed conventions
- Application use case sở hữu transaction boundary; không để controller tự phối hợp nhiều saves.
- Read-heavy projections dùng dedicated query services và projection trực tiếp tới DTO.
- Aggregate có persistence semantics phức tạp có thể có specialized repository với operations mang business meaning.
- Không thêm method vào generic repository chỉ để expose một EF query mới.
- Cross-aggregate transaction trong cùng database được phối hợp tại use-case boundary bằng cùng `DbContext`/transaction.
- Concurrency tokens và retry/conflict policy phải được xử lý explicit ở use case có requirement đó.

## Testing
Không lấy khả năng mock repository làm mục tiêu chính. Pure domain logic vẫn unit test không cần database. Persistence contracts, query translation, indexes/constraints và transaction behavior được integration test với relational database phù hợp production semantics.

## Các lựa chọn khác vẫn có thể đúng
### DbContext trực tiếp rộng hơn
Có thể hợp lý với CRUD-centric module nhỏ nếu architecture rules đủ rõ và team chấp nhận EF Core là persistence dependency của application layer.

### Repository rộng hơn
Có thể hợp lý nếu domain operations có persistence semantics ổn định, nhiều data sources cần phối hợp, hoặc organization có boundary/plugin requirement thật. Nhưng đó phải là requirement hiện tại, không phải giả định mơ hồ rằng database có thể đổi.

## Wrong approaches
- Tạo generic repository chỉ để có `GetAll`, `Find`, `Add`, `Update`, rồi liên tục thêm overload khi query phức tạp lên.
- Mock `IRepository` và coi test xanh là bằng chứng EF query chạy đúng trên SQL Server.
- Đưa transaction control vào từng repository khiến một use case khó đảm bảo atomicity qua nhiều entities.
- Cấm hoàn toàn repository hoặc bắt buộc repository tuyệt đối mà không xét loại workload.

## Production implications
Boundary ảnh hưởng trực tiếp đến khả năng quan sát query shape, transaction lifetime, concurrency handling và performance tuning. Abstraction quá kín có thể che mất ORM/database capabilities; boundary quá lỏng có thể làm persistence concerns lan khắp application.

## Trade-offs
Hybrid convention cần code review discipline hơn một rule máy móc `mọi entity phải có repository`. Đổi lại, abstraction được tạo khi nó mang semantic value và read path không phải phá abstraction để lấy lại khả năng query.

## Revisit triggers
- Module bắt đầu cần nhiều persistence implementations thật.
- Domain invariants đòi hỏi aggregate persistence operations phức tạp hơn.
- Team liên tục copy transaction/concurrency orchestration giữa use cases.
- Query code phân tán đến mức khó kiểm soát performance/security.
- Architecture boundary thay đổi do extraction thành service riêng.

## Senior insight
Câu hỏi quan trọng không phải “Repository Pattern có tốt không?” mà là “Boundary nào đang giảm coupling có ý nghĩa đối với constraints hiện tại, và chi phí abstraction có thấp hơn chi phí coupling mà nó loại bỏ không?”