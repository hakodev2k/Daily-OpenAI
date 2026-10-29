# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Rolling deployment dừng worker trong lúc render; queue lại ghi nhận job hoàn tất nhưng không có artifact. Khi gateway lưu artifact rồi báo lỗi, hệ thống cũng không thực hiện retry để xác nhận delivery.

## 2. Evidence
Case BADGE-042 kết thúc với `Completed=1` và `Artifacts=0`. Case BADGE-077 có `Completed=1` dù caller nhận `IOException`. Cả hai không còn `Pending` cho lần chạy sau. Dữ liệu là simulator, không phải telemetry thực tế.

## 3. Root cause
`BadgeWorker.HandleOneAsync` đặt `store.Acknowledge(jobId)` trong `finally`. `finally` chạy khi render thành công, bị cancellation hoặc ném exception. Worker đang đánh đồng “đã thoát khỏi handler” với “đã xử lý thành công”. Một delivery không hoàn tất bị loại khỏi queue vĩnh viễn.

## 4. Why the fix works
Xem [BadgeWorker.cs](BadgeWorker.cs). Chỉ acknowledge sau khi `RenderAsync` hoàn tất thành công; nếu lỗi hoặc cancellation, trả delivery về hàng đợi và để caller nhận exception. Trạng thái `Release` trong simulator mô phỏng redelivery của broker. `JobStore.Publish` dùng badge ID làm idempotency key, nên retry sau lỗi receipt không tạo artifact thứ hai.

## 5. How to verify
Thay nội dung `starter/BadgeWorker.cs` bằng ý tưởng của reference (hoặc tự triển khai). Chạy `./verify.ps1` tại root unit. Hai case phải đạt `VERIFICATION_PASSED`. Script kiểm tra code learner, không chạy `solution/`.

## 6. Alternative fixes
- Broker có lease/visibility timeout: không ack khi thất bại; để lease hết hạn và redeliver, đồng thời cân nhắc backoff.
- Explicit negative acknowledgment với delay và dead-letter threshold, nếu broker hỗ trợ và phù hợp contract.
- Persist trạng thái job vào database và dùng transactional outbox cho side effect nội bộ; vẫn cần idempotency ở downstream.

## 7. Wrong / tempting fixes
- Tăng `terminationGracePeriodSeconds` vô hạn: chỉ giảm xác suất bị dừng giữa job, không giải quyết crash hoặc lỗi receipt.
- Catch mọi exception rồi trả thành công: che failure và giữ lỗi mất job.
- Retry bằng cách tạo job ID mới: có thể sinh artifact trùng và phá audit.
- Chỉ sửa cancellation mà không kiểm tra lỗi sau publish: vẫn có cửa sổ ambiguous outcome.

## 8. Production implications
Simulator cố ý không mô phỏng crash giữa publish và ack, lease expiration, concurrent consumers hoặc lỗi network. Với broker thật, bảo đảm at-least-once + stable idempotency key + retry budget + poison-message/DLQ + observability. Cần test rolling shutdown, worker kill và lỗi ack. Không hứa exactly-once delivery xuyên broker và external storage.

## 9. Trade-offs
Release ngay cho retry nhanh nhưng có thể gây hot loop nếu downstream đang lỗi. Delayed retry và DLQ tăng độ ổn định nhưng tăng latency và vận hành. Kéo dài grace period giúp job ngắn hoàn tất nhưng giữ tài nguyên deployment lâu hơn. Nếu render kéo dài hàng phút, checkpoint hoặc chia nhỏ work units có thể hợp lý hơn.

## 10. What a Senior engineer should notice
Phải tách **delivery outcome**, **business side effect**, và **acknowledgment outcome**. “Artifact đã ghi nhưng response lỗi” là ambiguous commit: không thể giải quyết an toàn bằng retry mù nếu side effect không idempotent. Khi chuyển sang queue thật, phải xác định lease owner, crash window, timeout, visibility và retry semantics bằng contract test.
