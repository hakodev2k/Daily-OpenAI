# UNIT-TEST-010 — Test Suite Passes Alone, Fails Together

## Mục tiêu

Điều tra một test suite có kết quả phụ thuộc cách chạy: từng test riêng lẻ đều có vẻ ổn nhưng chạy theo suite lại xuất hiện failure có thể reproduce.

## Bối cảnh thực tế

Một thư viện pricing được dùng bởi nhiều service. CI bắt đầu báo một test format giá thất bại sau khi nhóm thêm test cho thị trường châu Âu. Developer chạy riêng test bị đỏ thì nó lại pass. Production code không đổi.

## Bạn cần làm gì

1. Chạy starter suite theo hướng dẫn.
2. Reproduce khác biệt giữa isolated run và ordered suite run.
3. Ghi ít nhất 3 hypotheses trước khi sửa.
4. Thu thập evidence từ output của test process và source test.
5. Sửa code trong `starter/` để test suite độc lập và deterministic.
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

Script chạy cùng behavior theo hai phạm vi khác nhau và xác nhận symptom mong đợi xuất hiện.

## Những gì cần quan sát

- Test nào pass khi chạy riêng.
- Test nào fail khi chạy sau test khác.
- Giá trị process-level nào khác nhau giữa hai thời điểm.
- Failure có liên quan timing hay có thể reproduce theo thứ tự cố định.

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

Before:
- isolated formatting check pass
- ordered suite run tạo một failure deterministic

After:
- cùng các checks pass bất kể test trước đó đã chạy
- test vẫn kiểm tra đúng behavior theo locale mong muốn

Nếu không reproduce được, chạy `dotnet --version`, sau đó chạy trực tiếp `dotnet run --project starter -- reproduce` và kiểm tra output từng bước.

## Estimated Time

Khoảng 55 phút.