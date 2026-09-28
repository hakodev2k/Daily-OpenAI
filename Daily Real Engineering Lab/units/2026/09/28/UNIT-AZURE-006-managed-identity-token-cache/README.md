# UNIT-AZURE-006 — Token cache không tôn trọng vòng đời credential

## Mục tiêu

Điều tra một integration failure nơi ứng dụng lấy credential thành công lúc khởi động nhưng bắt đầu bị từ chối sau khi chạy đủ lâu. Tập trung vào evidence về thời gian, cache lifetime và request boundary.

## Bối cảnh thực tế

Một background worker gọi API nội bộ bằng access token lấy từ một credential provider mô phỏng. Token có thời hạn ngắn để lab reproduce nhanh. Worker cache token để giảm số lần gọi provider, nhưng sau một khoảng thời gian dependency bắt đầu trả 401.

Lab chạy hoàn toàn local, không cần Azure subscription hay secret thật.

## Bạn cần làm gì

1. Chạy starter và reproduce.
2. Ghi hypothesis trước khi đọc hints.
3. Quan sát thời điểm token được cấp, thời điểm request bị từ chối và số lần provider được gọi.
4. Sửa lifecycle sao cho worker không dùng credential đã hết hạn.
5. Verify và giải thích trade-off giữa reuse token và refresh quá thường xuyên.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Bao nhiêu lần credential provider được gọi.
- Request nào bắt đầu thất bại.
- Quan hệ giữa thời gian cấp credential và thời gian request.
- Việc cache đang giữ object, value hay một contract có expiry.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-1.md)
- [Hint 2](hints/hint-2.md)
- [Hint 3](hints/hint-3.md)

## Reference Solution

⚠️ Spoiler: [Reference solution](solution/README.md)

## Expected Results

Sau fix, tất cả request đều thành công và credential chỉ được refresh khi cần, trước khi token trở nên không còn dùng được.

## Estimated Time

35–50 phút.
