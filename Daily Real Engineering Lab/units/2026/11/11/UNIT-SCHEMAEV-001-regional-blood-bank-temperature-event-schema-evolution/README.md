# UNIT-SCHEMAEV-001 — Schema evolution cho sự kiện nhiệt độ kho máu

## Mục tiêu
Đưa ra quyết định migration hợp đồng event khi producer và consumer không thể nâng cấp đồng thời.

## Bối cảnh thực tế
420 tủ bảo quản gửi telemetry mỗi 30 giây. Firmware mới được triển khai trong 21 ngày; dữ liệu cũ phải replay được trong 90 ngày. Một số cảnh báo nhiệt độ vượt 6°C biến mất dù broker báo thành công.

## Bạn cần làm gì
Đọc incident và JSONL; lập compatibility matrix giữa hai consumer và hai firmware; nêu ba giả thuyết, phép thử; viết ADR so sánh ít nhất hai phương án; xây contract test matrix, rollout 5% → 25% → 100%, rollback và xử lý thay đổi yêu cầu.

## Yêu cầu môi trường
PowerShell 7 hoặc bất kỳ JSON viewer; không cần dịch vụ cloud.

## Chạy nhanh
Tại thư mục unit:
```powershell
Get-Content .\evidence\events.jsonl | ForEach-Object { $_ | ConvertFrom-Json | Select-Object deviceId,temperatureC,celsius,schemaVersion }
```

## Cách reproduce vấn đề
Dùng contract của consumer trong `evidence/incident.md` để phân loại từng dòng fixture và ghi kết quả vào `workspace/my-investigation.md`. So sánh expected alert với alert thực tế theo từng consumer.

## Những gì cần quan sát
Khác biệt giữa broker ACK và business processing; version coverage, invalid payload, replay và khả năng rollback.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference decision](solution/README.md) chỉ xem sau khi tự hoàn thành ADR.

## Expected Results
Ma trận tương thích, contract tests, chính sách invalid event, rollout/rollback gates và ADR bảo vệ được trước rủi ro mất cảnh báo.

## Estimated Time
90 phút.
