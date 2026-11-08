# Reference solution

## Root cause
Client JSON được deserialize trực tiếp thành `VolunteerProfile` là domain model có cả `ReviewStatus`. Giá trị `Approved` do client gửi được giữ nguyên, tạo lỗi mass assignment / overposting.

## Fix
Chỉ deserialize vào `UpdateProfileRequest` gồm tên và số điện thoại. Giữ `Id` và `ReviewStatus` từ bản ghi server. Kiểm tra validation và ownership ở API thật; mọi thay đổi trạng thái cần authorization, transaction và audit.

## Wrong fixes
- Ẩn input ở frontend: attacker vẫn gửi JSON tùy ý.
- Chỉ fallback khi status rỗng: `Approved` vẫn được chấp nhận.
- Tin `Id` client: nguy cơ cập nhật chéo hồ sơ.

## Trade-offs
DTO và mapping tăng một ít code nhưng làm rõ quyền cập nhật. Binding attribute chỉ là defense-in-depth, không thay thế authorization. Với persistence thực, cần concurrency check và kiểm thử HTTP integration.

## Verify
Áp dụng thay đổi vào `starter/Program.cs` rồi chạy `pwsh -File ./scripts/verify.ps1`. Script chạy **starter**, không chạy solution. Có thể kiểm tra tham khảo bằng `dotnet run --project ./solution/VolunteerLab.csproj`.
