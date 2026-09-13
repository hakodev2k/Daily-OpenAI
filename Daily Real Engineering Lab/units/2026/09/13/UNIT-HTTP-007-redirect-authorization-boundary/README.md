# UNIT-HTTP-007 — Redirect hợp lệ nhưng request sau redirect bị 401

## Mục tiêu

Điều tra một lỗi HTTP chỉ xuất hiện khi endpoint trả redirect trong một flow cần `Authorization`.

## Bối cảnh thực tế

Một service .NET tải tài liệu từ partner API. Request đầu tiên tới URL ổn định của partner, sau đó partner redirect sang endpoint tải thực tế. Flow hoạt động khi gọi trực tiếp endpoint cuối nhưng request qua URL ban đầu lại kết thúc bằng `401 Unauthorized`.

## Bạn cần làm gì

1. Reproduce lỗi bằng starter.
2. Ghi lại request flow và ít nhất 3 hypothesis.
3. Xác định vì sao endpoint cuối không nhận được authentication như mong đợi.
4. Sửa starter theo cách không vô tình gửi credential tới redirect target không đáng tin cậy.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới mở reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần service bên ngoài

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-HTTP-007-redirect-authorization-boundary"
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Request đầu tiên được server redirect chấp nhận.
- Request cuối cùng tới resource endpoint trả `401 Unauthorized`.
- Cùng token đó có thể truy cập endpoint cuối khi được gửi đúng request.
- CPU, retry count và timeout không phải tín hiệu chính của incident này.

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

> Spoiler: chỉ xem sau khi đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:

- process in `starter/` kết thúc non-zero;
- output cuối có `FinalStatus=Unauthorized`.

After:

- learner code chỉ gửi credential tới redirect target đã được xác thực là hợp lệ;
- output cuối có `FinalStatus=OK`;
- process kết thúc với exit code `0`.

Nếu không reproduce được, kiểm tra xem port `5181` hoặc `5182` có đang bị process khác sử dụng hay không.

## Estimated Time

Khoảng 45 phút.
