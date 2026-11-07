# Reference solution (spoiler)

**Root cause:** publisher ghi đè trực tiếp key `map/north` và `map/south`, nên reader có thể thấy trạng thái trung gian giữa hai lần ghi. Việc `SetCurrent` ở cuối không giúp gì nếu reader không dùng release khi chọn object.

**Fix:** lưu tile bất biến theo `map/{release}/{cell}`. Chỉ chuyển `CurrentRelease` sau khi mọi tile của release mới tồn tại và đã được xác thực. Reader đọc `CurrentRelease` một lần rồi truy xuất cả north/south từ cùng namespace. File `solution/Program.cs` minh họa đường đi này.

**Verification:** copy code solution sang starter, chạy `pwsh ./verify.ps1`; checkpoint phải đọc toàn r1 và cuối cùng toàn r2.

**Wrong fixes:** (1) thêm delay giữa hai tile chỉ làm rộng cửa sổ lỗi; (2) retry từng tile không bảo đảm chung snapshot; (3) lock trong một process không bảo vệ nhiều publisher/reader khác process; (4) clear CDN không tạo atomicity ở origin.

**Trade-offs:** lưu thêm bản release tăng chi phí storage nhưng hỗ trợ rollback; pointer cần CAS khi nhiều publisher; cần garbage collection chỉ sau khi không còn reader/CDN tham chiếu bản cũ; manifest validation phải kiểm tra checksum và completeness trước cutover.
