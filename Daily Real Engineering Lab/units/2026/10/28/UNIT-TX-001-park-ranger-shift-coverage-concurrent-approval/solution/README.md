# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## 1. Symptoms

Hai yêu cầu nghỉ riêng lẻ đều hợp lệ tại thời điểm chúng đọc trạng thái; khi đồng thời, cả hai được duyệt và ca trực còn 0 người. Hệ thống không báo lỗi kỹ thuật.

## 2. Evidence

`reproduce.ps1` cho `approved=2, remaining=0` trong case 2 người. Case 3 người và 1 người là control cases. Điểm đồng bộ `TwoPartyGate` buộc cả hai request đi qua bước đọc trước khi có thay đổi.

## 3. Root cause

Đây là biến thể **write skew / check-then-act trên invariant xuyên nhiều row**. Hai request đọc cùng một trạng thái, mỗi request chỉ thay đổi row riêng của mình. Atomicity của từng `MarkOffDuty` không đảm bảo quy tắc tối thiểu một người trực trên toàn ca.

Đây là mô phỏng cơ chế bằng in-memory store; **không khẳng định** mọi database và mọi isolation level đều tái hiện giống hệt.

## 4. Why the fix works

Mã tham khảo chuyển kiểm tra minimum coverage và cập nhật vào cùng critical section `ShiftRosterStore.TryApproveLeaveKeepingCoverage`. Khi yêu cầu thứ nhất được duyệt, yêu cầu thứ hai quan sát trạng thái đã thay đổi và bị từ chối. `lock` ở đây **chỉ mô phỏng atomic transaction boundary**, không phải cơ chế distributed production.

Để đối chiếu sau khi làm bài:

```powershell
Copy-Item ./solution/ShiftRosterStore.cs ./starter/ShiftRosterStore.cs -Force
Copy-Item ./solution/ShiftLeaveService.cs ./starter/ShiftLeaveService.cs -Force
./verify.ps1
```

## 5. How to verify

`verify.ps1` chạy mã `starter/` mà người học sửa, không chạy mã solution. Phải có chính xác 1 approval khi còn 2 người; cả 2 approvals được phép khi có 3 người; không approval khi chỉ có 1 người. Mỗi case lặp 24 lần. Không dùng `reproduce.ps1` làm tiêu chí thành công sau sửa.

## 6. Alternative fixes

| Phương án | Khi phù hợp | Rủi ro / chi phí |
|---|---|---|
| Transaction `SERIALIZABLE` với retry transaction khi conflict | DB hỗ trợ và transaction kiểm tra đúng tập dữ liệu/predicate | Contention, serialization failures, retry toàn bộ unit of work |
| Một row aggregate cho shift + conditional update / version compare-and-swap | Có thể mô hình hóa mọi approval qua cùng shift record | Hot row, cần xác nhận row count và xử lý conflict |
| Serialize quyết định qua một command processor có durable state | Throughput thấp, yêu cầu thứ tự rõ | Độ trễ, failover, cần durable command handling |

Một transaction ở isolation thấp hơn, chỉ đọc count rồi update row riêng, **có thể không đủ**. Cần kiểm tra semantics cụ thể của DB, isolation, predicate locking và constraints.

## 7. Wrong / tempting fixes

- **Thêm `lock` trong API service:** chỉ đúng với một process; hai pod không chia sẻ lock.
- **Đọc lại số người ngay trước khi update:** vẫn còn khoảng trống giữa kiểm tra và ghi.
- **Chặn mọi yêu cầu đồng thời:** có thể giữ an toàn nhưng gây false rejection, không thỏa case 3 người.
- **Retry không điều kiện:** lặp lại thao tác khi invariant không còn thỏa có thể tạo side effect và lỗi nghiệp vụ.
- **Chỉ dùng `rowversion` của từng nhân viên:** hai request thay đổi hai row khác nhau nên không nhất thiết conflict.

## 8. Production implications

Cần transaction tại authoritative database, xác định shift identity và mọi lệnh làm thay đổi coverage; retry chỉ khi conflict được phân loại rõ; phân biệt `409 Conflict`/business rejection với lỗi 5xx. Log shiftId, requestId, decision, remaining coverage và conflict count; không ghi dữ liệu cá nhân không cần thiết.

## 9. Trade-offs

`SERIALIZABLE` dễ giữ mô hình nghiệp vụ nhưng có contention và deadlock/serialization retries. CAS trên aggregate có thể nhẹ hơn nhưng tạo hot spot. Queue giúp serialize nhưng tăng vận hành và cần idempotency. Chọn theo tần suất cập nhật, topology, SLA và đặc tính DB thật.

## 10. What a Senior engineer should notice

Invariant có phạm vi **ca trực**, không phải từng row nhân viên. Cần xác định linearization point ở nơi dữ liệu authoritative được commit, kiểm chứng với nhiều connection/replica, và test cả điều kiện không mất cơ hội hợp lệ (case 3 người). Không được xem fixture in-memory là bằng chứng isolation của SQL Server/PostgreSQL.

**One defensible production design:** lưu một shift aggregate có revision và active count, thực hiện conditional update trong transaction cùng các row thành viên; kiểm tra affected rows, retry khi version conflict, đồng thời kiểm tra invariant khi có đổi ca, chuyển người hoặc hủy nghỉ. Một thiết kế khác có thể phù hợp hơn nếu DB và workload khác.
