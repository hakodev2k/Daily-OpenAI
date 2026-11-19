# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
240 request tạo ra 240 metric series trong khi chỉ có 2 route và 2 status.

## Evidence
`./reproduce.ps1` ghi `requests=240 series=240 statuses=200,503`. Metric backend sẽ phải index hàng loạt chuỗi khác nhau.

## Root cause
`VisitId` là thuộc tính high-cardinality (gần như duy nhất mỗi request) được đưa vào series key. Cardinality tăng theo traffic thay vì theo tập label hữu hạn.

## Why the fix works
`solution/Program.cs` chỉ nhóm theo route cố định và status. Với 2×2 tổ hợp tối đa là 4 series, trong khi tổng số request và phân loại status vẫn chính xác.

## How to verify
So sánh `starter/Program.cs` với `solution/Program.cs`; tự sửa starter, chạy `./verify.ps1`. Kiểm tra cả 240 request và 2 status, không chỉ kiểm tra series count. Có thể kiểm tra tham khảo: `dotnet run --project solution/ClinicTelemetry.csproj -- --verify`.

## Alternative fixes
Dùng route template đã chuẩn hóa, nhãn status class (`2xx`/`5xx`) nếu không cần status chi tiết, hoặc hạn chế whitelist label theo metric instrument. Với endpoint động, không đưa raw URL vào label.

## Wrong or misleading fixes
Tăng RAM/retention của Prometheus chỉ dời ngưỡng chịu tải; sampling metric tùy tiện làm sai counter; bỏ tất cả label làm mất khả năng phân tích lỗi theo route.

## Production implications
Giám sát active series, churn, ingestion rate và chi phí. Đặt budget/alert theo service; tách correlation ID sang structured logs/traces, bảo vệ dữ liệu cá nhân.

## Trade-offs
Ít label giảm khả năng drill-down trên metrics nhưng tăng độ bền vận hành. Chi tiết theo visit nên truy qua trace/log với chính sách retention và access phù hợp.

## Senior engineer should notice
Metric cardinality là tích số lượng giá trị label; phải đánh giá cả các trường biến thiên theo thời gian và deployment, không chỉ số request hiện tại.
