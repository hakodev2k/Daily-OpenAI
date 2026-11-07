# UNIT-DOCKER-001 — Healthy Container, Unready Application

## Mục tiêu
Điều tra một deployment trong đó container được báo healthy nhưng request đầu tiên sau deploy vẫn thất bại.

## Bối cảnh thực tế
Một document-processing API cần hoàn tất bước khởi tạo nội bộ trước khi phục vụ request. Sau rollout, dashboard container báo healthy gần như ngay lập tức, nhưng traffic được chuyển vào instance mới quá sớm và một số request nhận `503`.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Quan sát trạng thái process, health endpoint và request nghiệp vụ theo timeline.
4. Sửa `starter/` để health contract phản ánh đúng trạng thái cần thiết cho routing.
5. Chạy `verify.ps1`.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- thời điểm process bắt đầu chạy
- kết quả health probe trong giai đoạn startup
- thời điểm request nghiệp vụ có thể thành công
- sự khác biệt giữa trạng thái process và khả năng phục vụ traffic

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
Before: probe có thể báo trạng thái chấp nhận traffic trước khi request nghiệp vụ sẵn sàng.

After: routing signal chỉ thành công khi instance thực sự có thể xử lý request; trạng thái sống của process vẫn có thể được đánh giá độc lập.

Nếu không reproduce được, chạy trực tiếp `dotnet run --project starter -- --reproduce` và kiểm tra timeline được in ra.

## Estimated Time
45 phút.