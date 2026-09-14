# UNIT-COSMOS-001 — Partition Key Fan-out

## Mục tiêu

Điều tra một luồng đọc document theo tenant có kết quả đúng nhưng chi phí truy vấn và số logical partition bị chạm tăng theo số tenant.

## Bối cảnh thực tế

Một backend multi-tenant lưu invoice document theo tenant. API tra cứu invoice hoạt động đúng ở môi trường nhỏ, nhưng khi số tenant tăng thì latency và request charge tăng dù mỗi request chỉ cần đúng một document.

Lab dùng một local simulator để mô phỏng hành vi partition routing, nên không cần Azure subscription.

## Bạn cần làm gì

- Reproduce triệu chứng.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Xác định tại sao một lookup cho một tenant lại chạm nhiều partition.
- Sửa code trong `starter/` để giữ nguyên kết quả nghiệp vụ nhưng giới hạn lookup vào đúng logical partition cần thiết.
- Chạy `verify.ps1`.
- Chỉ sau đó mới xem reference solution.

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

`reproduce.ps1` tạo dữ liệu nhiều tenant, chạy một lookup invoice và xác nhận rằng kết quả đúng nhưng số partition bị chạm lớn hơn 1.

## Những gì cần quan sát

- `FOUND_INVOICE`
- `PARTITIONS_TOUCHED`
- `SIMULATED_REQUEST_UNITS`
- Quan hệ giữa số tenant và chi phí lookup

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
- Tìm đúng invoice.
- Lookup chạm nhiều logical partition.
- Chi phí mô phỏng tăng theo số partition.

After:
- Vẫn tìm đúng invoice.
- Lookup chỉ chạm 1 logical partition.
- Functional behavior không thay đổi.

## Estimated Time

35–50 phút.
