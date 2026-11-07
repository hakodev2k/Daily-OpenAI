# UNIT-TEST-007 — Green Integration Test, Broken Database Contract

## Mục tiêu
Điều tra vì sao một integration test cho luồng đăng ký email luôn xanh nhưng cùng dữ liệu lại có thể thất bại khi chạy trên relational database.

## Bối cảnh thực tế
Một internal identity service dùng EF Core. Team vừa thêm unique constraint cho `NormalizedEmail`. Test hiện tại tạo hai user có cùng email chuẩn hóa và vẫn báo rằng persistence flow hoạt động bình thường. Production lại ghi nhận lỗi khi request tương tự xảy ra.

## Bạn cần làm gì
1. Chạy starter và xác nhận test hiện tại xanh.
2. Ghi hypothesis trước khi sửa.
3. Kiểm tra test environment có thực sự mô phỏng database contract cần kiểm chứng hay không.
4. Sửa test harness để regression test phát hiện duplicate email ở persistence boundary.
5. Chạy `verify.ps1`.
6. Sau đó mới xem `solution/`.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell
- Không cần SQL Server, Docker hoặc cloud service.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter test project nguyên bản. Lab được reproduce thành công khi test suite xanh dù scenario cố lưu hai record có cùng `NormalizedEmail`.

## Những gì cần quan sát
- Test process có exception hay không.
- Số record được lưu sau hai lần `SaveChangesAsync`.
- Model khai báo constraint gì cho `NormalizedEmail`.
- Test database provider nào đang thực thi persistence behavior.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
> Spoiler: chỉ xem sau khi đã thử sửa.

[Reference Solution](solution/README.md)

## Expected Results
**Before:** starter test xanh và lưu được 2 rows trong scenario duplicate.

**After:** regression test dùng persistence environment có relational semantics và chứng minh lần ghi duplicate bị database từ chối; normal insert vẫn hoạt động.

Nếu không reproduce được, chạy `dotnet --info`, sau đó `dotnet restore starter/ProviderFidelity.Tests.csproj` và chạy lại.

## Estimated Time
35–50 phút.