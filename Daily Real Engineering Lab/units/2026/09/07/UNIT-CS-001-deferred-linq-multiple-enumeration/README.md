# UNIT-CS-001 — Deferred LINQ Pipeline Gọi Dependency Hai Lần

## Mục tiêu

Hiểu deferred execution của LINQ, nhận diện multiple enumeration và chọn đúng materialization boundary khi pipeline có I/O hoặc side effect.

## Bối cảnh thực tế

Một scheduled job kiểm tra tồn kho trước khi tạo reservation. Kết quả business đúng, nhưng inventory dependency nhận số request gấp đôi dự kiến, làm tăng latency và tải hệ thống.

## Bạn cần làm gì

1. Chạy starter và xác nhận số inventory calls.
2. Ghi hypothesis trước khi sửa.
3. Xác định vì sao cùng một pipeline chạy nhiều lần.
4. Sửa để mỗi order chỉ kiểm tra inventory một lần.
5. Chạy verify và so sánh reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
dotnet run --project ./starter/RealEngineeringLab.csproj
```

Starter phải in `Inventory calls: 6` cho 3 orders.

## Những gì cần quan sát

- Kết quả eligible orders vẫn đúng.
- `Count()` enumerate pipeline một lần.
- `foreach` enumerate cùng pipeline thêm một lần.
- Predicate `CanReserve` mô phỏng dependency call có side effect.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](./hints/hint-01.md)
- [Hint 2](./hints/hint-02.md)
- [Hint 3](./hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã tự điều tra.

[Reference Solution](./solution/README.md)

## Expected Results

Before: `Inventory calls: 6`

After: `Inventory calls: 3`

Functional result vẫn là 2 eligible orders.

## Estimated Time

30–40 phút.
