# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Report trả `CapturedCount = 2` nhưng `CapturedTotal = 600.00`. Mỗi giá trị riêng lẻ từng đúng tại một thời điểm, nhưng cặp giá trị không mô tả cùng một database state.

## 2. Evidence

Statement đầu chạy trước writer commit. Statement thứ hai chạy sau writer commit. Writer hoàn thành trong khi transaction report vẫn mở.

## 3. Root cause

`READ COMMITTED` ngăn statement đọc uncommitted data nhưng không cung cấp một snapshot cố định cho toàn transaction. Hai statement trong cùng transaction có thể quan sát hai committed states khác nhau.

## 4. Why the fix works

Với `ALLOW_SNAPSHOT_ISOLATION ON`, đặt report transaction ở `SNAPSHOT` khiến các reads trong transaction quan sát một transaction-consistent versioned view. Writer vẫn có thể commit mà không cần report giữ shared locks cho toàn transaction.

## 5. How to verify

Thay nội dung learner-editable `starter/session-a-report.sql` theo giải pháp của bạn rồi chạy `./scripts/verify.ps1`. Reference code nằm ở `solution/session-a-report.sql` để so sánh sau khi đã thử.

## 6. Alternative fixes

Có thể gom hai aggregates vào một SQL statement; khi business requirement chỉ cần statement-level consistency, đây thường là lựa chọn đơn giản hơn. `SERIALIZABLE` cũng có thể cung cấp stronger guarantees nhưng locking/blocking behavior khác đáng kể.

## 7. Wrong or misleading fixes

- Thêm `NOLOCK` làm consistency yếu hơn và có thể đọc dữ liệu chưa commit.
- Chỉ bọc code trong transaction mà giữ nguyên assumptions về isolation không tạo transaction-level snapshot.
- Retry report không loại bỏ race; nó chỉ cho một cơ hội khác để race không xuất hiện.
- Chặn writer thủ công có thể che symptom nhưng làm thay đổi concurrency characteristics của production workload.

## 8. Production implications

Row versioning tăng áp lực lên version store và cần monitoring. Long-running snapshot transactions có thể giữ old row versions lâu hơn dự kiến.

## 9. Trade-offs

`SNAPSHOT` cung cấp consistent reads với ít reader/writer blocking hơn nhưng cần versioning resources và hiểu write-conflict semantics. Một statement aggregate duy nhất có surface area nhỏ hơn nếu requirement cho phép.

## 10. What a Senior engineer should notice

`BEGIN TRANSACTION` không tự định nghĩa mức consistency mà business invariant cần. Phải map invariant của report sang isolation semantics, đo concurrency impact và chọn scope nhỏ nhất cung cấp guarantee cần thiết.