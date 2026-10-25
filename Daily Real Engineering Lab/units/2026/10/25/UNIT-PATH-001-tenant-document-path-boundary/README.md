# UNIT-PATH-001 — Báo cáo tenant bị đọc nhầm qua ranh giới thư mục

## Mục tiêu
Điều tra một incident truy cập tài liệu trong hệ thống đa tenant: yêu cầu hợp lệ vẫn hoạt động, nhưng một đường dẫn khác trả về tài liệu nằm ngoài khu vực dữ liệu được cấp quyền. Thu thập bằng chứng, phân biệt lỗi authorization với lỗi lưu trữ, sửa logic kiểm tra ranh giới và kiểm thử hồi quy.

## Bối cảnh thực tế
API Compliance Documents ánh xạ mỗi tenant vào một thư mục dữ liệu riêng. Sau khi đội vận hành thêm thư mục lưu trữ dự phòng, cảnh báo DLP ghi nhận một request tải được tài liệu không thuộc tenant đang đăng nhập. Không có thay đổi token, vai trò hoặc quyền trên endpoint. Dữ liệu trong lab là fixture giả, hoàn toàn cục bộ.

## Bạn cần làm gì
1. Chạy baseline và quan sát ba case: tài liệu hợp lệ, thư mục có tên gần giống, thư mục không liên quan.
2. Viết ít nhất ba hypothesis và xác định bằng chứng nào phân biệt chúng.
3. Sửa **starter/PathPolicy.cs** để chỉ cho phép đường dẫn tương đối nằm trong thư mục tenant.
4. Chạy **verify.ps1** trên starter đã sửa. Ghi lại kết quả và đánh giá rủi ro còn lại với symbolic link và quyền hệ điều hành.

## Yêu cầu môi trường
- .NET SDK 8; PowerShell 5.1+ hoặc PowerShell 7 (hoặc dùng lệnh dotnet trực tiếp).
- Không cần database, Docker, Azure, secret, hay NuGet package ngoài SDK.
- Fixture được tạo trong thư mục tạm và xóa sau mỗi lần chạy.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
./verify.ps1
```
Trên Linux/macOS: `dotnet run --project ./baseline/PathBoundaryLab.Baseline.csproj -- --reproduce`, rồi `dotnet run --project ./starter/PathBoundaryLab.csproj -- --verify`.

## Cách reproduce vấn đề
`./reproduce.ps1` chạy **baseline bất biến**, thành công (exit 0) khi tái hiện được incident. `./verify.ps1` chạy **starter có thể chỉnh sửa**, ban đầu phải thất bại (exit 1) và chỉ thành công sau khi bạn sửa. Hai script có mục đích khác nhau.

## Những gì cần quan sát
- Tài liệu hợp lệ có được đọc thành công không?
- Kết quả của hai đường dẫn nằm ngoài phạm vi tenant khác nhau thế nào?
- Khi xem `evidence/incident-log.md`, giả thuyết nào được ủng hộ, giả thuyết nào chưa có bằng chứng?
- Khi sửa, behavior hợp lệ có bị từ chối nhầm không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự thử:** [Lời giải, giới hạn và trade-offs](solution/README.md).

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
70 phút.
