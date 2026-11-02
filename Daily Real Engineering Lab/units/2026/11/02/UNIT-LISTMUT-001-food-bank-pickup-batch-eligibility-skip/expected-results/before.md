# Trước khi sửa

Chạy `./reproduce.ps1` trên starter nguyên bản.

Dự kiến:

```text
Input:  FB-101, FB-102, FB-103, FB-104
Output: FB-102, FB-103, FB-104
Ineligible in output: FB-102
REPRODUCED: batch eligibility contract violated.
```

Lệnh reproduce trả exit code 0 khi triệu chứng được xác nhận. `./verify.ps1` phải thất bại ở contract có lô không hợp lệ liền nhau; đây là trạng thái mong đợi của starter, không phải kết quả verify thành công.

Nếu không reproduce được, kiểm tra phiên bản source chưa bị chỉnh sửa và chạy `dotnet run --project checks/Checks.csproj -- reproduce` từ thư mục unit.
