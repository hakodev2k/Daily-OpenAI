# UNIT-LINQ-005 — Membership Scan Amplification

## Mục tiêu

Điều tra một batch reconciliation có kết quả đúng nhưng lượng comparison tăng rất nhanh khi dữ liệu lớn hơn, dù không có database hay network bottleneck.

## Bối cảnh thực tế

Một worker nhận danh sách SKU được phép xử lý và lọc 2.000 product records trước khi đồng bộ. Batch nhỏ chạy nhanh, nhưng khi số SKU tăng, CPU time tăng không tương xứng. Lab dùng một key type có bộ đếm equality comparison để tạo evidence deterministic thay vì dựa vào timing máy.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Xác định vì sao số comparison tăng mạnh dù output vẫn đúng.
4. Sửa code trong `starter/` để giữ nguyên kết quả nghiệp vụ nhưng giảm đáng kể lượng comparison.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` build và chạy starter, sau đó xác nhận:

- output có đúng 1.000 product được chọn;
- số equality comparisons vượt xa số records đầu vào.

## Những gì cần quan sát

- `Matched`
- `Comparisons`
- collection nào bị đọc lặp lại trong membership check
- complexity thay đổi thế nào khi cả hai collection cùng tăng

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

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- `Matched=1000`.
- số comparisons ở mức hàng trăm nghìn hoặc cao hơn.

After:
- `Matched=1000` không đổi.
- số comparisons giảm xuống dưới ngưỡng verification.

## Estimated Time

35–50 phút.
