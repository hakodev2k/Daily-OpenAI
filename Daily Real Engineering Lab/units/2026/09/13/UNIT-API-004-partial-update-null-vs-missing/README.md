# UNIT-API-004 — PATCH không phân biệt `null` và field bị bỏ qua

Một API cấu hình hỗ trợ partial update với contract:

- field không có trong JSON: giữ nguyên giá trị cũ
- field có giá trị `null`: đặt field nullable thành `null`
- field có value: cập nhật value mới

Starter dùng field `tag`. Hiện tại request `{}` và request `{"tag":null}` bị collapse thành cùng một CLR state. Nhiệm vụ là reproduce, thu thập evidence, sửa code trong `starter/`, rồi verify mà không làm missing field vô tình ghi đè dữ liệu.

## Mục tiêu

Phân biệt **property presence** và **property value** trong partial-update contract.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Script setup tạo project .NET 8 từ source trong `starter/`, sau đó chạy ba case: missing, explicit-null và new-value.

## Những gì cần quan sát

- Hai payload có intent khác nhau nhưng starter xử lý giống nhau ở case explicit-null.
- Missing phải giữ `tag = "blue"`.
- Explicit null phải tạo `tag = null`.
- New value phải tạo `tag = "green"`.
- Ghi ít nhất ba hypothesis vào `workspace/my-investigation.md`.

## Quy tắc làm lab

1. Reproduce.
2. Ghi hypothesis và evidence.
3. Sửa `starter/Program.cs`.
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
- missing: PASS
- explicit-null: FAIL
- new-value: PASS
- starter exit code `1`; `reproduce.ps1` coi đây là reproduction thành công

After:
- cả ba case PASS
- `verify.ps1` exit code `0`

Nếu script setup gặp lỗi, kiểm tra `dotnet --version` phải hỗ trợ .NET 8 và chạy lại `./reproduce.ps1`.

## Estimated Time

Khoảng 45 phút.
