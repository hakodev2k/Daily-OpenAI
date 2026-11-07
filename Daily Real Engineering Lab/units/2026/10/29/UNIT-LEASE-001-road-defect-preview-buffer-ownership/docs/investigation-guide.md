# Investigation Guide — không đọc solution trước

1. Ghi lại giá trị A ngay sau Prepare(A), sau Prepare(B) và giá trị B.
2. Tách giả thuyết lỗi biến đổi dữ liệu nguồn, lỗi truyền qua consumer, và lỗi thời gian sống của vùng nhớ.
3. Đọc interface ArrayPool<byte>, ReadOnlyMemory<byte> và đường đi từ service sang consumer. Không thay đổi RecyclingArrayPool để làm test xanh.
4. Viết một invariant: khi Prepare trả về, kết quả phải độc lập với công việc tiếp theo cho đến khi consumer đọc xong.
5. Chỉnh starter/PreviewService.cs, chạy verify và giải thích vì sao test thực sự kiểm tra invariant.
6. Sau khi verify, mở solution và đánh giá lựa chọn copy-out so với chuyển ownership bằng IMemoryOwner<byte>.

Tài liệu chính thức để tra cứu sau khi tự thử:
- https://learn.microsoft.com/dotnet/api/system.buffers.arraypool-1
- https://learn.microsoft.com/dotnet/api/system.readonlymemory-1
- https://learn.microsoft.com/dotnet/api/system.buffers.imemoryowner-1
