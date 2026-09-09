# UNIT-DOTNET-005 — Upload Passes Validation but Stores an Empty File

## Mục tiêu

Điều tra một upload pipeline .NET trong đó cùng một input stream được dùng qua nhiều bước xử lý, xác định vì sao validation vẫn thành công nhưng dữ liệu được lưu không đúng, rồi sửa lifecycle của stream mà không bỏ bước kiểm tra integrity.

## Bối cảnh thực tế

Một document service nhận file, tính SHA-256 để ghi audit metadata, sau đó chuyển cùng stream cho storage adapter. API trả về thành công và hash được ghi đúng, nhưng một số file lưu xuống storage có kích thước `0 bytes`.

Business impact: document được đánh dấu là uploaded nhưng không thể tải lại, trong khi log validation không báo lỗi.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại `Length`, `Position` tại từng boundary.
3. Hình thành ít nhất hai hypothesis trước khi sửa.
4. Sửa code trong `starter/` để storage nhận đúng toàn bộ payload.
5. Giữ nguyên bước SHA-256 validation.
6. Chạy `verify.ps1` để kiểm tra cả content lẫn hash.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell
- Không cần Azure subscription, database hay Docker

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script build starter rồi chạy scenario với một document payload cố định. Một reproduction hợp lệ phải cho thấy:

- input ban đầu có dữ liệu
- integrity check tạo được SHA-256
- storage adapter nhận số byte khác với payload ban đầu

## Những gì cần quan sát

- `Length` của stream trước và sau mỗi bước
- `Position` của stream trước và sau mỗi bước
- số byte storage adapter thực sự copy
- hash của payload gốc và payload đã lưu

README chỉ mô tả evidence cần thu thập; không giả định nguyên nhân trước khi chạy.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- hash validation thành công
- storage adapter không lưu đủ payload
- process không cần exception để tạo ra dữ liệu sai

Sau khi sửa:

- SHA-256 validation vẫn chạy
- số byte lưu bằng số byte input
- stored content bằng input content
- verify pass mà không bỏ integrity check

## Estimated Time

25–40 phút.
