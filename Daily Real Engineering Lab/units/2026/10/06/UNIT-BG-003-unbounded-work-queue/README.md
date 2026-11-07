# UNIT-BG-003 — Unbounded Work Queue Under Burst Load

## Mục tiêu

Điều tra một background processing service hoạt động đúng ở steady state nhưng tăng memory và backlog rất nhanh khi producer có burst traffic.

## Bối cảnh thực tế

Một API nhận notification jobs rồi đẩy vào in-process queue cho worker xử lý. Bình thường throughput ổn định. Trong campaign burst, API vẫn accept jobs rất nhanh trong khi worker xử lý chậm hơn; process memory và queue depth tăng liên tục.

## Bạn cần làm gì

1. Chạy starter và reproduce.
2. Ghi hypothesis dựa trên producer rate, consumer rate, queue depth và memory.
3. Sửa `starter/` để queue có capacity contract rõ ràng và producer phải phản ứng khi capacity cạn.
4. Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
dotnet run --project ./starter/QueueLab.csproj -- reproduce
```

## Những gì cần quan sát

- Peak queue depth.
- Producer completion time.
- Consumer throughput.
- Quan hệ giữa tốc độ enqueue và tốc độ xử lý.

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

⚠️ Spoiler: [solution/README.md](solution/README.md)

## Expected Results

Starter cho thấy producer có thể hoàn tất gần như ngay lập tức trong khi backlog tăng lớn. Sau fix, queue depth phải bị giới hạn và producer phải chịu backpressure thay vì tạo backlog không giới hạn.

## Estimated Time

45–60 phút.
