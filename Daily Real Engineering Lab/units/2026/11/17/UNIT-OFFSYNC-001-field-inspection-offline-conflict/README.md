# UNIT-OFFSYNC-001 — Đồng bộ biên bản kiểm định cầu khi mất mạng

## Mục tiêu
Đưa ra quyết định đồng bộ dữ liệu offline dựa trên bằng chứng, phân biệt xung đột nghiệp vụ và xung đột kỹ thuật, kiểm tra an toàn khi nhiều thiết bị chỉnh sửa.

## Bối cảnh thực tế
Đội kiểm định cầu nông thôn dùng ứng dụng mobile lưu offline. Một kỹ sư và một giám sát viên cùng sửa một biên bản trước khi thiết bị kết nối lại. Dashboard trung tâm báo cầu an toàn, trong khi một thiết bị ghi nhận cần đóng cầu khẩn cấp.

## Bạn cần làm gì
Đọc [scenario](scenario.md), tự tính kết quả replay theo từng chiến lược, lập decision record và viết test cases trong [workspace](workspace.md). Chỉ mở [reference](solution.md) sau khi hoàn thành.

## Yêu cầu môi trường
Không cần cloud, SDK hay API trả phí. Chỉ cần editor và PowerShell 7 hoặc công cụ đọc CSV/Markdown.

## Chạy nhanh
Mở `evidence.csv`, sắp xếp theo `receivedAt`, đối chiếu `baseVersion` với server version tại mỗi bước; ghi lại kết quả vào `workspace.md`.

## Cách reproduce vấn đề
Áp dụng policy hiện tại mô tả trong scenario lên bốn event ở evidence.csv, lần lượt theo `receivedAt`. So sánh trạng thái cuối với safety invariant.

## Những gì cần quan sát
Đếm số trường dữ liệu bị mất, số thay đổi chưa được xử lý và thời điểm dashboard thể hiện trạng thái khác với quan sát tại hiện trường. Không suy luận chỉ từ timestamp thiết bị.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử policy khác.
4. Verify bằng replay và invariant.
5. Chỉ sau đó mới xem solution.

## Hints
Xem [hints](hints.md) theo thứ tự.

## Reference Solution
**Spoiler:** [solution](solution.md).

## Expected Results
Có decision record với ít nhất hai phương án, kết quả replay từng event, quy tắc xử lý conflict, test cho retry và thứ tự đến đảo ngược, cùng rollout/observability plan.

## Estimated Time
45–60 phút.
