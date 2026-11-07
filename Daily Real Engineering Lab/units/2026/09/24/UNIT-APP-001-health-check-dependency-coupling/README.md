# UNIT-APP-001 — Health Check Dependency Coupling

## Mục tiêu

Điều tra một incident availability trong ASP.NET Core chạy trên Azure App Service: application process vẫn hoạt động, nhưng khi một dependency phụ trợ gặp sự cố ngắn, serving capacity của toàn bộ application giảm mạnh.

## Bối cảnh thực tế

Customer Profile API có nhiều instance. Endpoint chính vẫn có thể phục vụ phần lớn request khi Recommendation API tạm thời unavailable. Trong incident, platform health observations chuyển nhiều instance sang unhealthy gần như cùng lúc và traffic thành công giảm mạnh hơn mức ảnh hưởng trực tiếp của Recommendation API.

## Bạn cần làm gì

1. Chạy starter để quan sát health endpoint trong trạng thái bình thường.
2. Reproduce dependency outage bằng cơ chế có sẵn.
3. Thu thập evidence từ health response, application behavior và incident timeline.
4. Ghi ít nhất ba hypothesis trước khi sửa.
5. Thay đổi learner-editable starter để health semantics phản ánh đúng availability contract.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị
- Không cần Azure subscription

## Chạy nhanh

```powershell
./run.ps1
```

Mở terminal khác:

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` bật trạng thái failure của dependency mô phỏng, gọi business endpoint và health endpoint, rồi lưu evidence ra console. Hãy so sánh phạm vi chức năng thực sự bị ảnh hưởng với tín hiệu health mà host/platform sẽ quan sát.

## Những gì cần quan sát

- Business endpoint nào còn phục vụ được request.
- HTTP status của health endpoint trước và trong dependency outage.
- Một dependency failure có làm thay đổi process/runtime health hay không.
- Blast radius nếu mọi instance cùng quan sát dependency failure đó.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

⚠️ Spoiler — chỉ xem sau khi đã tự điều tra và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before: dependency outage tạo health signal có blast radius lớn hơn capability thực sự bị mất.

After: instance vẫn được đánh giá theo đúng serving contract cốt lõi, trong khi trạng thái dependency vẫn quan sát được và capability liên quan degrade có kiểm soát.

## Estimated Time

45–75 phút.
