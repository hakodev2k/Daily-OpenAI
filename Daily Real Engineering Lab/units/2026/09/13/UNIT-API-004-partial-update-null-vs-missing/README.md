# UNIT-API-004 — PATCH không phân biệt `null` và field bị bỏ qua

Một Customer Profile API hỗ trợ partial update với contract:

- field không có trong JSON: giữ nguyên giá trị cũ
- field có giá trị `null`: đặt field nullable thành `null`
- field có value: cập nhật value mới

Hiện tại request `{}` và request `{"middleName":null}` tạo ra cùng kết quả. Nhiệm vụ của bạn là reproduce, điều tra serialization boundary, sửa code trong `starter/`, rồi verify mà không làm field bị bỏ qua bị ghi đè.

## Mục tiêu

Phân biệt được **property presence** và **property value** trong partial-update contract.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Hai payload có intent khác nhau nhưng starter xử lý giống nhau.
- Explicit `null` không đạt contract mong đợi.
- Ghi ít nhất ba hypothesis vào `workspace/my-investigation.md`.

## Quy tắc làm lab

1. Reproduce.
2. Ghi hypothesis.
3. Sửa `starter/`.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler — chỉ xem sau khi đã thử fix.

- [Giải thích](solution/README.md)
- [Reference code](solution/Program.cs)

## Expected Results

Before:
- `{}` giữ `middleName = "Quang"`.
- `{"middleName":null}` cũng giữ `middleName = "Quang"` và starter fail.

After:
- `{}` giữ nguyên `middleName`.
- `{"middleName":null}` đặt `middleName = null`.
- `{"middleName":"Minh"}` cập nhật thành `"Minh"`.
- `verify.ps1` exit code `0`.

Nếu script không chạy, dùng:

```powershell
dotnet run --project starter/PartialUpdateLab.csproj
```

## Estimated Time

Khoảng 45 phút.
