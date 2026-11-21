# UNIT-BLOBSAS-001 — Quyết định ranh giới tải hồ sơ học sinh

## Mục tiêu
Đánh giá hai kiến trúc tải tệp riêng tư, lập luận bằng dữ liệu, threat model và thử nghiệm giả lập; đề xuất phương án vận hành trong giới hạn thực tế.

## Bối cảnh thực tế
Một cổng trường học phục vụ 180.000 hồ sơ PDF, 9.000 lượt tải/ngày, đỉnh 90 lượt/giây. Tệp chứa dữ liệu cá nhân. API .NET 8 hiện đọc blob rồi stream về client. Trong đợt công bố kết quả, p95 tăng từ 350 ms lên 3,8 s; băng thông App Service tăng gấp sáu. Có yêu cầu chia sẻ cho phụ huynh được ủy quyền, thu hồi quyền trong tối đa 60 giây và lưu audit tối thiểu 2 năm.

## Bạn cần làm gì
1. Đọc [scenario](docs/scenario.md), [evidence](evidence/incident.md); ghi ít nhất ba giả thuyết có thể bác bỏ.
2. Phân tích lựa chọn API proxy, short-lived delegated URL, và hybrid.
3. Dùng bảng quyết định trong [workspace](workspace/decision.md); xác định trust boundaries, audit, thu hồi, cache, failure handling, cost và cách rollout.
4. Đưa ra một thiết kế có thể kiểm chứng, kèm phương án rollback và ba phép thử thất bại.

## Yêu cầu môi trường
Không cần Azure subscription. Có thể dùng PowerShell và bảng số liệu đã cung cấp. Không cần tải dependency.

## Chạy nhanh
Mở các tài liệu theo thứ tự README → docs/scenario.md → evidence/incident.md → workspace/decision.md. Chạy kiểm tra tài liệu cục bộ: `Test-Path ./workspace/decision.md`.

## Cách reproduce vấn đề
Trong bảng evidence, so sánh tải thấp và đỉnh; tính lưu lượng outbound nếu toàn bộ 90 lượt/giây tải tệp trung bình 6 MiB. Đối chiếu với giới hạn quyền truy cập và khả năng thu hồi. Đây là design-decision lab: không có executable failure.

## Những gì cần quan sát
Độ trễ và chi phí truyền tải, khả năng kiểm soát quyền sau khi URL được cấp, rủi ro URL rò rỉ, và tính đầy đủ của audit.

## Quy tắc làm lab
1. Reproduce bằng phân tích evidence trước.
2. Ghi hypothesis.
3. Đề xuất thiết kế và thử failure cases.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ được](solution/README.md). Chỉ mở sau khi hoàn thành quyết định của bạn.

## Expected Results
Một ADR có số liệu, boundary diagram bằng văn bản, bảng trade-off, các phép thử bảo mật và kế hoạch triển khai/rollback. Không có một lựa chọn đúng tuyệt đối.

## Estimated Time
60–75 phút.
