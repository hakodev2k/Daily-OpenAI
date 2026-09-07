# UNIT-HTTP-001 — Request nhìn đúng nhưng đi sai endpoint

## Mục tiêu

Điều tra một HTTP client gọi dependency nội bộ nhưng request thực tế lại đi tới path khác với path mà developer dự kiến.

## Bối cảnh thực tế

Một service gọi Order API thông qua `HttpClient`. Code review nhìn qua có vẻ hợp lý: client có `BaseAddress`, call site chỉ truyền relative path, không có exception transport. Tuy nhiên dependency trả về `404 Not Found` dù endpoint đích vẫn hoạt động.

## Bạn cần làm gì

1. Chạy starter và reproduce lỗi.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Quan sát URI thực tế mà `HttpClient` tạo ra.
4. So sánh URI thực tế với URI mà service cần gọi.
5. Sửa learner-editable code trong `starter/`.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới đọc hints sâu hơn và reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

Chạy starter và quan sát `ConfiguredBaseAddress`, `RequestedRelativeUri`, `ObservedRequestUri` và HTTP status code mà fake dependency trả về.

## Những gì cần quan sát

- Request có thực sự chứa path segment `/api/` hay không.
- `BaseAddress` và relative URI được combine thành absolute URI như thế nào.
- Có DNS/network exception hay chỉ là response HTTP hợp lệ nhưng sai endpoint.
- Việc đổi một trong hai URI input có làm thay đổi URI cuối cùng hay không.

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

Starter phải reproduce `404` và in URI thực tế không khớp endpoint mong đợi. Sau khi sửa đúng, `verify.ps1` phải kết thúc với `PASS`, URI cuối cùng là `https://orders.internal/api/orders/42` và status code là `200`.

## Estimated Time

25–40 phút.
