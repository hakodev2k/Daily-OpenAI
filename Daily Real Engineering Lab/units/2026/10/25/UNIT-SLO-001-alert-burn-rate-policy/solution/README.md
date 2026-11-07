# Reference Solution — CHỈ ĐỌC SAU KHI TỰ THỬ

## Symptoms
Ở peak traffic, pager kêu dù phần lớn request thành công; khi traffic ít, sự cố kéo dài lại không kích hoạt pager. Đây là lỗi **độ nhạy cảnh báo**, không phải lỗi .NET runtime.

## Evidence
- High-volume: 25/30,000 = 0.0833% lỗi, thấp hơn error budget 0.1%, nhưng ngưỡng count theo phút phát cảnh báo.
- Low-volume: 12/100 = 12% lỗi trong 30 phút, vượt xa budget nhưng ngưỡng count theo phút im lặng.
- Spike 1 phút: 15/100 trong một phút, nhưng trung bình 30 phút chỉ 0.5% và sau đó có thể phục hồi.
- Zero traffic: 0/0 không phải availability 100%.

## Root cause
`AlertPolicy.ShouldPage` quyết định bằng số lỗi tuyệt đối trong một bucket, không chuẩn hóa theo traffic, không tính mức tiêu hao error budget và không phân biệt sustained vs transient failure.

## Fix và vì sao hiệu quả
Tham khảo [AlertPolicy.cs](AlertPolicy.cs). Policy mẫu: SLO 99.9%, error budget 0.001; tính `burnRate = (errors / requests) / 0.001` trên 5 và 30 phút. Chỉ page khi **cả hai** burn rate >= 14.4. Đây là một lựa chọn được thiết kế cho bài lab, **không phải ngưỡng universal cho mọi SLO**. Cửa sổ dài loại spike đơn lẻ; cửa sổ ngắn tránh paging khi incident đã phục hồi. Với không đủ 30 bucket, không page theo policy này; trong production cần alert riêng cho missing telemetry/traffic.

## Verify
Sửa **`starter/AlertPolicy.cs`** rồi chạy `./scripts/verify.ps1`. Script chỉ chạy learner code, không dùng reference. Low-volume sustained page; healthy peak, brief spike, moderate drift, zero traffic và recovered incident không page. `reproduce.ps1` có thể thất bại sau fix.

## Alternative fixes
- Rolling error-rate threshold: đơn giản, dễ giải thích, nhưng không trực tiếp liên hệ mức tiêu hao error budget.
- Multi-window burn-rate: cân bằng sensitivity và noise, nhưng có nhiều tham số và cần đủ telemetry.
- Separate severity levels: page cho high burn, ticket cho drift; hữu ích khi có ownership rõ.
- Synthetic probes: bổ sung tín hiệu cho no-traffic và mất metrics; không thay thế hoàn toàn request-based SLI.

## Wrong / tempting fixes
- Nâng `Errors >= 20` thành 100: giảm false pages ở peak nhưng còn bỏ sót outage traffic thấp.
- Hạ threshold xuống 5: bắt được outage thấp nhưng tăng noise ở peak.
- Chỉ dùng tỷ lệ lỗi một phút: dễ page vì một spike ngắn hoặc ít sample.
- Xem 0 request là 0% lỗi rồi kết luận healthy: sai về mặt observability.
- Thêm retry vào hệ thống gửi notification để dập alert: có thể tăng load và làm thay đổi business semantics.

## Production implications / trade-offs
Cần định nghĩa lỗi theo user-visible outcome, loại bỏ health checks/internal traffic không thuộc SLI, kiểm tra metric completeness, sampling, missing data và low-traffic volatility. Có thể thêm minimum request count nhưng không được che low-volume outages; synthetic probes hoặc alert khác sẽ bù tín hiệu. Burn-rate 14.4 trên 5m+30m là policy thử nghiệm: backtest trên incident history, đo precision/recall, tuning, suppress duplicate pages, định nghĩa rollback và escalation. Đánh giá cả on-call cost, missed incidents, service dependencies và SLO window.

## Senior engineer cần nhận ra
Alert policy là **quyết định kỹ thuật có tác động vận hành**, không phải một if statement. Trước khi sửa ngưỡng, phải xác định user-impact SLI, phân biệt traffic volume, window, error budget, thiếu dữ liệu và mức độ nghiêm trọng. Một policy đúng trên fixtures chưa đủ để deploy production: cần replay/backtest với incident thật và đo false-positive/false-negative.
