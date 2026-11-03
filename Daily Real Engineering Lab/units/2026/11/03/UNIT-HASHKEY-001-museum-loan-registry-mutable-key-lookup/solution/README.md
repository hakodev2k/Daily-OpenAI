# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms & Evidence
`Dictionary.Count=1` nhưng `TryGetValue` trên cùng object sau khi sửa Year trả false. Trước khi sửa lookup thành công.

## Root cause
`LoanKey.GetHashCode()` phụ thuộc `Year`, là thuộc tính mutable. Dictionary chọn bucket lúc Add, nhưng tra cứu tính hash hiện tại; hash mới không dẫn về bucket cũ. Điều này phá hợp đồng bất biến của khóa khi lưu trong hash table.

## Why the fix works
Tham khảo `Program.cs`: record bất biến làm khóa; khi sửa năm, tạo khóa mới, Remove khóa cũ rồi Add khóa mới cùng trạng thái. Không thay đổi identity của entry đã nằm trong Dictionary.

## Verification
`dotnet run --project solution/LoanRegistry.csproj` phải exit 0, found=True, status=APPROVED, count=1. Đối với code của bạn, chạy `./verify.ps1`, luôn trỏ vào starter. Bổ sung test hai lần sửa năm, sửa về năm ban đầu, và xung đột khóa đã tồn tại.

## Alternatives & trade-offs
Dùng `ArtifactId` bất biến làm khóa, Year chuyển thành value: tốt khi ID thật sự ổn định, nhưng cần xử lý đồng bộ cập nhật value. Remove/Add phù hợp khi năm là một phần của identity, nhưng phải kiểm tra duplicate key trước khi xóa hoặc đảm bảo thao tác dưới lock/transaction phù hợp nếu có concurrent writers.

## Wrong / tempting fixes
Tăng capacity hoặc gọi `TrimExcess` không sửa invariant. Duyệt toàn bộ Dictionary để tìm bằng reference chỉ che lỗi lookup và gây O(n). Chỉ đổi `GetHashCode` thành hằng số sẽ suy giảm hiệu năng và không xử lý đúng equality/identity nghiệp vụ.

## Production implications
Dữ liệu không nhất thiết biến mất; lỗi lookup có thể dẫn tới ghi trùng, sai kiểm kê, hoặc trạng thái phê duyệt giả. Tránh lưu mutable domain entities làm khóa. Với nhiều luồng cập nhật, cần đồng bộ toàn bộ chuỗi kiểm tra–xóa–thêm.

## Senior engineering insight
Phân biệt entity identity với mutable attributes; thiết kế invariant của collection trước khi chọn kiểu khóa. Xác minh cả behavior và tính nhất quán khi business key thay đổi.
