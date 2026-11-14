# Reference solution — Spoiler

## Root cause
`SortedSet<T>` dùng kết quả `IComparer<T>.Compare` bằng 0 để xác định hai phần tử tương đương. So sánh chỉ theo ExpiryDays khiến V-201 và V-202 cùng hạn bị coi như một lô; không có exception. Vì thế COUNT và DOSES bị giảm.

## Fix
So sánh ExpiryDays tăng dần, sau đó BatchId theo `StringComparer.Ordinal` để tạo total order theo identity. Code tham khảo ở `Program.cs`. Chính sách nghiệp vụ cần quyết định trùng BatchId: ở đây xem cùng ID là một lô, không cộng dồn mù quáng.

## Verification
Copy phần comparator sang `starter/Program.cs`, chạy `scripts/verify.ps1`. Bảo đảm 4 ID khác nhau tồn tại, tổng 280 liều, ngày hết hạn tăng dần, trùng ID chỉ một bản ghi.

## Wrong fixes
- Chỉ đổi sang `List<T>`: giữ dữ liệu nhưng không xác định policy trùng ID hoặc thứ tự.
- Cộng thêm Doses cho bản ghi còn lại: làm sai truy xuất nguồn gốc từng BatchId.
- Dùng hash code làm tie-breaker: có collision và không đảm bảo ordering theo identity.

## Trade-offs
Nếu cần nhiều bản ghi cùng BatchId nhưng khác phiên bản, dùng composite identity hoặc tách cấu trúc lưu trữ theo khóa. Nếu cần ưu tiên FIFO trong cùng ngày, dùng arrival sequence thay BatchId và xử lý uniqueness độc lập.
