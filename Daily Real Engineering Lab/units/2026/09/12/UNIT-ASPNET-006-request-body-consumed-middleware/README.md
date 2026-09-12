# UNIT-ASPNET-006 — Webhook body biến mất sau middleware

## Mục tiêu

Điều tra một ASP.NET Core webhook endpoint hoạt động khi gọi trực tiếp nhưng bắt đầu trả lỗi sau khi team thêm middleware đọc request payload để phục vụ audit/signature diagnostics.

## Bối cảnh thực tế

Một commerce integration nhận webhook JSON từ đối tác. Sau khi thêm middleware capture raw payload, request hợp lệ bắt đầu bị endpoint từ chối trước khi business handler chạy. Log của middleware vẫn cho thấy payload đầy đủ nên team ban đầu nghi ngờ model binding hoặc JSON options.

## Bạn cần làm gì

1. Chạy starter service.
2. Reproduce request webhook hợp lệ.
3. Ghi hypothesis trước khi sửa.
4. Sửa `starter/` để middleware vẫn đọc được raw body nhưng downstream endpoint vẫn nhận đúng payload.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần Docker hay dịch vụ ngoài

## Chạy nhanh

```powershell
./run.ps1
```

Ở terminal khác:

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` tự khởi động starter service trên `http://127.0.0.1:5066`, gửi một webhook JSON hợp lệ và kiểm tra rằng request không tới được handler thành công.

## Những gì cần quan sát

- Middleware log được payload gì.
- Endpoint trả status code gì.
- Business handler có chạy hay không.
- Trạng thái request body trước và sau khi middleware đọc.
- Có subsystem nào đang chia sẻ cùng một stream hay không.

Đừng thay JSON serializer hoặc DTO chỉ vì lỗi xuất hiện ở model-binding boundary.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Middleware nhìn thấy JSON đầy đủ.
- Request hợp lệ không hoàn thành với `200 OK`.
- Endpoint không xử lý được DTO như mong đợi.

### After

- Middleware vẫn capture được raw payload.
- Endpoint nhận đúng DTO.
- Response là `200 OK` với cùng `orderId`.

## Estimated Time

30–45 phút.
