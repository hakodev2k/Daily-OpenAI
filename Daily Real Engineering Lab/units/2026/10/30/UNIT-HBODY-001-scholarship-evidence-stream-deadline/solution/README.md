# Reference Solution — spoiler

**Chỉ xem sau khi đã reproduce, ghi giả thuyết và tự sửa starter.**

## Symptoms
Partner trả headers HTTP 200 ngay nhưng worker vẫn hoàn tất tải hồ sơ sau khoảng 1200 ms, dù request budget là 250 ms. Caller cancel sau 90 ms cũng không dừng body read.

## Evidence
`/fast` và `/slow` có status, payload và handler giống nhau; chỉ khác độ trễ body. Các probe đo từ lúc gọi `FetchAsync` đến khi method trả về hoặc ném cancellation. Không có DNS hay kết nối thật trong harness.

## Root cause
`HttpCompletionOption.ResponseHeadersRead` khiến `HttpClient.SendAsync` hoàn tất khi headers sẵn sàng, trước khi toàn bộ response content được đọc. `HttpClient.Timeout` không bảo đảm timeout của việc đọc body tiếp theo ở mode này. Starter truyền caller token vào `SendAsync`, nhưng gọi `ReadAsStringAsync()` không token và không có cancellation budget xuyên suốt.

## Fix và vì sao hiệu quả
Tham khảo [PartnerAttachmentClient.cs](PartnerAttachmentClient.cs). Tạo `CancellationTokenSource.CreateLinkedTokenSource(callerCancellation)`, đặt `CancelAfter(budget)` trước khi gửi request, rồi truyền **cùng token** vào cả `SendAsync` và `ReadAsStringAsync`. `using` bảo đảm giải phóng request/response và CTS.

Sửa `starter/PartnerAttachmentClient.cs` bằng giải pháp của bạn; harness compile file starter, **không** compile reference source.

## Verification
Chạy `./verify.ps1`: FAST trả payload nguyên vẹn, SLOW_BODY bị cancel trước 1000 ms, CALLER_CANCEL bị cancel trước 650 ms. Nếu timing bất ổn trên máy quá tải, kiểm tra scheduling/SDK và ghi evidence trước khi điều chỉnh bất kỳ threshold nào.

## Alternative fixes
- Dùng `ReadAsStreamAsync(token)` + `CopyToAsync(destination, token)` với bounded destination để hỗ trợ tệp lớn, progress, byte-limit và backpressure.
- Dùng per-operation `CancellationTokenSource` tại orchestration boundary; quan trọng là cùng một end-to-end budget đi qua các pha.
- Với download cực lớn, stream ra file tạm và publish sau khi kiểm tra toàn vẹn thay vì buffer toàn bộ vào string.

## Wrong / tempting fixes
- Tăng `HttpClient.Timeout`: không sửa pha body thiếu deadline trong `ResponseHeadersRead`.
- Bỏ `ResponseHeadersRead`: có thể vô tình khiến `SendAsync` buffer body và áp dụng timeout khác, nhưng thay đổi memory/streaming contract, không phải giải pháp tổng quát.
- Dùng `Task.WhenAny` để trả sớm mà không cancel body: operation nền vẫn có thể chiếm tài nguyên.
- Chỉ cancel request token ở `SendAsync`: không đảm bảo body read sau headers tuân thủ caller cancellation.
- Retry tự động mọi cancellation: có thể nhân tải, che lỗi deadline và tạo side effect nếu endpoint không idempotent.

## Production implications và trade-offs
Ghi riêng `headersLatency`, `bodyLatency`, bytes transferred, `cancelReason` và deadline. Cần cân nhắc streaming, giới hạn kích thước, content integrity, retries có budget chung, idempotency và việc phân biệt timeout với user-initiated cancellation. Với body lớn, `ReadAsStringAsync` tiêu tốn memory và chỉ phù hợp với fixture nhỏ.

## Senior engineer should notice
SLA phải được gắn vào toàn bộ logical operation, không phải một API call. Cancellation là hợp đồng lan truyền qua các boundary; HTTP 200 chỉ là trạng thái giao thức ở pha headers, chưa chứng minh nghiệp vụ đã hoàn thành.
