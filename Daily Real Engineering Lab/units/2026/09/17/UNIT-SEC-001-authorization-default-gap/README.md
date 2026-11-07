# UNIT-SEC-001 — Endpoint mới bypass authentication assumption

## Mục tiêu
Điều tra một authorization regression khi API surface mở rộng nhưng security invariant của hệ thống không được enforce nhất quán.

## Bối cảnh thực tế
Một internal administration API đã chạy ổn định nhiều tháng. Team tin rằng mọi business endpoint đều yêu cầu authenticated user, ngoại trừ health check. Sau một release nhỏ, security regression test phát hiện endpoint mới `/reports/preview` trả `200` cho anonymous request, trong khi endpoint cũ vẫn trả `401`.

## Bạn cần làm gì
1. Chạy starter và reproduce behavior.
2. Ghi ít nhất 2 hypothesis trước khi sửa.
3. So sánh authorization behavior của endpoint cũ, endpoint mới và health endpoint.
4. Xác định security invariant mà team đang ngầm dựa vào.
5. Sửa code trong `starter/` để protected business surface fail closed mà health endpoint vẫn anonymous.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy authorization pipeline mô phỏng metadata của ba endpoint và xác nhận anonymous request có thể đi qua một business route mới.

## Những gì cần quan sát
- Decision của từng route khi request không có identity.
- Sự khác nhau giữa explicit endpoint metadata và application-wide behavior.
- Health route phải tiếp tục public sau khi sửa.
- Authenticated request phải tiếp tục truy cập protected routes.

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
[Reference Solution — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** một business route mới cho anonymous request đi qua; route cũ bị từ chối; health route vẫn public.

**After:** mọi protected business route đều từ chối anonymous request; health route vẫn public; authenticated request vẫn đi qua.

## Estimated Time
30–45 phút.