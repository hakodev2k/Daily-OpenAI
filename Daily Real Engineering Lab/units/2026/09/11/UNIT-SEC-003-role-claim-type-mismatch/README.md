# UNIT-SEC-003 — Role Claim Type Mismatch

## Mục tiêu

Điều tra một lỗi authorization trong đó token/principal có dữ liệu vai trò quản trị, nhưng ứng dụng vẫn đánh giá người dùng không thuộc role `Admin`.

## Bối cảnh thực tế

Một internal admin tool vừa chuyển sang nhận identity từ một external identity provider. Log cho thấy principal chứa claim biểu diễn vai trò `Admin`, authentication thành công, nhưng các kiểm tra role-based authorization lại từ chối truy cập. Lỗi chỉ xuất hiện sau khi thay nguồn identity; business code không thay đổi.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. So sánh dữ liệu claim thực tế với kết quả `IsInRole("Admin")`.
4. Sửa trực tiếp `starter/Program.cs` để role-based authorization hiểu đúng contract claim hiện tại.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay dịch vụ ngoài

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script xác nhận principal có một claim mang giá trị `Admin`, nhưng kiểm tra role hiện tại vẫn trả về `False`.

## Những gì cần quan sát

- Authentication state là authenticated.
- Có claim chứa giá trị `Admin`.
- `IsInRole("Admin")` trả về `False` ở starter.
- Không có exception bắt buộc phải xuất hiện.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- principal authenticated
- claim `roles=Admin` tồn tại
- `IsInRole("Admin") == false`

After:
- principal vẫn authenticated
- claim dữ liệu không bị mất
- `IsInRole("Admin") == true`
- `verify.ps1` pass

## Estimated Time

30–45 phút.
