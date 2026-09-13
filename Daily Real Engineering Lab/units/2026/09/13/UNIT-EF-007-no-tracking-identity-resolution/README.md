# UNIT-EF-007 — Read-only query tạo nhiều object cho cùng một entity

## Mục tiêu

Điều tra một read-only EF Core query trả về dữ liệu đúng về mặt giá trị nhưng tạo nhiều CLR object cho cùng một database entity, sau đó sửa mà không bật tracking cho toàn bộ query.

## Bối cảnh thực tế

Dashboard vận hành shipment đọc nhiều đơn hàng cùng thuộc một customer. API không update dữ liệu nên team đã tối ưu query theo hướng no-tracking. Sau thay đổi này, một bước downstream dựa trên object identity phát hiện nhiều instance khác nhau cho cùng `CustomerId`, đồng thời allocation tăng hơn kỳ vọng khi batch lớn.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi ít nhất 3 hypotheses vào `workspace/my-investigation.md`.
3. Kiểm tra query/materialization behavior.
4. Sửa `starter/Program.cs` để vẫn giữ read-only semantics nhưng cùng database entity không bị materialize thành nhiều CLR instances trong một query result.
5. Chạy `scripts/verify.ps1`.
6. Sau đó mới xem `solution/`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Internet cho lần restore NuGet đầu tiên

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-EF-007-no-tracking-identity-resolution"
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`scripts/reproduce.ps1` chạy starter ở trạng thái ban đầu. Script chỉ pass khi triệu chứng dự kiến thực sự xuất hiện.

## Những gì cần quan sát

- Các order đều trỏ tới cùng `CustomerId`.
- Số CLR object đại diện cho customer lớn hơn số database customer tương ứng trong result.
- Functional values vẫn đúng, nên bug dễ bị bỏ qua nếu chỉ assert dữ liệu field-by-field.

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

> Spoiler — chỉ xem sau khi đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

**Before:** 3 orders cùng `CustomerId`, nhưng có nhiều CLR customer instances; starter exit code khác 0.

**After:** dữ liệu vẫn read-only, 3 orders vẫn đúng, nhưng cùng customer chỉ có một CLR instance trong result; verify exit code 0.

Nếu không reproduce được, xóa `bin/` và `obj/`, chạy `dotnet restore` rồi chạy lại script.

## Estimated Time

Khoảng 40 phút.
