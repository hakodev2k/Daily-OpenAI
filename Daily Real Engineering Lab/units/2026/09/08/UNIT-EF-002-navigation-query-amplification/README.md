# UNIT-EF-002 — Trang navigation nhanh ở dev nhưng chậm dần theo số menu

## Mục tiêu

Điều tra một luồng đọc dữ liệu bằng EF Core nơi kết quả chức năng vẫn đúng nhưng số database command tăng theo số navigation item được render.

## Bối cảnh thực tế

Một CMS-backed storefront dựng navigation từ các section đang active. Ở dữ liệu dev chỉ có vài section nên response gần như tức thời. Khi content team thêm nhiều section, endpoint vẫn trả đúng JSON nhưng latency tăng gần tuyến tính theo số section.

## Bạn cần làm gì

1. Chạy starter system và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Dùng command counter để xác định relationship giữa số section và số database command.
4. Sửa code trong `starter/` mà không thay đổi output contract.
5. Chạy `scripts/verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

Script seed dữ liệu local SQLite, render navigation và in ra số item cùng tổng số database command đã thực thi trong phần workload.

## Những gì cần quan sát

- Navigation output có đúng số section hay không.
- Tổng số database command của workload.
- Command count thay đổi thế nào khi tăng số section seed.
- Có exception hay functional failure nào không.

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

> ⚠️ Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference solution](solution/README.md)

## Expected Results

Before: output đúng nhưng command count vượt ngưỡng chấp nhận khi có 8 section.

After: output không đổi, nhưng workload dùng tối đa 2 database command và `scripts/verify.ps1` kết thúc với `PASS`.

## Estimated Time

35–55 phút.
