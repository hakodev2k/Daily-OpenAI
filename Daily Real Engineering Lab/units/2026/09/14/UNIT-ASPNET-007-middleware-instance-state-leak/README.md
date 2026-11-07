# UNIT-ASPNET-007 — Middleware Instance State Leak

## Mục tiêu

Điều tra một lỗi cross-request contamination trong ASP.NET Core middleware khi hai request đồng thời đi qua cùng một middleware instance.

## Bối cảnh thực tế

Một middleware ghi audit tenant cho request. Ở tải thấp log nhìn đúng, nhưng khi có request đồng thời, đôi lúc audit record của request A lại mang tenant của request B. Đây là lỗi khó chịu vì request vẫn trả thành công và không có exception.

## Bạn cần làm gì

- Reproduce lỗi bằng script có sẵn.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Xác định state nào có lifetime không phù hợp với request concurrency.
- Sửa code trong `starter/` mà không thay đổi yêu cầu nghiệp vụ.
- Chạy `verify.ps1` để chứng minh cả hai request giữ đúng tenant identity.
- Sau đó mới xem reference solution.

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

Script chạy hai request mô phỏng đồng thời qua cùng middleware instance và kiểm tra audit output.

## Những gì cần quan sát

- Hai request đều hoàn thành bình thường.
- Audit output có thể gán sai tenant cho ít nhất một request trong starter state.
- Không cần exception để chứng minh có lỗi correctness.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thu thập evidence từ output và lifetime của object.
4. Thử fix trong `starter/`.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- request A và B chạy thành công.
- ít nhất một audit record có tenant không khớp request gốc.

After:
- `request-a -> tenant-a`.
- `request-b -> tenant-b`.
- verification pass ổn định.

## Estimated Time

35–50 phút.
