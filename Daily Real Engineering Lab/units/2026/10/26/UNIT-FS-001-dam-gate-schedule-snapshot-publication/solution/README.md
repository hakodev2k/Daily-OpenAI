# Reference Solution — spoiler

## Symptoms
Console đôi lúc đọc JSON bị cắt giữa hai lần publish hợp lệ.

## Evidence
Checkpoint cố định chứng minh `DURING` không thuộc bản trước hay sau, và không parse được.

## Root cause
Publisher mở chính file đích với `FileMode.Create`, truncate bản đang được phục vụ rồi mới ghi từng phần. `FlushAsync` không tạo publication boundary. Callback lỗi cũng để file đích bị thay đổi.

## Why the fix works
Ghi trọn payload vào staging file cùng thư mục, đóng staging, sau đó dùng `File.Move(staging, destinationPath, overwrite: true)` để công bố. Nếu callback lỗi, `finally` xóa staging, file cũ còn nguyên.

## How to verify
`./verify.ps1` luôn chạy code người học ở `starter/`. Tự kiểm tra reference riêng:
```powershell
dotnet run --project solution/ReferenceHarness.csproj -- verify
```
Kiểm tra cả snapshot đọc tại checkpoint lẫn callback ném exception.

## Alternative fixes
`File.Replace` với backup khi file đích tồn tại; immutable versioned files + manifest pointer; database transaction nếu cần consistency đa bản ghi.

## Wrong / tempting fixes
`FlushAsync` thêm không ngăn reader thấy bản dở. `Task.Delay` và retry parse chỉ che triệu chứng. `lock` trong một process không đồng bộ reader ở process khác.

## Production implications
Same-filesystem rename/replace thường cung cấp atomic visibility cho reader mở mới nhưng không tự đảm bảo crash durability, multi-file transaction hay multi-writer isolation. Windows open handles thiếu delete-sharing có thể cản trở replace; network filesystem có semantics riêng. Phải thử nghiệm trên hệ thống thật, thêm retry có giới hạn và cơ chế fsync nếu cần power-loss durability.

## Trade-offs
Staging tăng I/O và dung lượng tạm nhưng giảm rủi ro đọc dở. Nhiều writer cần version control/serialization; DB không mặc nhiên là lựa chọn tốt nhất.

## What a Senior engineer should notice
Phân biệt prepare/commit, visibility/durability, normal/failure path, và nêu rõ giả định một writer, local filesystem, reader mở lại mỗi poll.
