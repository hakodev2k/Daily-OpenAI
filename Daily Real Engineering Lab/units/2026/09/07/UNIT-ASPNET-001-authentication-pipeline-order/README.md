# UNIT-ASPNET-001 — Request Có Identity Header Nhưng Vẫn Bị Từ Chối

## Mục tiêu

Điều tra một lỗi ASP.NET Core request pipeline khiến request mang thông tin định danh hợp lệ vẫn bị chặn trước khi endpoint xử lý.

## Bối cảnh thực tế

Một internal API dùng header `X-User` để mô phỏng cơ chế authentication của upstream gateway. Sau một thay đổi middleware, endpoint `/secure` bắt đầu trả `401` dù client vẫn gửi header đúng.

## Bạn cần làm gì

1. Chạy starter app.
2. Gửi request có `X-User`.
3. Quan sát status code và log.
4. Ghi ít nhất 2 hypothesis trước khi sửa.
5. Chỉ thay đổi code trong `starter/`.
6. Chạy `verify.ps1` để xác nhận fix.
7. Sau khi verify thành công mới xem `solution/`.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ khuyến nghị
- Port `5088` còn trống

## Chạy nhanh

```powershell
./run.ps1
```

Ở terminal khác:

```powershell
Invoke-WebRequest http://localhost:5088/secure -Headers @{ 'X-User' = 'ha' }
```

## Cách reproduce vấn đề

1. Start app bằng `run.ps1`.
2. Gửi request tới `/secure` với header `X-User: ha`.
3. Xác nhận request nhận `401` thay vì `200`.
4. Gửi request tới `/public` để xác nhận process vẫn hoạt động bình thường.

## Những gì cần quan sát

- `/public` có hoạt động không?
- `/secure` trả status nào khi có và không có `X-User`?
- Tại thời điểm access gate chạy, `HttpContext.User.Identity.IsAuthenticated` có giá trị gì?
- Log nào xuất hiện trước khi request bị kết thúc?
- Endpoint `/secure` có thực sự được gọi hay không?

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

⚠️ Spoiler: chỉ mở sau khi đã thử fix và chạy verify.

- [Reference solution](solution/README.md)
- [Wrong fixes](solution/wrong-fixes.md)

## Expected Results

Sau khi sửa đúng:

- `/public` trả `200`.
- `/secure` không có `X-User` vẫn trả `401`.
- `/secure` với `X-User` hợp lệ trả `200` và body chứa username.

## Estimated Time

25–40 phút.
