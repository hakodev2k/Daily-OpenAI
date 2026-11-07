# Reference Solution — chỉ đọc sau khi tự reproduce và thử sửa

## Symptoms và evidence

File có record cuối sai kiểu dữ liệu: JsonException được ném ra, nhưng store vẫn có GEAR-101 và GEAR-102. Contract yêu cầu reject cả batch mà không thay đổi kho. File hợp lệ vẫn phải import đủ số lượng.

## Root cause

DeserializeAsyncEnumerable<T> trả từng record khi parser có đủ dữ liệu, không đợi toàn bộ JSON array hợp lệ. Starter gọi UpsertAsync ngay trong vòng lặp; hai side effect xảy ra trước khi parser phát hiện record thứ ba không hợp lệ. Đây là lỗi ranh giới commit, không phải lỗi thread scheduling.

## Why the fix works

Reference BatchImporter.cs tách pha đọc và kiểm tra toàn bộ payload khỏi pha công bố. Mọi record phải hợp lệ về JSON và nghiệp vụ, ID không trùng trong batch, số lượng phải dương. Chỉ sau khi đọc xong mới gọi ApplyBatchAsync, primitive copy-on-write trong ManifestStore, để áp dụng toàn bộ thay đổi cùng một lần. Khi parse/validation thất bại, store không bị chạm tới.

## How to verify

Sau khi tự làm, có thể so sánh hoặc chép solution/BatchImporter.cs vào starter/BatchImporter.cs rồi chạy ./verify.ps1. Harness phải báo CONTRACT_PASS; không sửa Program.cs. Dùng ./reproduce.ps1 trên starter ban đầu để quan sát lỗi.

## Alternative fixes

- Với batch nhỏ: staging trong memory, sau đó atomic commit như reference.
- Với batch lớn: staging vào bảng tạm/staging table, validate và merge trong một database transaction; cân nhắc timeout, transaction log, locks và chunking.
- Với workflow cho phép partial success: đổi hợp đồng API để trả kết quả từng record, trạng thái retry và idempotency rõ ràng; không được âm thầm dùng partial success khi hợp đồng yêu cầu all-or-nothing.

## Wrong / tempting fixes

- Bọc mỗi UpsertAsync trong try/catch: không rollback các thay đổi đã có.
- Chỉ kiểm tra JsonException: không bắt được record âm hoặc ID trùng.
- Retry cả batch sau lỗi: có thể nhân đôi side effects nếu store không idempotent.
- Dùng lock quanh vòng lặp: không tạo transaction rollback khi record cuối lỗi.

## Production implications và trade-offs

In-memory staging có O(n) memory; cần giới hạn kích thước, số record và thời gian xử lý. Copy-on-write chỉ là simulator local; database thật cần atomic transaction và concurrency policy. Nếu có external side effects, transactional outbox hoặc thiết kế saga/compensation có thể cần thiết. Kiểm tra cancellation trước commit, duplicate IDs, audit và retry semantics.

## Senior engineer should notice

Một API trả 4xx/validation error không đồng nghĩa hệ thống đã rollback. Phải xác định chính xác ranh giới durable commit, có bằng chứng kiểm thử failure-path và định nghĩa all-or-nothing vs partial-success trước khi chọn công nghệ.

## Reference

- Microsoft Learn: https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializer.deserializeasyncenumerable
- Microsoft Learn: https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken
- Microsoft Learn: https://learn.microsoft.com/dotnet/api/system.io.invaliddataexception
