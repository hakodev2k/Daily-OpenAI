# UNIT-OUTBOXBOUND-001 — Ranh giới phát sự kiện trong mạng lưới ngân hàng máu

## Mục tiêu
Thiết kế và kiểm chứng ranh giới nhất quán giữa việc xác nhận một đơn vị máu trong SQL Server và thông báo sang hệ thống điều phối, khi tiến trình có thể chết ở bất kỳ điểm nào.

## Bối cảnh thực tế
Một trung tâm phân phối tiếp nhận 18.000 thay đổi trạng thái/ngày từ 12 bệnh viện. Dịch vụ ASP.NET Core lưu trạng thái kiểm định và phát `BloodUnitReleased` cho các hệ thống đặt lịch vận chuyển. Đôi khi dashboard ghi nhận đơn vị đã sẵn sàng nhưng hệ thống vận chuyển không nhận thông báo; ở một số ca khác, một thông báo xuất hiện hai lần.

Giả định bài tập: một SQL Server, một broker tương thích at-least-once, 3 application replicas; 99.9% sự kiện phải được giao trong 60 giây khi hạ tầng khỏe; không được vận chuyển đơn vị chưa được duyệt; sự kiện trùng không được tạo hai lệnh vận chuyển. Không có distributed transaction chung giữa DB và broker.

## Bạn cần làm gì
1. Đọc [timeline sự cố](evidence/incident-timeline.md) và [hợp đồng](docs/constraints.md).
2. Viết ít nhất ba hypothesis, xác định bằng chứng phân biệt từng hypothesis.
3. Điền [workspace](workspace/my-investigation.md): chọn ranh giới commit, cơ chế phát sự kiện, retry, idempotency, ordering và recovery.
4. Mô phỏng thủ công các điểm crash theo [test matrix](docs/failure-matrix.md), ghi kết quả của thiết kế.
5. Chỉ sau đó đọc [hints](hints/hint-01.md) theo thứ tự và [reference solution](solution/README.md).

## Yêu cầu môi trường
Chỉ cần Markdown editor; không cần Azure subscription, broker hay SQL Server để thực hiện tabletop. Có thể vẽ sequence diagram bằng Mermaid.

## Chạy nhanh
`git pull`; mở `docs/failure-matrix.md`, đánh dấu từng ca PASS/FAIL cho phương án của bạn. Không có script runtime vì đây là L5 design-decision lab.

## Cách reproduce vấn đề
Lần theo hai lịch trình crash trong `evidence/incident-timeline.md`, chỉ rõ trạng thái DB, broker và downstream sau mỗi bước. Đây là replay logic, không phải test đã chạy trên hạ tầng thật.

## Những gì cần quan sát
Sự lệch nhau giữa trạng thái nghiệp vụ đã commit, thông điệp đã được broker chấp nhận và tác động cuối cùng ở consumer. Phân biệt mất sự kiện, trùng sự kiện, và xử lý sai thứ tự.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng failure matrix.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phương án tham chiếu và trade-offs](solution/README.md) — chỉ xem sau khi tự thiết kế.

## Expected Results
Một thiết kế có thể hồi phục sau crash, không mất thông báo đã commit trong điều kiện hệ thống cuối cùng phục hồi, và không tạo vận chuyển trùng; có kế hoạch đo độ trễ, backlog và xử lý poison message. Không được tuyên bố exactly-once end-to-end.

## Estimated Time
65 phút.
