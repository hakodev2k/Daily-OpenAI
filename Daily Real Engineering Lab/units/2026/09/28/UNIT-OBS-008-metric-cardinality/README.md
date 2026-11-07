# UNIT-OBS-008 — Metric cardinality bùng nổ

## Mục tiêu

Điều tra vì sao một metric tưởng như đơn giản tạo ra hàng nghìn time series và làm chi phí/khả năng truy vấn telemetry xấu đi.

## Bối cảnh thực tế

Một API theo dõi số request theo endpoint. Traffic vẫn bình thường nhưng backend metrics báo số series tăng gần tuyến tính theo số user hoạt động.

## Bạn cần làm gì

Chạy starter, đo số series, xác định dimension nào làm identity của series tăng không kiểm soát, rồi sửa instrumentation để vẫn trả lời được câu hỏi vận hành “endpoint nào đang có traffic?” mà không gắn identity riêng cho từng request/user.

## Yêu cầu môi trường

- .NET 10 SDK
- PowerShell 7+

## Chạy nhanh

```powershell
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

Starter mô phỏng 5.000 requests và in `series_count`.

## Những gì cần quan sát

- Số request.
- Số metric series duy nhất.
- Các dimension tạo nên identity của một series.
- Thông tin nào thực sự cần cho dashboard/alert.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-1.md) · [Hint 2](hints/hint-2.md) · [Hint 3](hints/hint-3.md)

## Reference Solution

⚠️ Spoiler: [solution](solution/README.md)

## Expected Results

`series_count` giảm xuống một tập hữu hạn nhỏ trong khi metric vẫn phân biệt được logical endpoint.

## Estimated Time

30–45 phút.
