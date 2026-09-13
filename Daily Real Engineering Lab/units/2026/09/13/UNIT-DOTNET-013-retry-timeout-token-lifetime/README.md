# UNIT-DOTNET-013 — Retry sau timeout bị dừng ngay lập tức

## Bối cảnh

Một background worker đồng bộ catalog từ supplier theo từng attempt. Attempt đầu có thể vượt quá timeout cục bộ nên worker retry. Trong production, team thấy retry thứ hai đôi khi kết thúc gần như ngay lập tức dù downstream đang phản hồi rất nhanh.

## Mục tiêu

- Reproduce hành vi retry bất thường.
- Thu thập evidence trước khi sửa.
- Xác định lifetime phù hợp cho timeout state của từng attempt.
- Sửa starter và verify mà không làm mất khả năng dừng toàn bộ operation từ token bên ngoài.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-DOTNET-013-retry-timeout-token-lifetime"
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter nguyên bản. Kịch bản cố ý làm attempt 1 chậm hơn timeout, sau đó retry với downstream rất nhanh.

## Những gì cần quan sát

- Attempt 1 hết thời gian như dự kiến.
- Attempt 2 không có đủ thời gian để hoàn thành dù simulated downstream chỉ cần vài chục milliseconds.
- So sánh thời điểm bắt đầu/kết thúc của từng attempt và trạng thái token truyền xuống dependency.

Ghi evidence và ít nhất 3 hypotheses vào `workspace/my-investigation.md` trước khi mở solution.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Sửa trực tiếp `starter/Program.cs`.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference Solution

> Spoiler — chỉ mở sau khi bạn đã tự thử fix.

- `solution/README.md`
- `solution/Program.cs`

## Expected Results

Before:
- attempt 1 timeout;
- retry tiếp theo dừng gần như ngay lập tức;
- process kết thúc với exit code khác 0.

After:
- attempt 1 vẫn timeout theo policy;
- attempt 2 có timeout budget độc lập và hoàn thành thành công;
- token ngoài vẫn có thể dừng toàn bộ operation;
- process kết thúc với exit code 0.

## Estimated Time

Khoảng 35 phút.
