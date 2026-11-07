# UNIT-HTTP-009 — Retry Request Message Reuse

## Mục tiêu

Điều tra một HTTP retry flow: attempt đầu nhận `503`, nhưng attempt tiếp theo thất bại trước khi fake downstream nhận request.

## Bối cảnh thực tế

Một integration service gọi partner API và retry một lần khi gặp transient failure. Kết quả nghiệp vụ thất bại dù fake dependency được cấu hình để trả `200` ở lần gọi thứ hai.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1`.
2. Ghi hypothesis.
3. Kiểm tra lifecycle của dữ liệu request giữa các attempt.
4. Sửa code trong `starter/`.
5. Chạy `./verify.ps1`.
6. Sau đó mới xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Fake handler trả `503` ở lần gửi đầu và `200` ở lần gửi thứ hai nếu request thứ hai thực sự được gửi.

## Những gì cần quan sát

- exception type;
- số lần fake downstream nhận request;
- state nào đang được giữ lại giữa các retry attempt.

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

> Chỉ xem sau khi tự điều tra và thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

Before: downstream nhận 1 request và retry kết thúc bằng exception.

After: downstream nhận 2 request, lần thứ hai trả `200`, verification pass.

## Estimated Time

25–40 phút.
