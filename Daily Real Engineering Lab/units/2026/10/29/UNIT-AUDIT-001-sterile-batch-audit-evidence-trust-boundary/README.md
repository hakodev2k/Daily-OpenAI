# UNIT-AUDIT-001 — Khi bản xuất audit của lô vô trùng không còn khớp

## Mục tiêu

Thiết kế và bảo vệ một quyết định kiến trúc về **tính toàn vẹn có thể kiểm chứng** của audit trail: phân tích bằng chứng, xác định trust boundary, so sánh phương án, xây dựng cơ chế xác minh và kế hoạch rollout/rollback. Đây là lab **L6 Architecture / Solution, D4**, không phải bài sửa một dòng code.

## Bối cảnh thực tế

Một nền tảng quản lý phát hành lô vô trùng phục vụ 12 cơ sở sản xuất. Bản xuất audit của lô \`B-4721\` ở hai thời điểm có nội dung khác nhau, trong khi API export vẫn trả thành công. Một bộ phận tuân thủ đang giữ một receipt độc lập được phát hành trước đó. Đội kỹ thuật cần đánh giá liệu hệ thống có thể chứng minh lịch sử phê duyệt hay không.

## Bạn cần làm gì

1. Đối chiếu hai snapshot, receipt và incident timeline; ghi rõ **quan sát** khác với **giả thuyết**.
2. Vẽ data flow hiện tại, quyền quản trị và các trust boundary.
3. Lập ADR chọn kiến trúc có thể kiểm chứng cho 5 năm lưu trữ; so sánh ít nhất ba phương án và nêu giới hạn bảo đảm.
4. Đề xuất giao thức kiểm tra, vận hành, migration, failure handling và bằng chứng nghiệm thu.
5. Giải quyết thay đổi yêu cầu trong [senior challenge](extensions/senior-challenge.md).

## Yêu cầu môi trường

Git, PowerShell 7 hoặc Windows PowerShell 5.1, trình soạn thảo Markdown. Không cần Azure, Docker, NuGet hay tài khoản trả phí. Đây là architecture tabletop dựa trên fixture cố định, không có ứng dụng starter phải build.

## Chạy nhanh

Từ root repository:

\`\`\`powershell
cd "Daily Real Engineering Lab/units/2026/10/29/UNIT-AUDIT-001-sterile-batch-audit-evidence-trust-boundary"
Import-Csv .\evidence\snapshot-t0.csv | Where-Object BatchId -eq 'B-4721' | Format-Table
Import-Csv .\evidence\snapshot-t1.csv | Where-Object BatchId -eq 'B-4721' | Format-Table
Get-Content .\evidence\independent-receipt.json
\`\`\`

## Cách reproduce vấn đề

\`\`\`powershell
$a = Import-Csv .\evidence\snapshot-t0.csv
$b = Import-Csv .\evidence\snapshot-t1.csv
Compare-Object ($a | Where-Object BatchId -eq 'B-4721') ($b | Where-Object BatchId -eq 'B-4721') -Property EventId,Sequence,Action
\`\`\`

Đối chiếu kết quả với [timeline](evidence/incident-timeline.md) và receipt. Fixture cố định nên người học phải quan sát được cùng một khác biệt trên mọi máy.

## Những gì cần quan sát

Ghi số sự kiện, thứ tự, actor, hành động và khoảng thời gian của từng snapshot. Xác định bằng chứng nào độc lập, bằng chứng nào cùng phụ thuộc một hệ thống. Đừng suy luận nguyên nhân từ riêng HTTP 200 hoặc một dòng log.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử thiết kế phương án.
4. Verify bằng ma trận acceptance.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi đã tự lập ADR và chạy acceptance review:** [Một phương án tham chiếu](solution/README.md).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md) · [Acceptance tests](docs/acceptance-tests.md)

## Estimated Time

80–100 phút. Ghi kết quả vào [workspace](workspace/my-investigation.md). Đọc [scenario](docs/scenario.md) trước khi quyết định.
