# Investigation guide (không đọc solution trước)

1. Chạy reproduce, lưu output và exit code.
2. Vẽ sequence: caller → relay → HttpClient → partner fake.
3. Tách bạch số lần gọi logic với số request thực sự tới partner.
4. Viết ba giả thuyết: partner luôn 503; request thứ hai bị client từ chối; payload thay đổi giữa các lần gửi.
5. Thiết kế một quan sát có thể bác bỏ từng giả thuyết.
6. Sau khi sửa, chạy verify để xác nhận không retry vô hạn, không retry 400, không mất cancellation.
7. Thảo luận production: retry chỉ an toàn với thao tác có idempotency contract; 429/503 có thể cần Retry-After, backoff, jitter, observability và giới hạn retry budget.

Tài liệu tham khảo nên xem sau khi tự thử: Microsoft Learn về HttpClient.SendAsync, HttpRequestMessage, HTTP status code semantics.
