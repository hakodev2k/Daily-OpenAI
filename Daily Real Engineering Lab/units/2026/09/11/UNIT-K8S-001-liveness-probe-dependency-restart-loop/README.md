# UNIT-K8S-001 — Liveness Probe Dependency Restart Loop

## Mục tiêu

Điều tra một service chạy ổn định bình thường nhưng liên tục bị restart khi database downstream bị gián đoạn ngắn. Mục tiêu là phân biệt health signal dành cho process survival với health signal dành cho traffic routing, rồi sửa probe contract mà không che giấu dependency failure.

## Bối cảnh thực tế

Một ASP.NET Core API chạy trong Kubernetes. Khi database bị chậm hoặc tạm thời unavailable trong khoảng ngắn, pod bắt đầu restart lặp lại. Sau khi database phục hồi, một số pod vẫn mất thêm thời gian mới ổn định, làm thời gian gián đoạn dài hơn sự cố dependency ban đầu.

Lab dùng một simulator local để tái hiện cách kubelet phản ứng với health result, không cần Kubernetes cluster thật.

## Bạn cần làm gì

1. Chạy starter và reproduce restart loop.
2. Thu thập evidence từ output của simulator.
3. Viết ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
4. Sửa logic health policy trong `starter/`.
5. Chạy `verify.ps1` để chứng minh dependency outage không còn làm process bị restart, trong khi service vẫn bị đánh dấu không sẵn sàng nhận traffic.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Docker/Kubernetes/database thật

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy một timeline deterministic gồm trạng thái database healthy → unavailable → healthy và kiểm tra số lần simulated restart.

## Những gì cần quan sát

- Process bản thân vẫn có thể chạy trong thời gian dependency outage.
- Traffic eligibility thay đổi khi dependency unavailable.
- Một health decision khác lại khiến simulator tăng restart count.
- Sự cố dependency ngắn có thể bị khuếch đại thành thời gian recovery dài hơn.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- dependency outage làm service trở thành not-ready
- simulator đồng thời ghi nhận restart
- restart count lớn hơn 0

After:
- dependency outage vẫn làm service not-ready
- process health vẫn phản ánh khả năng process tiếp tục chạy
- restart count bằng 0 trong transient dependency outage

Nếu không reproduce được, chạy `dotnet run --project starter` trực tiếp và kiểm tra output từng tick.

## Estimated Time

35–50 phút.
