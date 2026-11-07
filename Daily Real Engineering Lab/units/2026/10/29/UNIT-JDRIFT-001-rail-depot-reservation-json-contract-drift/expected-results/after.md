# After — learner code đã sửa

- `./verify.ps1` trả exit code **0**, tất cả **8/8** contract tests PASS.
- v1 hợp lệ và v1 có `reservedUnits=0` tường minh được chấp nhận.
- Missing field, renamed field không hỗ trợ, negative units, unknown field, version 2 và empty reservation ID bị từ chối rõ ràng.
- Không có trường hợp nào âm thầm biến dữ liệu không hợp lệ thành phiếu giữ 0 units.
- `./reproduce.ps1` không còn tái hiện triệu chứng ban đầu (exit code khác 0 là bình thường sau khi sửa).

Nếu không build: kiểm tra `dotnet --info`, target `net8.0`, chạy từ thư mục unit, và bảo đảm chỉ có một định nghĩa `ReservationPayload` trong project.
