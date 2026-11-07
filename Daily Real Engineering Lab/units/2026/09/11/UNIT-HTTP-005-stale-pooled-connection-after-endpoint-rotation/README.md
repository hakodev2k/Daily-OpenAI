# UNIT-HTTP-005 — Stale Pooled Connection After Endpoint Rotation

## Mục tiêu

Điều tra một HTTP client chạy ổn khi service ổn định nhưng tiếp tục gửi request tới instance cũ sau khi endpoint backend đã được rotate.

## Bối cảnh thực tế

Một background worker gọi `partner.internal` liên tục. Trong đợt deployment, DNS/service-discovery đã chuyển traffic từ instance A sang B. Worker không crash, request vẫn thành công, nhưng telemetry cho thấy một số process vẫn gọi A lâu hơn dự kiến.

Lab mô phỏng local hai backend A/B và một endpoint registry thay đổi đích kết nối.

## Bạn cần làm gì

1. Chạy reproduction và ghi evidence.
2. Giải thích vì sao thay đổi endpoint không đồng nghĩa mọi request sau đó lập tức đi tới endpoint mới.
3. Sửa `starter/ClientFactory.cs`.
4. Chạy `verify.ps1` để chứng minh client eventually dùng backend B mà không tạo `HttpClient` mới cho từng request.
5. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần Docker hay dịch vụ cloud.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-HTTP-005-stale-pooled-connection-after-endpoint-rotation"
./reproduce.ps1
```

## Cách reproduce vấn đề

Script chạy `starter/` ở chế độ reproduction. Chương trình:

- khởi động backend A và B trên loopback
- request lần đầu khi registry trỏ tới A
- chuyển registry sang B
- request lần hai bằng cùng client

Reproduction thành công khi quan sát được request thứ hai vẫn đi tới instance cũ.

## Những gì cần quan sát

- Registry đã đổi sang B trước request thứ hai.
- Request thứ hai vẫn hoàn thành thành công.
- Header `X-Backend` cho biết backend thực tế xử lý request.
- Không có lỗi DNS hay connection exception để dựa vào như tín hiệu bắt buộc.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Chỉ sửa `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- request 1: backend A
- registry chuyển sang B
- request 2: vẫn backend A

### Sau khi sửa

- request 1: backend A
- registry chuyển sang B
- sau policy refresh window, request 2: backend B
- vẫn tái sử dụng `HttpClient`; không tạo client mới mỗi request

Nếu port local bị chiếm, đổi `PortA`/`PortB` trong `Program.cs` sang hai port trống.

## Estimated Time

35–55 phút.