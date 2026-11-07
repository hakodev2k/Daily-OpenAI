# UNIT-SCHEMAMIG-001 — Nâng cấp schema sổ đối soát vé xe không downtime

## Mục tiêu
Thiết kế, diễn tập và bảo vệ một phương án thay đổi schema PostgreSQL khi hai phiên bản dịch vụ chạy đồng thời; đưa ra bằng chứng về tính tương thích, toàn vẹn dữ liệu, rollback và điều kiện loại bỏ schema cũ.

## Bối cảnh thực tế
Nền tảng vé xe khu vực nhận 1.200 giao dịch/phút. Mỗi bản ghi vé có số tiền và mã tuyến. Nhóm tài chính muốn thêm khoản điều chỉnh, mã tiền tệ và phân loại nguồn thu trước kỳ đối soát. Hệ thống triển khai rolling, có consumer xử lý chậm 30 phút; không thể dừng nhận vé. Một bản release thử nghiệm làm một số giao dịch thất bại, nhưng dashboard tổng doanh thu vẫn có vẻ bình thường.

## Bạn cần làm gì
1. Đọc [bản mô tả và dữ liệu sự cố](evidence/incident.md), lập ít nhất ba giả thuyết.
2. Tạo ma trận tương thích giữa application V1/V2 và schema S0/S1/S2.
3. Viết kế hoạch triển khai từng bước, bao gồm DDL, backfill, dual-write hoặc cơ chế tương đương, kiểm tra chênh lệch, điều kiện chuyển đọc, rollback.
4. Đánh giá thay đổi yêu cầu: consumer có thể trễ tới 72 giờ, không chỉ 30 phút.
5. Đối chiếu [rubric](docs/acceptance.md) trước khi xem [reference](solution/README.md).

## Yêu cầu môi trường
Không cần cloud hoặc database. Dùng Markdown và công cụ vẽ sơ đồ tùy chọn. Nếu thử SQL trên PostgreSQL cục bộ, dùng database synthetic riêng, không chạy trên production.

## Chạy nhanh
Mở `evidence/incident.md`, sao chép `workspace/decision-record.md` làm bản làm việc của bạn. Có thể mở toàn bộ lab bằng VS Code.

## Cách reproduce vấn đề
Dựa trên lịch phát hành và ma trận ở evidence, diễn tập bằng giấy: cho V1 và V2 cùng ghi khi schema đổi, rồi cho consumer cũ đọc một message phát trước deployment. Xác định operation nào không còn hợp lệ. Đây là *architecture tabletop*, không có script reproduce giả lập runtime.

## Những gì cần quan sát
Theo dõi số bản ghi bị từ chối, độ trễ consumer, sai lệch tổng tiền theo ngày/tuyến, lock time của migration và điểm không thể rollback. Không kết luận nguyên nhân trước khi điền bảng hypothesis.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc quyết định kiến trúc.
4. Verify theo acceptance.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án tham khảo, không phải đáp án duy nhất](solution/README.md).

## Expected Results
Trước: rollout có ít nhất một đường ghi/đọc không tương thích và chưa có điểm rollback an toàn. Sau: tất cả tổ hợp ứng dụng/schema đang phục vụ đều tương thích; reconciliation phát hiện lệch; rollback được diễn giải theo từng phase.

## Estimated Time
90 phút.
