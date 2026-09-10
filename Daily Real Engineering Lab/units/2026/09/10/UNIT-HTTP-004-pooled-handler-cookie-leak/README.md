# UNIT-HTTP-004 — Pooled Handler Cookie Leak

## Mục tiêu

Điều tra một HTTP integration có state xuất hiện chéo giữa hai request độc lập dù mỗi request tạo `HttpClient` mới.

## Bối cảnh thực tế

Một gateway gọi legacy partner API. Request của tenant A chạy trước, sau đó tenant B thực hiện request độc lập. Log cho thấy downstream đôi khi nhận một cookie mà tenant B chưa từng gửi. Sự cố chỉ xuất hiện khi các request dùng cùng named client trong `IHttpClientFactory`.

## Bạn cần làm gì

1. Reproduce hiện tượng cross-request state.
2. Ghi hypothesis trước khi sửa.
3. Xác định state nằm ở đâu và vòng đời nào đang giữ nó.
4. Sửa `starter/` để request sau không tự động mang state của request trước.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Port `5098` đang trống

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Request đầu tiên khiến downstream trả về cookie.
- Một `HttpClient` mới được tạo cho request thứ hai.
- Request thứ hai vẫn có thể mang cookie từ request trước.
- Không có biến static/application cache nào chủ động copy cookie giữa hai request.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước fix, output có `SECOND_CALL_COOKIE=LegacySession=tenant-a`.

Sau fix, output phải có `SECOND_CALL_COOKIE=<none>` và request đầu tiên vẫn thành công.

## Estimated Time

35–50 phút.
