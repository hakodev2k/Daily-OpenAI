# UNIT-MSG-001 — Một payment event tạo hai shipment reservation

## Mục tiêu

Điều tra một message consumer chạy ổn trong happy path nhưng tạo side effect lặp khi cùng một event được delivery nhiều hơn một lần.

## Bối cảnh thực tế

Một fulfillment worker nhận sự kiện `OrderPaid` rồi tạo shipment reservation. Trong một incident, broker telemetry cho thấy cùng một logical event xuất hiện hai lần gần nhau. Worker không crash, không có exception, nhưng downstream shipping system ghi nhận hai reservation cho cùng order.

## Bạn cần làm gì

1. Chạy starter system và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Xác định evidence nào cho biết hai delivery có phải cùng một logical event hay không.
4. Sửa learner-editable code trong `starter/` để side effect chỉ xảy ra đúng theo business intent.
5. Chạy `verify.ps1`.
6. Sau khi hoàn tất mới xem hints sâu hơn và reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

Không cần database, Docker, cloud account hay message broker thật.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy local deterministic broker simulation với hai delivery và kiểm tra xem downstream side effect có bị lặp hay không.

## Những gì cần quan sát

- Số delivery mà consumer nhận được.
- `EventId` của từng delivery.
- `OrderId` tương ứng.
- Số lần shipping gateway được gọi.
- Số reservation được tạo cho cùng order.
- Consumer có exception hay retry explicit nào không.

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

> ⚠️ Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference solution](solution/README.md)

## Expected Results

Before fix:

- hai delivery đều được consumer xử lý;
- cùng một logical event dẫn tới nhiều hơn một shipment reservation;
- không cần exception để bug xuất hiện.

After fix:

- consumer vẫn chấp nhận hai delivery;
- business side effect chỉ xảy ra một lần cho cùng logical event;
- delivery khác với `EventId` mới vẫn được xử lý bình thường.

Nếu không reproduce được, chạy `dotnet --version` và xác nhận SDK 8.x khả dụng, sau đó chạy `dotnet clean starter/Lab.csproj` rồi thử lại.

## Estimated Time

35–50 phút.
