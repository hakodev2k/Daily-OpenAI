# UNIT-ETAG-001 — Hồ sơ lưu trữ bị ghi đè bởi phiên biên tập cũ

## Mục tiêu
Điều tra một sự cố lost update, chứng minh tác động bằng replay xác định và thiết kế hợp đồng cập nhật có kiểm soát xung đột.

## Bối cảnh thực tế
Hai nhân viên đồng thời cập nhật manifest của một bộ sưu tập ảnh lưu trữ. Cả hai giao diện đều báo lưu thành công, nhưng ghi chú của người lưu trước biến mất. Audit log không có lỗi hệ thống.

## Bạn cần làm gì
Chạy mô phỏng, ghi lại hai phiên đọc và thứ tự ghi; nêu ít nhất ba giả thuyết; sửa đường cập nhật trong `starter/Program.cs` để không chấp nhận ghi đè im lặng; giữ hành vi cập nhật bình thường.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần database hay cloud.

## Chạy nhanh
Từ thư mục unit: `dotnet run --project starter/ArchiveManifest.csproj`

## Cách reproduce vấn đề
`./reproduce.ps1` (exit 0 khi chứng minh được triệu chứng; không đồng nghĩa đã sửa).

## Những gì cần quan sát
Hai phiên đọc cùng revision, cả hai lệnh lưu báo thành công, nhưng nội dung cuối cùng không phản ánh đầy đủ lịch sử thao tác. Phân biệt log HTTP thành công và tính đúng đắn nghiệp vụ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md) chỉ xem sau khi thử sửa.

## Expected Results
Trước: hai lần lưu trả SUCCESS, mất thay đổi của phiên trước. Sau: phiên dùng dữ liệu cũ bị CONFLICT; phiên đọc lại revision mới có thể cập nhật; không có partial write.

## Estimated Time
60–75 phút (L3 / D4).
