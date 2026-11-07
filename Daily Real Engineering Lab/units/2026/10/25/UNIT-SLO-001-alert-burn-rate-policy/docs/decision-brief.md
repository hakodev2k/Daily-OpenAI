# Decision Brief — paging cho dịch vụ gửi nhắc lịch hẹn

## Contract
- Availability SLO: **99.9% successful requests** trên cửa sổ 30 ngày; error budget là 0.1% tổng request.
- Metrics được ghi theo bucket một phút, mỗi bucket có `Requests` và `Errors`.
- Paging dành cho sự cố có ảnh hưởng nghiêm trọng và **kéo dài**, không phải mọi spike đơn lẻ.
- Không page khi tỷ lệ lỗi nằm trong error budget dù số request cao.
- Không được im lặng trước lỗi kéo dài ở thời điểm ít traffic.
- Không có traffic **không chứng minh** dịch vụ khỏe; tách no-traffic / telemetry-missing alert khỏi paging error-rate.

## Nhiệm vụ thiết kế
Chọn và chứng minh policy dùng dữ liệu của **5 phút gần nhất** và **30 phút gần nhất**. Nêu cách xử lý thiếu bucket, sample size thấp, recovered incidents, alert fatigue và cost của on-call. Phân biệt page và ticket/diagnostic alert. Không sửa fixture để làm test xanh.

## Acceptance
`./scripts/verify.ps1` phải kiểm tra đúng learner policy; 6 scenarios phải có kết quả hợp lý. Hãy viết một ADR ngắn trong workspace gồm: lựa chọn, lựa chọn bị loại, tác động tới false positive/false negative, điều kiện rollback và kế hoạch đo hiệu quả.
