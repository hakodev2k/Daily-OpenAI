# Reference Solution — SPOILER

## Symptoms
Attempt đầu đọc đủ attachment rồi gặp transient failure. Attempt sau hoàn tất nhưng lưu payload rỗng.

## Evidence
`BytesReadPerAttempt` cho thấy lần đầu đọc toàn bộ dữ liệu, lần sau không đọc được byte nào.

## Root cause
Retry đang tái sử dụng cùng một input `Stream`. Nguồn mô phỏng là non-seekable và đã bị consume trong attempt đầu, nên retry không có payload để replay.

## Vì sao fix hoạt động
Reference implementation capture payload một lần trước retry boundary, sau đó tạo một readable stream mới cho từng attempt. Mỗi retry vì vậy nhận cùng logical request body.

## Cách verify
Chạy `verify.ps1`. Cả hai attempt phải đọc đủ payload và blob cuối cùng phải đúng nội dung ban đầu.

## Wrong fixes
- Chỉ tăng retry count: tăng số lần gửi payload rỗng.
- Gán `Position = 0`: không áp dụng cho nguồn non-seekable và không phải contract tổng quát của upload input.
- Bỏ retry: tránh triệu chứng nhưng làm giảm resilience với transient storage failures.

## Alternatives và trade-offs
Buffer toàn bộ payload trong memory đơn giản nhưng không phù hợp file rất lớn. Production system có thể spool vào temporary file, dùng replayable request abstraction, hoặc dựa vào SDK transfer API có retry semantics phù hợp. Cần đặt giới hạn kích thước và cancellation/cleanup rõ ràng.

## Production implications
Retry chỉ an toàn khi operation input có thể được replay theo cùng semantics. Với upload lớn, memory pressure, temporary storage capacity, idempotency và lifecycle cleanup phải được xem như một phần của retry design.

## Senior insight
Retry không chỉ là policy về thời gian và exception. Nó còn tạo ra yêu cầu đối với **replayability của operation state**. Trước khi retry một I/O operation, phải xác định input nào đã bị consume hoặc mutated bởi attempt trước.