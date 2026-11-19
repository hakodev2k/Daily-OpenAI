# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Hai client đọc phiên bản 4; client B ghi đè khảo sát mới của A.

## Evidence
Replay ban đầu in `first=200 second=200 final=healthy version=6`, trong khi client B vẫn gửi `"v4"`.

## Root cause
Đường cập nhật nhận `ifMatch` nhưng không thực thi điều kiện phiên bản trước khi ghi.

## Why the fix works
So sánh strong ETag với version hiện hành; thiếu điều kiện trả 428, điều kiện cũ trả 412. Chỉ cập nhật sau khi điều kiện đúng. Mẫu code trong `Program.cs` là bản so sánh tối thiểu.

## How to verify
Thay `starter/Program.cs` bằng cách sửa của bạn và chạy `verify.ps1`. Thử thêm thiếu header, ETag yếu và bản ghi không tồn tại.

## Alternative fixes
SQL `UPDATE ... WHERE Id=@id AND Version=@expected` và kiểm tra affected rows là lựa chọn tốt khi có nhiều process. SQL Server `rowversion` hoặc EF Core concurrency token có thể thực thi điều kiện ở DB.

## Wrong or misleading fixes
Chỉ khóa một process bằng `lock` không đủ khi triển khai nhiều replicas. Tự động retry với dữ liệu cũ có thể tiếp tục ghi đè ý định của người khác. Chỉ kiểm tra ở controller rồi ghi sau đó vẫn có race.

## Production implications
Map ETag đúng với version thực tế, kiểm soát authorization trước khi tiết lộ dữ liệu, ghi telemetry cho 412/428 và test đồng thời ở storage boundary.

## Trade-offs
ETag giúp client xử lý xung đột nhưng đòi UX merge/reload; dùng DB conditional update để đảm bảo atomicity giữa nhiều instance.

## What a Senior engineer should notice
Bản demo in-memory chỉ mô phỏng tuần tự; cần kiểm thử hai writer thật với database và xác nhận 0 hoặc 1 writer thắng tại cùng phiên bản. Không xem bản demo như bằng chứng an toàn đa instance.

Tham khảo: RFC 9110, mục 13.1.1 và 15.5.13; RFC 6585 mục 3.
