# UNIT-SB-001 — Message Lock Renewal

## Mục tiêu
Điều tra vì sao một Azure Service Bus worker hoàn tất business work nhưng cùng message vẫn xuất hiện lại và tạo invoice lần thứ hai.

## Bối cảnh thực tế
Worker nhận yêu cầu tạo PDF invoice. Phần render mô phỏng mất 8 giây, trong khi message lock của lab chỉ có hiệu lực 5 giây. Starter cố complete message sau khi render xong.

## Nhiệm vụ
Chạy starter để reproduce duplicate delivery, thu thập evidence từ timeline, xác định contract bị vi phạm và sửa trong `starter/` để ownership của message còn hợp lệ cho toàn bộ handler lifetime. Giải pháp cũng phải thừa nhận rằng messaging vẫn có at-least-once semantics.

## Chạy lab
- `./run.ps1`
- `./reproduce.ps1`
- Sau khi sửa: `./verify.ps1`

## Quan sát
Tập trung vào thời điểm lock được cấp, thời điểm business work kết thúc, thời điểm complete và lý do broker simulator redeliver.

## Quy tắc
Không giảm thời gian xử lý, không tăng lock duration mặc định để che triệu chứng, không bỏ bước complete, và không biến simulator thành exactly-once broker.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference Solution
Chỉ mở `solution/README.md` sau khi tự điều tra.

## Expected Result
Sau fix, `./verify.ps1` phải báo `PASS`, message chỉ tạo một business effect trong scenario và lock được giữ hợp lệ đến completion.
