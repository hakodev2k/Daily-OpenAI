# UNIT-OTA-001 — Thiết kế rollout firmware an toàn cho 24.000 solar inverter mất kết nối

## Mục tiêu

Điều tra canary rollout bằng evidence, xây dựng architecture và operational decision record cho fleet OTA: safety invariant, capacity, partial connectivity, telemetry, rollback và trade-offs.

## Bối cảnh thực tế

Một đơn vị vận hành 24.000 inverter ở ba vùng. Phiên bản firmware mới đang được rollout dần; pilot xuất hiện kết quả không nhất quán và ACK trễ. Quản lý muốn mở cohort tiếp theo trong ngày. Bạn là Technical Lead phải ra quyết định dựa trên dữ liệu và đề xuất hệ thống có thể dừng an toàn.

## Bạn cần làm gì

1. Đọc [scenario](docs/scenario.md), [constraints](docs/constraints.md), [snapshot](evidence/pilot-snapshot.csv) và [timeline](evidence/incident-timeline.md).
2. Tính các chỉ số go/no-go và viết ít nhất ba giả thuyết.
3. Điền [Decision Record](workspace/decision-record.md): kiến trúc, state machine, invariants, capacity, failure handling, security, alternatives.
4. Walkthrough [Acceptance Matrix](docs/acceptance-matrix.md) và ghi evidence. Chỉ sau đó xem reference.

## Yêu cầu môi trường

Trình soạn thảo Markdown; PowerShell 7+ tùy chọn để đọc CSV. Không cần cloud, database, Docker, thiết bị thật hay dịch vụ trả phí. Dữ liệu synthetic.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
Import-Csv ./evidence/pilot-snapshot.csv | Format-Table
Get-Content ./evidence/incident-timeline.md
~~~

Đây là architecture tabletop, không có ứng dụng cần biên dịch.

## Cách reproduce vấn đề

Dùng pilot 08:00 và snapshot 08:55 để tái hiện quyết định mở wave 5% lúc 09:05. Tính tỷ lệ lỗi theo nhiều denominator, đối chiếu sự kiện đến trễ và duplicate trong timeline. Ghi lại các điều chưa thể kết luận khi không biết trạng thái thiết bị offline.

## Những gì cần quan sát

- Targeted, online, terminal và in-progress khác nhau như thế nào.
- ACK trễ/trùng tác động ra sao đến dashboard.
- Khả năng dừng phát lệnh mới khác với khả năng đảo ngược lệnh đã thực thi.
- Ràng buộc site, bandwidth, loại phần cứng và nhân sự vận hành.

## Quy tắc làm lab

1. Reproduce từ evidence trước.
2. Ghi hypothesis.
3. Đưa ra architecture và trade-offs.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution](solution/README.md) chỉ là một phương án có thể bảo vệ bằng bằng chứng, không phải kiến trúc duy nhất.

## Expected Results

Xem [before](expected-results/before.md), [after](expected-results/after.md) và [investigation guide](docs/investigation-guide.md).

## Estimated Time

90–110 phút (L6 / D4).
