# Reference solution

`SHA256.HashDataAsync(Stream)` đọc đến EOF; `CopyToAsync` tiếp tục từ EOF nên object rỗng. Với seekable stream, nhớ vị trí ban đầu, hash, rồi đặt lại vị trí trước copy. Với non-seekable stream, dùng bounded buffering hoặc streaming tee.

## Verify
`./verify.ps1` luôn chạy starter người học sửa. `dotnet run --project solution/ArchiveLab.csproj` chạy reference.

## Wrong fixes
- Bỏ checksum làm mất integrity.
- Đặt `Position=0` vô điều kiện gây lỗi non-seekable và sai offset caller.
- Buffer không giới hạn có thể OOM.
- Kiểm tra `Length` thay vì bytes đã upload không xác minh kết quả.

## Trade-offs
Seek+replay tiết kiệm RAM nhưng cần seekable source và nguồn không thay đổi giữa hai lượt. Bounded buffering hỗ trợ forward-only stream nhưng giới hạn kích thước; streaming tee giảm lượt đọc song phức tạp hơn.
