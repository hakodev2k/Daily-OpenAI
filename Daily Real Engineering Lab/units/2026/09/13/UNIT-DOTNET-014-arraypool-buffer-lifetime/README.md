# UNIT-DOTNET-014 — Pooled Buffer Lifetime

## Mục tiêu

Điều tra một lỗi dữ liệu xảy ra khi code tối ưu allocation bằng pooled buffer nhưng dữ liệu được trả về cho caller không còn ổn định theo thời gian.

## Bối cảnh thực tế

Một service tạo payload nhị phân trước khi gửi sang hệ thống downstream. Để giảm allocation, implementation dùng buffer pool. Ở lần chạy đơn lẻ payload có vẻ đúng, nhưng khi một request khác chạy ngay sau đó, payload trước đó đôi khi đổi nội dung trước khi được gửi.

## Bạn cần làm gì

- Chạy starter và reproduce triệu chứng.
- Ghi hypothesis trước khi sửa.
- Xác định boundary ownership/lifetime của vùng nhớ được trả về.
- Sửa starter sao cho payload mà caller nhận được vẫn hợp lệ cho tới khi caller dùng xong.
- Chạy `verify.ps1` để xác nhận fix.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-DOTNET-014-arraypool-buffer-lifetime"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy starter với một pool deterministic để hiện tượng có thể tái hiện ổn định trên mọi máy.

## Những gì cần quan sát

- Payload đầu tiên đúng ngay sau khi được tạo.
- Sau một lần tạo payload khác, dữ liệu mà caller đang giữ cho payload đầu tiên không còn giống giá trị ban đầu.
- Không có exception bắt buộc phải xuất hiện; failure chính là dữ liệu thay đổi ngoài kỳ vọng của caller.

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

> Spoiler: chỉ xem sau khi đã thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- `reproduce.ps1` xác nhận payload đầu tiên bị thay đổi sau khi pool được dùng lại.

**After**

- `verify.ps1` xác nhận payload đầu tiên vẫn giữ nguyên giá trị dù có operation tiếp theo sử dụng pool.
- Functional output vẫn đúng.

## Estimated Time

25–40 phút.
