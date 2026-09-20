# UNIT-DESIGN-002 — CQRS Boundary for an Internal Operations Module

## Mục tiêu

Đưa ra một quyết định thiết kế có căn cứ về nơi nên và không nên áp dụng CQRS/MediatR trong một module ASP.NET Core đang phát triển nhanh.

## Bối cảnh thực tế

Team warehouse có một module nội bộ với khoảng 18 endpoints. Phần lớn là CRUD cấu hình, nhưng một số workflow như `ConfirmReceiving`, `AllocateInventory` và `CloseShipment` có validation, transaction và side effects rõ rệt. Team đang tranh luận giữa hai hướng: mọi request đều phải đi qua command/query handler để thống nhất convention, hoặc bỏ CQRS hoàn toàn để giảm abstraction.

Team có 4 backend developers, release hàng tuần, chưa có nhu cầu independent read model hay separate datastore. Production support hiện do chính team đảm nhiệm.

## Bạn cần làm gì

1. Phân loại các endpoint thành nhóm có đặc tính engineering khác nhau.
2. Đề xuất boundary cho CQRS thay vì chọn công nghệ theo convention.
3. Nêu rõ transaction ownership, validation, authorization và observability nằm ở đâu.
4. So sánh ít nhất 3 phương án.
5. Đưa ra migration plan có thể thực hiện dần, không rewrite toàn module.
6. Ghi các dấu hiệu thực tế khiến bạn sẽ xem xét lại quyết định sau 6 tháng.

## Yêu cầu môi trường

Không cần runtime. Đây là Design Decision Lab. Dùng `docs/scenario.md` và ghi quyết định vào `workspace/my-decision.md`.

## Chạy nhanh

Đọc scenario, hoàn thành decision record, sau đó mới mở reference solution.

## Cách reproduce vấn đề

Không áp dụng. Vấn đề ở đây là decision quality dưới các constraint đã cho, không phải runtime failure.

## Những gì cần quan sát

Tập trung vào complexity thật của từng workflow, số lượng cross-cutting concerns, consistency boundary, khả năng test, cognitive load và chi phí vận hành. Không coi số lượng class hay pattern name là evidence tự thân.

## Quy tắc làm lab

1. Ghi assumptions.
2. Phân loại workload.
3. So sánh alternatives.
4. Chọn boundary và bảo vệ quyết định bằng constraints.
5. Chỉ sau đó xem reference solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

[Spoiler — một defensible solution, không phải kiến trúc duy nhất đúng](solution/README.md)

## Expected Results

Một quyết định tốt phải giải thích được tại sao cùng một module có thể dùng các mức abstraction khác nhau, boundary nào cần explicit behavior model, và điều kiện nào sẽ khiến thiết kế cần thay đổi.

## Estimated Time

55 phút.