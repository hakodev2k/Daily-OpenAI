# Hint 3 — gần lời giải
Trong chế độ `ZipArchiveMode.Create`, thông tin thư mục trung tâm của ZIP được hoàn thiện khi `ZipArchive` được dispose. Đảm bảo mảng byte chỉ được tạo **sau** thời điểm đó và stream đích vẫn mở nhờ `leaveOpen: true`.
