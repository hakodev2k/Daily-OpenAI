# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Formatting hoàn thành nhưng consumer tiếp theo nhận `ObjectDisposedException` khi cố đọc cùng stream.

## 2. Evidence

Stream được tạo ở orchestration scope, được truyền qua helper, và vẫn còn downstream consumer. Exception chỉ xuất hiện sau khi helper return.

## 3. Root cause

`AppendFooter` dispose `Stream` mà caller truyền vào. Helper chỉ mượn resource nhưng vô tình nhận ownership bằng `using (output)`, nên lifetime kết thúc trước consumer cuối cùng.

## 4. Why the fix works

Helper chỉ dispose resource mà chính nó sở hữu (`StreamWriter`) và cấu hình writer để không đóng underlying stream. Caller, là owner tạo stream, dispose sau upload simulation.

## 5. How to verify

Chạy `./verify.ps1`. Learner path phải đọc được cả rows và footer sau khi helper return.

## 6. Alternative fixes

API có thể transfer ownership một cách explicit và documented, hoặc helper có thể tạo/return resource do chính nó sở hữu. Với một số API, copy sang immutable payload cũng hợp lý nếu lifetime isolation quan trọng hơn allocation cost.

## 7. Wrong or misleading fixes

Bỏ toàn bộ `Dispose` chỉ che lỗi và tạo leak risk. Catch `ObjectDisposedException` rồi tạo stream mới làm mất payload. `GC.KeepAlive` không sửa ownership contract vì object đã được dispose chủ động.

## 8. Production implications

Ownership mơ hồ đặc biệt nguy hiểm với streams, HTTP responses, database resources và native handles. Refactor helper có thể thay đổi lifetime dù business logic không đổi.

## 9. Trade-offs

Caller-owned lifecycle rõ ràng nhưng caller phải giữ orchestration responsibility. Ownership transfer có thể đơn giản hóa caller nhưng contract phải explicit. Copy payload cô lập lifetime tốt hơn nhưng tăng memory/CPU.

## 10. What a Senior engineer should notice

`IDisposable` không có nghĩa rằng mọi method nhìn thấy object đều nên dispose nó. Cần phân biệt creator, owner, borrower và transfer boundary; cleanup phải đúng owner và đúng thời điểm.