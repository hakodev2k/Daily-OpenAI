# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Ba cảnh báo được đưa vào worker, nhưng chỉ `AQ-101` gửi thành công. `AQ-102` có lỗi từ đối tác, `AQ-103` không được thử, host dừng sớm.

## 2. Evidence

`attempts=AQ-101,AQ-102`, `published=AQ-101`, `failed=`, `hostStoppedEarly=True` và log `BackgroundService` cho thấy lỗi của một lần gửi đã vượt qua ranh giới vòng đời worker. Dữ liệu `AQ-103` không lỗi; nó chưa được thực thi.

## 3. Root cause

`FakeGateway.PublishAsync` ném `PartnerGatewayException` cho `AQ-102`. Starter không xử lý exception trong vòng lặp `NoticeWorker.ExecuteAsync`, nên task nền kết thúc với lỗi. Theo mặc định, .NET Generic Host dùng `BackgroundServiceExceptionBehavior.StopHost`: một unhandled exception từ `BackgroundService` yêu cầu dừng host. Đây là lựa chọn fail-fast hợp lý cho lỗi không dự kiến, nhưng lỗi gateway được phân loại có thể cần xử lý riêng ở cấp item.

## 4. Why the fix works

[Xem code tham chiếu](NoticeWorker.cs). `try/catch` được đặt quanh từng lần gửi; chỉ `PartnerGatewayException` khi chưa shutdown mới được ghi vào `Failed`, sau đó vòng lặp tiếp tục. `OperationCanceledException` do shutdown và các exception không biết trước không bị nuốt. Chỉ thay `starter/NoticeWorker.cs`, không sửa harness.

## 5. How to verify

Chạy `./verify.ps1`. Harness biên dịch **starter**, không tham chiếu `solution/`. Pass khi cả ba lần gửi đã được thử, `AQ-101/AQ-103` thành công, `AQ-102` có failure record và host chưa dừng tại mốc quan sát.

## 6. Alternative fixes

- Retry có giới hạn và backoff theo loại lỗi, sau đó ghi vào durable retry queue hoặc dead-letter store; cần budget, idempotency và telemetry.
- Tách nhóm đối tác thành các worker/process có failure domain độc lập khi SLA và blast radius yêu cầu.
- Fail-fast toàn host vẫn có thể đúng nếu lỗi cho thấy invariant hoặc dữ liệu quan trọng bị hỏng; supervisor/orchestrator phải có chính sách restart.

## 7. Wrong / tempting fixes

- Đặt `HostOptions.BackgroundServiceExceptionBehavior = Ignore` toàn cục: host không dừng nhưng **worker đã chết không tự restart**; `AQ-103` vẫn không được gửi.
- Bọc toàn bộ `ExecuteAsync` bằng một catch rồi return: che lỗi nhưng bỏ phần việc còn lại.
- Catch `Exception` vô điều kiện rồi tiếp tục: có thể che lỗi lập trình, corruption hoặc shutdown.
- Tăng số replica trước khi xác định failure boundary: có thể tạo thêm bản sao xử lý và chi phí mà không sửa logic.

## 8. Production implications

`DeliveryJournal` chỉ là mô hình in-memory cho lab; production phải persist trạng thái thất bại và có cơ chế retry/dead-letter đáng tin cậy. Không được coi ghi log là bảo đảm giao hàng. Cần metric số item thất bại, tuổi hàng đợi, alert theo SLO, và xử lý cancellation có kiểm soát.

## 9. Trade-offs

Cô lập lỗi theo item tăng khả năng tiếp tục phục vụ nhưng có thể khiến backlog lỗi tích tụ; fail-fast toàn host làm sự cố hiển thị rõ nhưng tăng blast radius. Quyết định phụ thuộc tính độc lập của item, retry budget, ordering, data integrity và khả năng recovery.

## 10. What a Senior engineer should notice

Phải phân biệt **host liveness** với **worker progress**. Một host còn sống không có nghĩa tất cả background tasks đang xử lý. Bài test cần xác minh cả tiến độ, bản ghi thất bại và host lifetime, không chỉ exit code. Chính sách catch/retry phải dựa trên failure classification và delivery contract.

Tài liệu chính thức: [Worker Services](https://learn.microsoft.com/dotnet/core/extensions/workers), [Generic Host](https://learn.microsoft.com/dotnet/core/extensions/generic-host).
