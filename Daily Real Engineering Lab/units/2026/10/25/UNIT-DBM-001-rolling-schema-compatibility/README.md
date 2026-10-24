# UNIT-DBM-001 — Giữ tương thích database trong rolling deployment

## Mục tiêu

Thiết kế và bảo vệ một kế hoạch thay đổi schema SQL Server khi hai phiên bản ASP.NET Core API cùng phục vụ traffic. Luyện phân tích compatibility, thứ tự release, data correctness, rollback và kiểm chứng bằng evidence thay vì chọn giải pháp theo thói quen.

## Bối cảnh thực tế

Nền tảng quản lý đối tác B2B có 4 API pods triển khai rolling. Một release bổ sung cách hiển thị tên liên hệ mới. Trong đợt canary, một số request đọc và ghi hồ sơ thất bại; rollback ứng dụng cũng không đưa hệ thống trở lại bình thường. Product yêu cầu không downtime, không mất thay đổi của người dùng, và có thể rollback ứng dụng trong 24 giờ.

## Bạn cần làm gì

1. Đọc [constraints](docs/scenario.md) và [incident evidence](evidence/incident.md); tự lập compatibility matrix cho các cặp application version × schema version.
2. Đề xuất **ít nhất hai** chiến lược rollout, nêu assumptions và rủi ro về concurrent writes, backfill, rollback, DDL locks.
3. Điền [workspace/my-decision.md](workspace/my-decision.md): release stages, gates, rollback, observability, test cases và quyết định cuối.
4. Dùng timeline và ma trận để thử bác bỏ kế hoạch của chính bạn, đặc biệt khi các pods chạy khác phiên bản.
5. Sau khi tự quyết định, mở reference solution để so sánh.

## Yêu cầu môi trường

Không cần Azure subscription, SQL Server instance hay package trả phí. Đây là **L5 Design Decision Lab**, không phải executable bug lab; tài liệu cung cấp mô phỏng release và evidence để bạn phân tích. Có thể dùng Markdown để vẽ sequence/compatibility matrix.

## Chạy nhanh

Mở `docs/scenario.md` → `evidence/incident.md` → hoàn thành `workspace/my-decision.md`. Thời gian mục tiêu: 60–75 phút.

## Cách reproduce vấn đề

Không có script chạy ứng dụng: tái hiện **quyết định triển khai** bằng cách lần lượt áp dụng các mốc trong evidence lên ma trận phiên bản. Với mỗi mốc, đánh dấu thao tác read/write nào còn hợp lệ và liệu rollback có an toàn không. Không cần thay đổi production database.

## Những gì cần quan sát

- Tỷ lệ lỗi theo pod version và theo operation, không chỉ error rate tổng.
- Thời điểm DDL, app rollout và rollback chồng lấn.
- Những giá trị người dùng cập nhật trong thời gian coexistence có thể khác nhau giữa các phiên bản hay không.
- Điều kiện kiểm chứng trước khi loại bỏ compatibility layer.

## Quy tắc làm lab

1. Reproduce bằng timeline và ma trận trước.
2. Ghi hypothesis, assumptions và evidence còn thiếu.
3. Thiết kế rollout/rollback và thử các failure scenarios.
4. Verify bằng acceptance matrix, không chỉ bằng một happy-path query.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md). Mở lần lượt khi cần.

## Reference Solution

⚠️ **Spoiler:** [Một phương án có thể bảo vệ được](solution/README.md) — chỉ đọc sau khi hoàn thành workspace.

## Expected Results

[Trước khi sửa kế hoạch](expected-results/before.md) · [Sau khi sửa kế hoạch](expected-results/after.md). Đáp án được đánh giá theo compatibility, data correctness, rollback và operational gates, không phải số lượng thành phần kiến trúc.

## Estimated Time

60–75 phút. Tài liệu chính thức: [references](docs/references.md).
