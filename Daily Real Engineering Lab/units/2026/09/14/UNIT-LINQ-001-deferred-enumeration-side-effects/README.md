# UNIT-LINQ-001 — Deferred Enumeration Side Effects

## Mục tiêu

Điều tra một batch xử lý invoice có kết quả nghiệp vụ đúng nhưng số lần gọi catalog dependency tăng bất thường khi pipeline thực hiện nhiều bước thống kê trên cùng một tập dữ liệu.

## Bối cảnh thực tế

Một worker tạo invoice lines từ danh sách SKU. Mỗi line cần lấy giá từ catalog. Ở production, batch nhỏ chạy ổn nhưng số request tới catalog cao hơn dự kiến và có nguy cơ chạm rate limit khi khối lượng tăng.

## Bạn cần làm gì

- Reproduce hiện tượng.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Xác định vì sao cùng một batch lại gây nhiều dependency call hơn số SKU.
- Sửa `starter/` sao cho kết quả nghiệp vụ không đổi và dependency chỉ bị gọi đúng mức cần thiết.
- Chạy verification.
- Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy trạng thái starter và kiểm tra rằng tổng tiền vẫn đúng nhưng số lần catalog lookup cao hơn số SKU đầu vào.

## Những gì cần quan sát

- Tổng tiền cuối cùng.
- Số SKU đầu vào.
- Số lần `FakeCatalog.GetPrice` được gọi.
- Các vị trí trong pipeline nơi cùng một sequence được đọc.

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

> Spoiler: chỉ xem sau khi bạn đã tự thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Invoice total vẫn đúng.
- Catalog lookup count lớn hơn số SKU.

After:
- Invoice total không đổi.
- Mỗi SKU chỉ cần một catalog lookup cho batch này.
- Verification pass.

## Estimated Time

30–45 phút.
