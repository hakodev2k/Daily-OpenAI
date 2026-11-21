# UNIT-MONOCLOCK-001 — Đồng hồ điều chỉnh khiến SLA vận chuyển lạnh ghi thời gian âm

## Mục tiêu
Điều tra số đo thời lượng bất thường, thu thập bằng chứng và bảo vệ tính đúng đắn của chỉ số SLA khi môi trường thay đổi.

## Bối cảnh thực tế
Dịch vụ giám sát hộp thuốc lạnh ghi nhận thời gian từ lúc nhận tới lúc bàn giao. Trong một đợt đồng bộ đồng hồ máy chủ, dashboard xuất hiện thời lượng âm, dù quá trình bàn giao thực tế vẫn mất vài giây.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai hypothesis, xác định nguồn đo đáng tin cậy cho thời lượng, sửa code tại `starter/Program.cs`, sau đó verify cả trường hợp bình thường.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell, không cần database, cloud hay API trả phí.

## Chạy nhanh
```powershell
./scripts/reproduce.ps1
./scripts/verify.ps1
```
Lệnh verify **phải thất bại trên starter gốc**; chỉ chạy thành công sau khi bạn sửa.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` từ thư mục unit. Quan sát dòng elapsed và giá trị mong đợi.

## Những gì cần quan sát
Đối chiếu thứ tự thao tác thực tế với thời lượng báo cáo. Thu thập giá trị trước/sau, các giả thuyết về hệ thống đo, và bằng chứng từ trace giả lập.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md)

## Estimated Time
35 phút.
