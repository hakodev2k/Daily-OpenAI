# UNIT-API-008 — Partial Content Range Contract

## Mục tiêu

Điều tra một download endpoint hỗ trợ resume nhưng trả metadata HTTP không nhất quán ở một số request gần cuối file.

## Bối cảnh thực tế

Một hệ thống tài liệu nội bộ cho phép client tải tiếp file bị gián đoạn bằng `Range`. Tải toàn bộ hoạt động bình thường. Một số client resume gần cuối file lại từ chối response dù body vẫn có dữ liệu.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce request được cung cấp.
3. So sánh request, response headers và số byte thực tế.
4. Ghi hypothesis trước khi sửa.
5. Sửa `starter/Program.cs`.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- requested byte interval
- resource length
- response status
- `Content-Range`
- body length thực tế
- sự nhất quán giữa metadata và payload

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

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

Before: response metadata và số byte thực tế không mô tả cùng một interval cho case reproduce.

After: mọi case trong verification có response interval hợp lệ, body length khớp metadata, và full download không regression.

Nếu không reproduce được, kiểm tra `dotnet --version`, sau đó chạy trực tiếp `dotnet run --project starter -- --reproduce`.

## Estimated Time

Khoảng 35 phút.