# UNIT-HTTP-001 — Request đúng endpoint name nhưng vẫn nhận 404

## Mục tiêu

Điều tra một HTTP client integration nơi `BaseAddress` nhìn đúng, relative endpoint cũng nhìn đúng, nhưng request cuối cùng lại đi sai path.

## Bối cảnh thực tế

Một backend service gọi internal fulfillment API phía sau reverse proxy. Proxy expose API dưới prefix `/gateway/v1/`. Code review cho thấy `HttpClient.BaseAddress` đã chứa prefix này, nhưng production log lại ghi nhận request tới root path và trả về `404`.

## Bạn cần làm gì

1. Chạy starter system và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa.
3. Quan sát URI thực tế mà `HttpClient` gửi xuống handler.
4. Sửa learner-editable code trong `starter/` để request giữ đúng path prefix.
5. Chạy `verify.ps1`.
6. Sau khi verify mới đọc hints sâu hơn và reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

Starter không gọi Internet. Một custom `HttpMessageHandler` ghi lại URI cuối cùng mà `HttpClient` tạo ra. Chạy lab và so sánh URI thực tế với URI mà integration contract yêu cầu.

## Những gì cần quan sát

- `BaseAddress` được cấu hình là gì.
- Relative request target được truyền vào `GetAsync` là gì.
- URI cuối cùng trong handler.
- Path prefix có còn tồn tại hay không.
- HTTP status giả lập dựa trên URI cuối cùng.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-1.md)
- [Hint 2](hints/hint-2.md)
- [Hint 3](hints/hint-3.md)

## Reference Solution

⚠️ Spoiler: chỉ mở sau khi đã tự verify.

- [Reference solution](solution/README.md)
- [Wrong fixes](solution/wrong-fixes.md)

## Expected Results

Sau khi sửa đúng, `verify.ps1` phải kết thúc với `PASS`, final URI phải là `https://fulfillment.local/gateway/v1/orders/42` và status phải là `200`.

## Estimated Time

25–40 phút.
