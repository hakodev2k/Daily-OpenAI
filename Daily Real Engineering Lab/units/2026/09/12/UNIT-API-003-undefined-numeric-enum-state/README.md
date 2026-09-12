# UNIT-API-003 — Trạng thái ngoài contract vẫn đi qua API

## Mục tiêu

Điều tra một REST API nhận payload có vẻ hợp lệ về JSON nhưng tạo ra trạng thái domain mà hệ thống không định nghĩa.

## Bối cảnh thực tế

Order API nhận cập nhật trạng thái từ nhiều client. Mobile app chính chỉ gửi các giá trị đã document, nhưng một integration cũ gửi một payload khác dạng. Request vẫn nhận `200 OK`, sau đó downstream code gặp trạng thái không biết xử lý và đẩy order vào nhánh fallback.

## Bạn cần làm gì

1. Chạy starter API.
2. Reproduce request bất thường bằng `reproduce.ps1`.
3. Ghi hypothesis trước khi sửa.
4. Sửa code trong `starter/` để API giữ đúng public contract và không tạo domain state ngoài tập hợp hợp lệ.
5. Chạy `verify.ps1` để kiểm tra cả request hợp lệ và bất hợp lệ.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần Docker hay external service

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script tự khởi động starter trên `127.0.0.1:5063`, gửi một request nằm ngoài contract đã document và chỉ PASS khi chứng minh API vẫn chấp nhận request đó.

## Những gì cần quan sát

- HTTP status của request bất thường.
- Giá trị status được application echo lại.
- Sự khác nhau giữa JSON syntax hợp lệ và domain value hợp lệ.
- Boundary nào nên chịu trách nhiệm bảo vệ public API contract.
- Một fix có vô tình phá request hợp lệ đang dùng string value hay không.

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

- `"Shipped"` được chấp nhận.
- Payload bất thường trong script vẫn nhận success và tạo một status ngoài contract.

### After

- `"Shipped"` vẫn được chấp nhận.
- Payload ngoài contract bị từ chối bằng HTTP 4xx.
- Không cần hard-code kiểm tra vào downstream workflow để che triệu chứng.

## Estimated Time

30–45 phút.