# Before — starter gốc
- FAST: HTTP 200, body đúng, hoàn tất nhanh.
- SLOW_BODY: HTTP 200, body đúng nhưng elapsed khoảng 1200 ms, vượt budget 250 ms.
- CALLER_CANCEL: caller hủy sau 90 ms nhưng body vẫn hoàn tất sau khoảng 1200 ms.
- `./reproduce.ps1` trả exit code 0 để xác nhận triệu chứng; `./verify.ps1` trả khác 0 trước khi sửa.
- Nếu không reproduce: xác nhận đang dùng starter ban đầu, .NET 8 SDK và body delay trong `harness/Program.cs`.
