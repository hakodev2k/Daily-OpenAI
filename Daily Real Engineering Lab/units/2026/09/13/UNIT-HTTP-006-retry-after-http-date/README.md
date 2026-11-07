# UNIT-HTTP-006 — Retry-After HTTP-date

## Mục tiêu

Điều tra một HTTP client xử lý rate limit không ổn định: một số `429 Too Many Requests` được retry đúng, nhưng một số khác bị retry gần như ngay lập tức dù server đã yêu cầu chờ.

## Bối cảnh thực tế

Một background worker đồng bộ tồn kho gọi Partner API. Khi traffic tăng, Partner API trả `429` cùng `Retry-After`. Trong production, log cho thấy client đôi khi backoff đúng vài giây, nhưng có lúc retry liên tiếp và kéo dài rate-limit window.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce hai response `Retry-After` khác nhau.
3. Ghi hypothesis trước khi sửa.
4. Sửa code trong `starter/` để client hiểu đúng contract của header và không tạo delay âm/quá lớn.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần internet hay dịch vụ bên ngoài

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy starter với hai dạng response rate-limit hợp lệ và chứng minh một dạng bị chuyển thành retry delay sai.

## Những gì cần quan sát

- Header `Retry-After` nguyên bản.
- Delay mà client tính ra.
- Hai response đều hợp lệ theo HTTP nhưng kết quả parser khác nhau.
- Hành vi fallback khi header không parse được.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Một `Retry-After` được chuyển thành delay hợp lý.
- Một `Retry-After` hợp lệ khác bị rơi vào fallback `0s`.

### After

- Cả hai representation hợp lệ đều tạo delay hợp lý.
- Delay quá khứ được clamp về `0`.
- Invalid header vẫn đi qua explicit fallback policy.

## Estimated Time

30–45 phút.