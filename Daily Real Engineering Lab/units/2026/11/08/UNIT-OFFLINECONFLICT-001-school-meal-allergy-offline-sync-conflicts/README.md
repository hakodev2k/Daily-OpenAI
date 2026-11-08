# UNIT-OFFLINECONFLICT-001 — Đồng bộ offline danh sách dị ứng bữa ăn học đường

## Mục tiêu
Thiết kế và bảo vệ quyết định kiến trúc cho tablet phát suất ăn khi mất mạng, dữ liệu thay đổi đồng thời và yêu cầu an toàn cao.

## Bối cảnh thực tế
18 trường có 36 tablet, mất mạng tối đa 6 giờ. Trung tâm cập nhật hồ sơ dị ứng trong khi tablet offline. Khi reconnect, một số tablet hiển thị trạng thái khác server.

## Bạn cần làm gì
1. Replay E0–E6 từ [evidence](evidence/replay.md), ghi trạng thái và giả thuyết.
2. So sánh ba phương án: last-write-wins, optimistic conflict review, authoritative safety state.
3. Điền [decision workspace](workspace/decision.md) gồm API contract, failure handling, rollout và 8 test case.
4. Đối chiếu [verification checklist](docs/verification.md) trước khi xem solution.

## Yêu cầu môi trường
Markdown editor, không cần cloud hay runtime. Đây là lab kiến trúc tabletop.

## Chạy nhanh
Mở evidence, tính kết quả merge theo rule hiện tại, điền workspace và kiểm tra T1–T8.

## Cách reproduce vấn đề
Áp dụng thuật toán được mô tả trong evidence cho E0–E6, so sánh trạng thái cuối với invariant an toàn.

## Những gì cần quan sát
Khác biệt giữa local snapshot, server revision, thời điểm sự kiện và quyết định phát suất ăn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md).

## Expected Results
ADR có decision matrix, sync contract, safety invariant, 8 case được giải thích, rollout và rollback.

## Estimated Time
70 phút.
