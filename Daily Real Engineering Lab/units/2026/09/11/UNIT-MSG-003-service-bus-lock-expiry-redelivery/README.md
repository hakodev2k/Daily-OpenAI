# UNIT-MSG-003 — Service Bus Lock Expiry Redelivery

## Mục tiêu

Điều tra một background worker xử lý message dài hơn bình thường và đôi lúc cùng một công việc được xử lý đồng thời bởi hai consumer. Bạn cần tìm boundary gây redelivery, sửa lifecycle của message processing và xác minh rằng công việc dài vẫn chỉ có một active owner.

## Bối cảnh thực tế

Một worker tạo bản preview cho tài liệu nhận job từ queue. Phần lớn job hoàn tất nhanh, nhưng file lớn mất lâu hơn. Production log thỉnh thoảng cho thấy cùng `messageId` bắt đầu lần thứ hai trước khi lần xử lý đầu kết thúc, gây hai lần ghi output và tranh chấp trạng thái.

Lab dùng broker simulator local để tái hiện contract tương tự peek-lock, không cần Azure subscription.

## Bạn cần làm gì

1. Chạy reproduction và ghi timeline quan sát được.
2. Đưa ra ít nhất hai hypothesis trước khi mở hints.
3. Sửa code trong `starter/` để handler dài không mất quyền sở hữu message trong khi vẫn đang xử lý.
4. Chạy `verify.ps1`.
5. Sau đó mới xem reference solution và liên hệ với Azure Service Bus SDK.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-MSG-003-service-bus-lock-expiry-redelivery"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter với một job có thời gian xử lý dài hơn lease window của simulator. Script kỳ vọng thấy delivery thứ hai bắt đầu trước khi delivery đầu hoàn tất.

## Những gì cần quan sát

- cùng một `messageId` xuất hiện ở nhiều delivery
- timeline của `START` và `COMPLETE`
- số active handler lớn nhất cho cùng message
- broker vẫn coi message là có thể giao lại trong khi công việc đầu chưa xong

README cố ý không chỉ ra cơ chế hoặc API cần sửa.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler — chỉ mở sau khi đã tự điều tra và thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:
- cùng message có thể có hai handler active chồng lấn
- `MAX_CONCURRENT_FOR_MESSAGE` lớn hơn `1`
- có redelivery trước khi xử lý đầu kết thúc

Sau khi sửa:
- job vẫn mất lâu hơn lease window ban đầu
- ownership được duy trì trong suốt quá trình xử lý
- `MAX_CONCURRENT_FOR_MESSAGE=1`
- `DELIVERIES=1`
- `COMPLETIONS=1`

Nếu không reproduce được, chạy `dotnet --info` rồi `dotnet run --project starter/Lab.csproj` để xem output trực tiếp.

## Estimated Time

40–60 phút.