# UNIT-SASREVOKE-001 — Thu hồi quyền tải hồ sơ địa chính
## Mục tiêu
Điều tra một sự cố cấp link tải PDF và đưa ra quyết định kiến trúc bảo đảm quyền truy cập được thực thi đúng thời điểm.
## Bối cảnh thực tế
Cổng hồ sơ địa chính phát link tải Azure Blob Storage cho công dân. Sau khi cán bộ thu hồi quyền, người dùng vẫn tải được tài liệu bằng link cũ.
## Bạn cần làm gì
Đọc kịch bản, đối chiếu evidence, viết ba giả thuyết có phép thử bác bỏ, đánh giá hai phương án, thực hiện change request và lập kế hoạch rollout.
## Yêu cầu môi trường
Text editor, Markdown; không cần Azure subscription. Evidence là mô phỏng.
## Chạy nhanh
Mở [scenario](docs/scenario.md), [timeline](evidence/timeline.csv), điền [workspace](workspace/my-investigation.md), chấm theo [acceptance](docs/acceptance.md).
## Cách reproduce vấn đề
So sánh thời điểm revoke với các HTTP GET trong timeline; tìm request vi phạm hợp đồng thu hồi.
## Những gì cần quan sát
Khoảng thời gian truy cập vẫn thành công, vị trí kiểm quyền, trạng thái HTTP, và bằng chứng cần đo thực tế.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix/decision.
4. Verify.
5. Sau đó mới xem solution.
## Hints
[1](hints/01.md) · [2](hints/02.md) · [3](hints/03.md)
## Reference Solution
**Spoiler:** [Reference](solution/README.md)
## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)
## Estimated Time
75 phút.
