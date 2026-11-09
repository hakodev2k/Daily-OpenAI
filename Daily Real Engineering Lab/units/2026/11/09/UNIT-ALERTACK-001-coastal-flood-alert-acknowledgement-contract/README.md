# UNIT-ALERTACK-001 — Cảnh báo lũ ven biển: xác nhận nhận lệnh không đồng nghĩa còi đã hoạt động

## Mục tiêu
Thiết kế giao thức điều khiển và quan sát trạng thái cho các trạm còi cảnh báo lũ khi mạng gián đoạn. Phân biệt sự kiện được tiếp nhận với kết quả vật lý đã được xác nhận; đánh giá safety, availability và chi phí vận hành.

## Bối cảnh thực tế
Một trung tâm điều hành phát lệnh cảnh báo tới 240 gateway tại khu vực ven biển. Gateway có thể offline 5–20 phút, đồng hồ có thể lệch và lệnh đi qua broker at-least-once. Dashboard báo **240/240 đã kích hoạt** nhưng người dân ở hai khu vực không nghe còi. Sau khi trung tâm phát lệnh kết thúc cảnh báo, một gateway vừa kết nối lại bất ngờ phát còi.

## Bạn cần làm gì
1. Đọc [yêu cầu](docs/requirements.md) và [evidence](evidence/incident-timeline.csv), ghi ít nhất ba hypothesis trong [workspace](workspace/my-investigation.md).
2. Chạy replay cục bộ để xem bảng trạng thái theo từng sự kiện. Không mở solution trước.
3. Thiết kế state machine, hợp đồng command/ack và ranh giới tin cậy; điền [decision template](workspace/decision.md).
4. Xử lý thay đổi yêu cầu: trạm không có Internet trong 30 phút, nhưng phải có cơ chế cảnh báo cục bộ.
5. Kiểm tra thiết kế bằng [acceptance matrix](docs/acceptance.md), rồi đối chiếu [reference solution](solution/README.md).

## Yêu cầu môi trường
PowerShell 7+ để chạy script; không cần cloud, database hay gói NuGet. Dữ liệu CSV và PowerShell có sẵn trong unit.

## Chạy nhanh
Từ thư mục unit: `pwsh -File reproduce.ps1`. Xem các trạng thái tổng hợp và đối chiếu timeline.

## Cách reproduce vấn đề
Chạy `pwsh -File reproduce.ps1` hai lần; xác nhận số trạm hiển thị active và xem sự kiện trạm G-07 khi nối mạng lại. Đây là replay mô phỏng từ incident, không phải bằng chứng còi vật lý đã hoạt động.

## Những gì cần quan sát
- Trạng thái dashboard trước và sau sự kiện mất kết nối.
- Chênh lệch giữa thông báo của broker, gateway và tín hiệu thực tế.
- Lệnh có còn hợp lệ tại thời điểm thiết bị nhận hay không.
- Đường đi khi mất tín hiệu, nhận lặp hoặc đảo thứ tự.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc đề xuất thiết kế.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Một thiết kế có thể bảo vệ được](solution/README.md) — chỉ đọc sau khi tự phân tích.

## Expected Results
Trước: dashboard có thể báo hoàn tất dù thiếu bằng chứng từ thiết bị, và có sự kiện trễ. Sau: thiết kế phân biệt rõ mức xác nhận, trạng thái không xác định, hiệu lực lệnh và đường vận hành dự phòng; mỗi case trong matrix có cách quan sát/kiểm chứng.

## Estimated Time
85 phút.