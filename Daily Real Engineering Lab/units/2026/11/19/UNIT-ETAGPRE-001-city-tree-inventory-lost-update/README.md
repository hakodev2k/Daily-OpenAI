# UNIT-ETAGPRE-001 — Điều kiện cập nhật bản đồ cây xanh bị bỏ qua

## Mục tiêu
Điều tra mất cập nhật khi hai nhân viên cùng chỉnh sửa một bản ghi; thiết lập hợp đồng HTTP conditional write và kiểm chứng tính toàn vẹn dữ liệu.

## Bối cảnh thực tế
Ứng dụng kiểm kê cây xanh đô thị cho phép đội khảo sát sửa tình trạng cây từ hai thiết bị. Một biên bản hiện trường mới đôi khi biến mất sau khi một thiết bị cũ đồng bộ.

## Bạn cần làm gì
Chạy tình huống hai thiết bị, ghi lại status, version và trạng thái cuối. Đưa ra ít nhất ba giả thuyết; sửa code trong `starter/Program.cs` để một bản cập nhật cũ không thể ghi đè dữ liệu mới.

## Yêu cầu môi trường
PowerShell 7+ và .NET SDK 8.x. Không cần database, Docker hoặc dịch vụ bên ngoài.

## Chạy nhanh
`dotnet run --project starter/TreeInventory.csproj`

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./reproduce.ps1`. Script chỉ thành công khi triệu chứng mất dữ liệu còn tồn tại.

## Những gì cần quan sát
Ghi lại mã phản hồi của hai thao tác, trạng thái cuối và phiên bản bản ghi. So sánh với mong đợi khi thiết bị thứ hai gửi điều kiện dựa trên snapshot cũ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Xem lời giải sau khi tự sửa](solution/README.md).

## Expected Results
Trước sửa: hai cập nhật đều được chấp nhận; kết quả mới bị thay thế. Sau sửa: yêu cầu đầu tiên thành công, yêu cầu dùng snapshot cũ bị từ chối và dữ liệu giữ nguyên. Thử thêm trường hợp thiếu điều kiện và bản ghi không tồn tại.

## Estimated Time
50 phút.
