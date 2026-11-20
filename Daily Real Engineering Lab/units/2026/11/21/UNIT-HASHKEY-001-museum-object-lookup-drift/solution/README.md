# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
`count=1` nhưng lookup bằng key mới thất bại; enumeration vẫn thấy object.

## Evidence
Cùng một instance được lưu làm key, rồi thuộc tính tham gia `Equals/GetHashCode` thay đổi. Bucket được chọn khi insert không được cập nhật khi object thay đổi.

## Root cause
`Dictionary<TKey,TValue>` dựa vào hash/equality ổn định của key trong thời gian lưu trữ. Key bị mutate sau insertion khiến lookup dùng hash mới tìm sai bucket.

## Why the fix works
Sử dụng `record` bất biến làm key, remove bằng key cũ trước khi tạo key mới và insert lại. Có thể dùng mã accession bất biến làm key và lưu hall trong value để tránh di chuyển identity.

## How to verify
Từ unit root chạy `./verify.ps1` sau khi sửa `starter/Program.cs`; so sánh với `dotnet run --project solution/ExhibitLookup.csproj`.

## Alternative fixes
Chọn `Code` bất biến làm key nếu duy nhất toàn hệ thống; lưu `Hall` như thuộc tính value. Nếu cần composite identity, remove/reinsert dưới lock hoặc transaction tương ứng.

## Wrong / tempting fixes
Duyệt `Keys` để tìm đúng object che lỗi và biến lookup O(1) thành O(n). Gọi `TrimExcess` hay đổi capacity không giải quyết hợp đồng key. Tạo `Dictionary` mới sau mỗi update chỉ che triệu chứng, tăng chi phí và không đảm bảo atomicity.

## Production implications
Concurrent relocation cần serialize mutations, bảo vệ readers, và dùng authoritative storage nếu nhiều instance. Dictionary không thread-safe cho concurrent writes.

## Trade-offs
Composite key phản ánh location nhưng làm relocation đổi identity; accession ID ổn định giúp model đơn giản hơn nếu nghiệp vụ cho phép.

## Senior engineer should notice
Audit enumeration không chứng minh khả năng lookup; thêm invariant tests cho cả before/after và xem xét API identity, concurrency, dữ liệu lịch sử.
