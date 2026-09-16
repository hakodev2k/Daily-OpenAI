# UNIT-API-006 — Refactor API Contract Without Breaking Clients

## Mục tiêu

Modernize một API đang trả trực tiếp persistence model, trong khi giữ nguyên public JSON contract đã được client tích hợp.

## Bối cảnh thực tế

Team cần đổi tên và tổ chức lại field nội bộ của `CustomerRecord` để phù hợp schema mới. Một thay đổi tưởng như chỉ là refactor backend khiến contract test của client thất bại dù dữ liệu nghiệp vụ vẫn đủ.

## Bạn cần làm gì

1. Chạy starter và reproduce contract regression.
2. Ghi hypothesis về boundary nào đang khiến thay đổi nội bộ lan ra public API.
3. Refactor code trong `starter/` để persistence model có thể thay đổi độc lập nhưng response contract hiện tại vẫn giữ nguyên.
4. Không sửa expected contract để làm test pass.
5. Chạy `verify.ps1` trên code learner-editable.
6. Sau đó mới đọc solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter ở trạng thái ban đầu và xác nhận serialized response không còn khớp contract mà client đang dùng.

## Những gì cần quan sát

- JSON property names thực tế.
- Shape của object được serialize tại API boundary.
- Backend refactor nào đang làm thay đổi output mà client nhìn thấy.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử refactor.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

Before: contract check fail vì public JSON shape thay đổi theo persistence model.

After: persistence model giữ naming mới nhưng serialized API response vẫn đúng contract cũ; verification pass.

## Estimated Time

45–60 phút.