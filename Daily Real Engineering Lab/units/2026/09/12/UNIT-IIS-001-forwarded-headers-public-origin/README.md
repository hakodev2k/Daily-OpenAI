# UNIT-IIS-001 — Public callback URL sai sau reverse proxy

## Mục tiêu

Điều tra một ASP.NET Core service tạo callback URL đúng khi chạy trực tiếp nhưng sai khi được đặt sau reverse proxy / TLS termination.

## Bối cảnh thực tế

Checkout service chạy HTTP nội bộ phía sau reverse proxy. Người dùng truy cập public endpoint bằng HTTPS. Payment provider nhận callback URL trỏ về internal origin nên redirect flow thất bại chỉ ở môi trường proxy.

## Bạn cần làm gì

1. Chạy starter service.
2. Reproduce request mô phỏng reverse proxy.
3. Ghi hypothesis trước khi sửa.
4. Sửa `starter/` để application hiểu đúng public origin nhưng không tin proxy metadata từ mọi nguồn.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần IIS thật, Docker, Azure hay dịch vụ trả phí

## Chạy nhanh

```powershell
./run.ps1
```

Ở terminal khác:

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` gửi request tới `127.0.0.1:5058` cùng metadata mô phỏng reverse proxy cho public request `https://shop.example.test`.

Script xác nhận starter đang tạo callback URL từ internal origin thay vì public origin.

## Những gì cần quan sát

- URL trả về bởi `/checkout/callback-url`.
- Giá trị `Request.Scheme` và `Request.Host` mà application nhìn thấy.
- Proxy metadata có mặt nhưng có được framework áp dụng hay không.
- Thứ tự middleware xử lý request metadata.
- Trust boundary: nguồn nào được phép ảnh hưởng tới scheme/host công khai.

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

- Request mô phỏng external HTTPS đi qua proxy.
- Application vẫn trả callback URL bắt đầu bằng `http://127.0.0.1:5058/...`.

### After

- Với request đến từ proxy tin cậy, callback URL là `https://shop.example.test/checkout/complete`.
- Request trực tiếp không có proxy metadata vẫn dùng origin trực tiếp.
- Business code không hard-code production hostname.

## Estimated Time

30–45 phút.