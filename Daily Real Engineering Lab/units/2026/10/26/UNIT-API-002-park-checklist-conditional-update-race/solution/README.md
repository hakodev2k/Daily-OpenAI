# Reference Solution — Chỉ xem sau khi tự sửa

## Symptoms

Hai client cùng đọc revision 7, gửi ghi chú khác nhau với cùng If-Match. Cả hai được báo thành công nhưng trạng thái cuối chỉ giữ một ghi chú, revision vẫn là 8.

## Evidence

`reproduce.ps1` cho thấy hai status 200 và một revision increment. Điểm đồng bộ của harness buộc cả hai request đi qua cùng một cửa sổ race. Không cần dựa vào timing ngẫu nhiên.

## Root cause

Starter kiểm tra `expected == _revision` trước một điểm `await` có thể nhường execution. Khi hai request cùng vượt qua check, cả hai tiếp tục ghi từ revision cũ. Điều kiện tiền đề và mutation không nằm trong cùng atomic boundary; revision cuối bị ghi đè thành `expected + 1`.

## Why the fix works

Trong `solution/RevisionedChecklistStore.cs`, sau khi hai request đã đến rendezvous, kiểm tra revision và cập nhật note/revision được thực hiện dưới cùng một `lock`. Request đầu tiên cập nhật revision 8; request thứ hai thấy ETag 7 đã stale và nhận 412. `Read()` cũng đồng bộ để lấy snapshot nhất quán. Không giữ lock xuyên `await`.

## How to verify

Tại thư mục unit:

```powershell
pwsh -File ./verify.ps1
dotnet run --project solution/ChecklistLab.csproj -- verify
```

Lệnh đầu kiểm tra code người học trong `starter/`; lệnh thứ hai tự kiểm reference. Chạy `reproduce.ps1` trên starter đã sửa sẽ không còn đạt điều kiện tái hiện (đây là hành vi đúng).

## Alternative fixes

- Với SQL Server/PostgreSQL: `UPDATE ... SET Revision = Revision + 1, Note = @note WHERE Id = @id AND Revision = @expected`; thành công khi affected rows = 1, còn 0 tương ứng precondition failed (sau khi phân biệt record not found).
- Dùng optimistic concurrency token/rowversion của EF Core, bắt `DbUpdateConcurrencyException` và ánh xạ hợp đồng API.
- Có thể dùng transaction với isolation phù hợp, nhưng transaction đơn thuần không thay thế được điều kiện compare-and-swap khi yêu cầu nghiệp vụ là từ chối stale edits.

## Wrong or misleading fixes

- `Task.Delay` hoặc retry khi 412: chỉ đổi lịch race hoặc âm thầm ghi đè ý định của người khác.
- Chỉ dùng `volatile` cho revision: visibility không biến check + write thành atomic operation.
- Khóa riêng từng request: không tạo mutual exclusion giữa các request.
- Luôn tăng revision sau mỗi request: che giấu việc hai bản sửa cùng được nhận, không bảo vệ người cập nhật đầu tiên.

## Production implications

ETag cần gắn với đúng representation/version; mọi instance API phải dùng chung atomicity boundary ở **database**, không dùng `lock` cục bộ nếu scale-out. Xử lý 404/412/428 nhất quán, tránh log nội dung nhạy cảm, đo tỉ lệ 412 và trải nghiệm giải quyết xung đột trên UI. Bản mẫu không phải HTTP parser đầy đủ.

## Trade-offs

Optimistic concurrency ít contention khi xung đột hiếm, đổi lại client phải đọc lại và hòa giải thay đổi. Pessimistic locking có thể phù hợp các phiên chỉnh sửa dài nhưng làm tăng lock contention và rủi ro timeout. Chọn theo xác suất xung đột, SLA và tính quan trọng của dữ liệu.

## Senior engineer should notice

Một test tuần tự pass không chứng minh tính đúng đắn dưới concurrency. Kiểm thử cần ép cả hai request quan sát cùng revision, assert **số lần được chấp nhận**, revision và nội dung winner, đồng thời bao phủ stale/malformed/missing precondition. Atomicity phải đúng tại nguồn dữ liệu có thẩm quyền, không chỉ tại controller.
