# Reference solution — chỉ đọc sau khi tự verify

`HttpRequestMessage` không được gửi lại sau khi đã qua `HttpClient.SendAsync`. Ở lần 2, runtime từ chối trước khi handler nhận request. Sửa bằng cách tạo request và `HttpContent` mới cho **mỗi attempt**, giữ `HttpClient` tái sử dụng và dispose response 503.

Đối với POST, việc gửi lại có thể gây side effect lặp nếu response bị mất sau khi server đã xử lý. Demo sử dụng `Idempotency-Key` ổn định, nhưng production cần server thực thi idempotency một cách atomic, lưu kết quả theo key và kiểm tra payload fingerprint. Không retry vô hạn hoặc retry mọi 4xx; cần backoff, jitter, cancellation và timeout budget. Nếu dùng StreamContent, phải tạo stream mới hoặc có cơ chế rewind hợp lệ cho từng attempt.

Chạy tham khảo: `dotnet run --project solution/HeritageUpload.csproj`. Bản này minh họa sửa lỗi; `verify.ps1` cố ý chỉ chạy `starter/` để kiểm tra code người học.

**Wrong fixes:** (1) tạo `HttpClient` mới nhưng giữ request cũ không sửa lỗi; (2) bắt exception rồi trả 200 giả làm mất ảnh; (3) retry POST không idempotency có thể nhân đôi ảnh; (4) dùng `Task.Delay` mà không tạo request mới không giải quyết lifecycle.

