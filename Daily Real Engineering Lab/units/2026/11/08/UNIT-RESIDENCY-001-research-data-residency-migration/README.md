# UNIT-RESIDENCY-001 — Chuyển vùng dữ liệu tenant khi worker vẫn đang ghi

## Mục tiêu
Thiết kế và kiểm chứng kế hoạch chuyển vùng lưu trữ tenant từ EU-West sang EU-Central mà không mất dữ liệu, không tạo bản ghi sai vùng sau cutover và có thể chứng minh bằng audit.

## Bối cảnh thực tế
Nền tảng SaaS quản lý mẫu nghiên cứu vật liệu có API .NET, PostgreSQL, object storage và queue workers. Tenant **Atlas** yêu cầu mọi dữ liệu cá nhân và kết quả phân tích mới phải ở EU-Central từ 09:00 UTC. Dịch vụ vẫn phải hoạt động trong giới hạn downtime 5 phút.

## Bạn cần làm gì
1. Đọc [ràng buộc và evidence](evidence/incident.md), phân loại rủi ro theo đường ghi.
2. Vẽ luồng dữ liệu, thiết kế ít nhất hai phương án cutover và chọn một phương án có lập luận.
3. Điền [workspace quyết định](workspace/my-investigation.md) với timeline, invariant, kiểm thử, rollback.
4. Chạy tabletop theo [kịch bản kiểm chứng](docs/verification.md); ghi lại các trường hợp pass/fail.
5. Chỉ sau khi tự quyết định mới mở [reference solution](solution/README.md).

## Yêu cầu môi trường
Chỉ cần Markdown editor và đồng hồ UTC; không cần Azure hoặc database thật. Đây là architecture tabletop, không phải executable lab.

## Chạy nhanh
Mở `evidence/incident.md`, sao chép template trong `workspace/my-investigation.md` và replay từng sự kiện của `docs/verification.md`.

## Cách reproduce vấn đề
Dựa vào timeline của evidence, mô phỏng chuyển router sang EU-Central tại 09:00, sau đó cho các message tồn đọng tiếp tục được worker xử lý. Ghi lại vùng lưu thực tế của từng side effect.

## Những gì cần quan sát
So sánh số event đã nhận, xử lý, từ chối và còn pending theo tenant, region và routing epoch. Kiểm tra có artifact nào xuất hiện sai vùng sau mốc cutover hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Đọc sau khi hoàn thành phương án riêng](solution/README.md).

## Expected Results
[Tiêu chí kiểm chứng](docs/verification.md): không có write mới sai vùng; mọi message được xử lý đúng một hiệu ứng nghiệp vụ hoặc được đưa vào quarantine có thể truy vết; cutover có điều kiện go/no-go rõ ràng.

## Estimated Time
75 phút.