# UNIT-ASP-001 — Request Body Disappears After Audit Middleware

## Mục tiêu

Điều tra một ASP.NET Core request pipeline mà cùng một JSON request hoạt động khi bỏ middleware audit nhưng trả lỗi khi middleware được bật. Thu thập evidence trước khi thay đổi code và giữ nguyên hành vi audit sau khi sửa.

## Bối cảnh thực tế

Một internal Order API vừa thêm middleware để ghi lại raw request payload phục vụ audit. Sau deployment, `POST /orders` bắt đầu trả `400 Bad Request`, trong khi log audit vẫn in đúng JSON client gửi lên. GET endpoint và các request không có body vẫn bình thường.

## Bạn cần làm gì

1. Chạy starter và reproduce lỗi.
2. Ghi ít nhất 2 hypothesis về nơi payload có thể bị mất hoặc thay đổi.
3. Quan sát log middleware và response của endpoint.
4. Sửa `starter/Program.cs` để audit vẫn đọc được payload và endpoint vẫn nhận đúng JSON.
5. Chạy `verify.ps1` trên chính code bạn đã sửa.
6. Chỉ sau đó so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị
- Port `5081` đang trống

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script sẽ build và chạy `starter`, gửi một `POST /orders` với JSON hợp lệ, sau đó kiểm tra response.

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Middleware audit có nhìn thấy payload client gửi hay không.
- Endpoint trả HTTP status nào.
- Endpoint có đọc được một JSON document đầy đủ hay không.
- Vấn đề xảy ra trước hay sau khi request đi qua middleware audit.

Không thay đổi request payload để làm test pass; mục tiêu là giữ nguyên contract của client.

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

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

**Before:** audit log chứa JSON nhưng `POST /orders` trả `400` với thông báo endpoint không nhận được JSON hợp lệ.

**After:** audit log vẫn chứa JSON; cùng request trả `201`; response chứa `orderId` và `sku` đúng. Không dựa vào timing tuyệt đối.

## Estimated Time

30–45 phút.