# Reference Solution — chỉ xem sau khi tự làm

## 1. Symptoms
Một logical job có thể được delivery lại trong khi delivery cũ vẫn tiếp tục chạy; cả hai execution có thể ghi completion side effect.

## 2. Evidence
Timeline cho thấy processing duration lớn hơn lock duration. Delivery đầu mất lock trước khi kết thúc, broker redeliver cùng WorkItem, trong khi handler đầu vẫn tiếp tục.

## 3. Root cause
Handler giả định quyền sở hữu message tồn tại cho toàn bộ thời gian xử lý. Khi lock hết hạn mà chưa được renew, broker được phép redeliver. Side effect lại xảy ra trước khi handler biết `CompleteAsync` không còn hợp lệ.

## 4. Why fix works
Reference solution renew lock định kỳ trong lúc processing còn hợp lệ và đặt side effect sau khi lifecycle được kiểm soát. Production cần thêm idempotency key ở durable side-effect boundary vì renewal không loại bỏ mọi nguồn redelivery.

## 5. How to verify
Chạy `verify.ps1`; deterministic long-running job phải kết thúc với `FINAL_SIDE_EFFECTS=1` và exit code 0.

## 6. Alternative fixes
- Chia work thành các bước ngắn hơn và checkpoint durable.
- Dùng broker/client auto-lock-renewal với giới hạn phù hợp.
- Thiết kế side effect idempotent bằng operation/message ID.
- Với workload rất dài, queue message chỉ khởi tạo durable job rồi worker khác quản lý lifecycle.

## 7. Wrong / misleading fixes
- Chỉ tăng lock duration rất lớn: có thể giảm symptom nhưng không tạo idempotency và làm recovery chậm khi worker chết.
- Catch `MessageLockLost` rồi coi là success: side effect có thể đã chạy và redelivery vẫn tiếp tục.
- Tăng số consumer: làm incident duplicate side effect dễ nghiêm trọng hơn.

## 8. Production implications
Theo dõi delivery count, lock-lost exceptions, processing duration distribution, duplicate operation IDs và dead-letter rate. Đặt renewal limit để job treo không giữ message vô hạn.

## 9. Trade-offs
Renewal kéo dài ownership nhưng tăng coupling với broker lifecycle. Idempotency durable tăng storage/transaction complexity nhưng bảo vệ trước nhiều loại redelivery hơn.

## 10. What a Senior engineer should notice
At-least-once delivery nghĩa là duplicate processing phải được xem là trạng thái bình thường cần thiết kế, không chỉ là exception. Lock renewal giải quyết ownership window; idempotency giải quyết business side-effect correctness.