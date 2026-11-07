# UNIT-SEC-004 — Archive Import Vượt Khỏi Workspace

## Mục tiêu

Điều tra một import flow xử lý package ZIP từ người dùng và xác định vì sao một file có thể xuất hiện bên ngoài workspace được phép ghi.

## Bối cảnh thực tế

Một CMS nội bộ cho phép khách hàng upload theme package dạng ZIP. Service giải nén package vào thư mục import tạm trước khi validate và publish. Functional test với package bình thường đều pass, nhưng security review phát hiện một package đặc biệt có thể tạo file ở vị trí không thuộc thư mục import.

## Bạn cần làm gì

- Reproduce hành vi bằng package test đã được tạo sẵn bởi starter.
- Quan sát file nào được tạo trong và ngoài import workspace.
- Ghi lại hypothesis trước khi sửa.
- Sửa `starter/ArchiveImporter.cs` để mọi file hợp lệ vẫn được import nhưng entry không được phép ghi ra ngoài destination root.
- Chạy `verify.ps1` để kiểm tra cả security property và functional behavior.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

Không cần Docker, database hoặc cloud account.

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Từ folder của unit:

```powershell
./reproduce.ps1
```

Script tạo package ZIP deterministic, chạy starter importer và xác nhận symptom ban đầu tồn tại.

## Những gì cần quan sát

- File theme hợp lệ vẫn xuất hiện dưới import workspace.
- Một file khác xuất hiện ở vị trí không thuộc workspace đó.
- Importer không cần crash để security boundary bị vi phạm.

Ghi đường dẫn thực tế và hypothesis của bạn vào `workspace/my-investigation.md`.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- `reproduce.ps1` xác nhận một file được tạo ngoài import root.
- File theme hợp lệ vẫn được extract.

**After**

- `verify.ps1` pass.
- File hợp lệ vẫn được extract đúng nội dung.
- Không có file nào từ package được tạo ngoài import root.

Nếu không reproduce được, xóa `.lab-output` rồi chạy lại `./reproduce.ps1`.

## Estimated Time

30–50 phút.
