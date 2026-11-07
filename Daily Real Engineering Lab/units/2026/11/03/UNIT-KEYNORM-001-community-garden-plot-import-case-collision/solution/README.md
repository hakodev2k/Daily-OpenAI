# Reference Solution — spoiler

## Symptoms
Bốn đăng ký nguồn nhưng chỉ ba mục trong index; chủ sở hữu Mai bị thay thế.

## Evidence
`InputRows=4`, `StoredPlots=3`; key `A-17` và `a-17` cùng identity theo `OrdinalIgnoreCase`.

## Root cause
`Dictionary<TKey,TValue>` indexer setter cập nhật giá trị khi key đã tồn tại. Không có exception; xung đột bị xem là cập nhật hợp lệ dù nghiệp vụ đang import đăng ký mới.

## Why the fix works
Định nghĩa comparer theo nghiệp vụ và dùng `TryAdd`; khi xung đột, throw exception trước khi trả kết quả. Chỉ persist dictionary sau khi import toàn bộ batch thành công. Không ghi dần từng row vào database trước bước kiểm tra.

## How to verify
Thay `starter/Program.cs` bằng `solution/Program.cs`, chạy `./verify.ps1` (script vẫn trỏ tới starter, nên copy nội dung sang starter trước). Kiểm tra batch trùng bị reject, batch không trùng giữ đủ ba dòng.

## Alternative fixes
`GroupBy` cùng comparer rồi kiểm tra nhóm trùng trước khi `ToDictionary` có thể cho lỗi tổng hợp đầy đủ. Database unique constraint bổ sung bảo vệ khi có nhiều writer đồng thời.

## Wrong / tempting fixes
Đổi comparer sang `Ordinal` để lưu cả `A-17` và `a-17` chỉ hợp lệ nếu nghiệp vụ thực sự phân biệt hoa/thường. Dùng `Distinct` hay `Last()` chỉ giấu xung đột. Đếm số lượng sau import không đảm bảo dữ liệu được commit đúng.

## Production implications
Chuẩn hóa key tại API, service và database theo cùng một quy tắc; định nghĩa policy cho retry, import cập nhật có chủ đích và conflict response. Batch validation không thay thế atomic transaction khi commit vào database.

## Trade-offs
Reject toàn batch dễ audit và đảm bảo nhất quán, nhưng người dùng phải sửa nguồn. Partial accept phù hợp vài pipeline nếu có report chi tiết, idempotency và chính sách phục hồi rõ ràng.

## Senior engineer should notice
Phân biệt duplicate trong cùng batch với update có version; yêu cầu unique index ở storage để chặn race giữa nhiều import workers; tránh silently overwriting trong cả in-memory lẫn persistence.
