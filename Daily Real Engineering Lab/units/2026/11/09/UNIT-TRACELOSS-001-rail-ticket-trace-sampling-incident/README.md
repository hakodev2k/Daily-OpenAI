# UNIT-TRACELOSS-001 — Điều tra trace biến mất trong thanh toán vé tàu

## Mục tiêu
Phân tích sự khác biệt giữa error metrics và distributed traces, thiết kế chính sách quan sát sự cố hiếm trong giới hạn chi phí.

## Bối cảnh thực tế
Hệ thống bán vé tàu liên tỉnh nhận 1.2 triệu request/ngày. Sau thay đổi cấu hình telemetry, dashboard vẫn ghi nhận payment timeout nhưng đội trực không tìm được trace của khách hàng.

## Bạn cần làm gì
1. Đọc dữ liệu synthetic tại `evidence/incident.md` và `evidence/traffic.csv`.
2. Tính tỷ lệ timeout từ metrics, tỷ lệ exported traces và mức độ quan sát được lỗi.
3. Ghi ít nhất ba giả thuyết, hai phép kiểm tra bác bỏ giả thuyết.
4. Đề xuất ADR về pipeline metrics/logs/traces, sampling, memory, overload, privacy và rollback.
5. Dùng replay cases tính số exported traces theo chính sách và so với ngân sách.

## Yêu cầu môi trường
PowerShell, Markdown reader; không cần tài khoản cloud.

## Chạy nhanh
```powershell
Get-Content .\evidence\incident.md
Import-Csv .\evidence\traffic.csv | Format-Table
Import-Csv .\evidence\replay-cases.csv | Format-Table
```

## Cách reproduce vấn đề
Tính tổng `payment_timeouts` và `exported_timeout_traces` trong `traffic.csv`; đối chiếu correlation IDs thiếu trace trong incident dossier.

## Những gì cần quan sát
Sai khác giữa metrics và trace, khả năng suy luận từ mẫu không đầy đủ, ngân sách khi lỗi tăng đột biến và mức độ tin cậy của telemetry pipeline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử decision/fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
⚠️ Spoiler: [Phương án tham khảo](solution/reference.md).

## Expected Results
Trước: metrics cho thấy timeout nhưng đa số timeout không có trace. Sau: thiết kế tăng khả năng điều tra lỗi, có phép đo coverage, giới hạn tài nguyên và mô tả các trường hợp vẫn có thể mất dữ liệu.

## Estimated Time
80 phút (L5 / D4).
