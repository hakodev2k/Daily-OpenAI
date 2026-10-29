# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi hypothesis và thử sửa.

## 1. Symptoms
Preview A có bytes đúng ngay khi tạo, nhưng bị thay đổi sau một yêu cầu B dù không có code nào chủ động cập nhật A. Vấn đề là tính ổn định dữ liệu giữa producer và consumer, không phải hiệu năng.

## 2. Evidence
Fixture cấp một mảng, nhận lại mảng đó rồi tái sử dụng cho request tiếp theo. Vì A là view trên mảng, A sau khi tạo B mang bytes của B. Reproduce phải in REPRODUCED và exit 0 trên starter gốc.

## 3. Root cause
PreviewService trả ReadOnlyMemory<byte> trỏ vào mảng đã trả cho ArrayPool<byte> trong finally. Return chấm dứt quyền sử dụng của caller đối với lease; một consumer giữ view không sở hữu bytes đó. ReadOnlyMemory chỉ hạn chế cách ghi thông qua API, không tạo immutable snapshot hay kéo dài lifetime.

## 4. Why the fix works
Trong solution/PreviewService.cs, copy đúng vùng hợp lệ sang mảng owned trước khi Return. Consumer nhận một vùng nhớ không bị pool ghi đè. Dùng AsSpan(0, renderedTile.Length), không lấy toàn bộ capacity của mảng thuê.

## 5. How to verify
Thay nội dung starter/PreviewService.cs bằng code tham chiếu (hoặc bản sửa của bạn) rồi chạy ./verify.ps1. Kỳ vọng mọi check PASS, exit 0. ./reproduce.ps1 sau khi sửa phải báo NOT REPRODUCED và exit khác 0.

## 6. Alternative fixes
- Bỏ pool ở đoạn này, tạo byte[] riêng từ đầu: đơn giản, có thể hợp lý nếu payload nhỏ.
- Dùng IMemoryOwner<byte> và chuyển quyền sở hữu rõ ràng đến consumer: tránh copy nhưng phải quản lý Dispose sau khi consumer thực sự xong, kể cả exception/cancellation.
- Dùng một abstraction kiểu owned immutable payload có giới hạn size và retention policy: thích hợp khi dữ liệu tồn tại trong queue lâu.

## 7. Wrong / tempting fixes
- Chỉ đổi kiểu trả về thành ReadOnlyMemory: không đổi ownership.
- Return(clearArray: true): làm payload biến thành zero sớm hơn, không giải quyết lifetime.
- Lock quanh Prepare: lock hết hạn trước khi consumer đọc, không ngăn reuse về sau.
- Chỉnh RecyclingArrayPool để không reuse: che bug, không đáp ứng contract của ArrayPool.Shared.
- Dùng Task.Delay hoặc tăng capacity pool: chỉ thay đổi xác suất gặp lỗi.

## 8. Production implications
Định nghĩa rõ ai sở hữu bytes, khi nào release, giới hạn kích thước và thời gian giữ payload. Với pipeline nhiều worker, thêm regression cho concurrent consumers và cancellation. Tránh giữ pool lease qua hàng đợi không có giới hạn thời gian.

## 9. Trade-offs
Copy-out tăng O(n) copy và một allocation trên mỗi preview, đổi lại contract đơn giản và ít rủi ro use-after-return. Lease-based owner có thể giảm copy nhưng làm API và đường lỗi phức tạp hơn. Đo throughput, GC allocations, retained bytes, queue dwell time trước khi tối ưu.

## 10. Senior engineer should notice
Test cần kiểm tra tính bất biến của kết quả sau khi service trả về, không chỉ đúng ở thời điểm tạo. Tách correctness khỏi tối ưu hóa. Fixture là deterministic model của một lịch reuse hợp lệ, không phải benchmark hay chứng minh lịch reuse cụ thể của ArrayPool.Shared.
