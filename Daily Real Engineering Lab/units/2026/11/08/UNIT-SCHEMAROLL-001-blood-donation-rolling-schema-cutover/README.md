# UNIT-SCHEMAROLL-001 — Rolling schema migration cho hệ thống hiến máu

## Mục tiêu
Thiết kế và kiểm chứng một kế hoạch thay đổi schema PostgreSQL khi hai phiên bản ASP.NET Core chạy đồng thời, có thể rollback mà không mất cập nhật.

## Bối cảnh thực tế
Nền tảng đặt lịch hiến máu của 18 trung tâm có 45.000 lượt đăng ký/ngày. Một bản phát hành đổi cách lưu lựa chọn liên hệ. Deployment rolling trong 25 phút; trong thời gian đó, V1 và V2 cùng truy cập một database. Thử nghiệm staging một lần cho thấy một số request lỗi và dashboard lệch số lượng người đồng ý nhận SMS.

## Bạn cần làm gì
1. Đọc [timeline và dữ liệu](evidence/rollout-timeline.md), phân biệt quan sát với giả thuyết.
2. Điền [workspace](workspace/decision-record.md): compatibility matrix cho V1/V2 với từng trạng thái schema, trình tự deploy/migrate/backfill, rollback và gate.
3. Đề xuất ít nhất hai phương án, chọn một và giải thích trade-off.
4. Áp dụng thay đổi requirement: rollout phải rollback trong 10 phút khi V2 lỗi ở 20% traffic.
5. Chỉ sau khi tự quyết định, đọc reference.

## Yêu cầu môi trường
Không cần cloud hay database; Markdown editor và Git đủ dùng. Có thể dùng bảng giấy. Lab là tabletop design decision, không yêu cầu chạy script.

## Chạy nhanh
Mở `evidence/rollout-timeline.md`, sau đó sao chép `workspace/decision-record.md` để điền quyết định của bạn.

## Cách reproduce vấn đề
Đối chiếu timeline 09:00–09:25 với hai hợp đồng V1/V2; đánh dấu mọi trạng thái mà một phiên bản có thể ghi thất bại hoặc dữ liệu đọc sai. Dùng bảng trạng thái để mô phỏng rollout và rollback.

## Những gì cần quan sát
Tỷ lệ lỗi theo app version, chênh lệch giữa old/new field, số bản ghi chưa backfill, tỷ lệ worker phiên bản cũ, thời gian rollback và độ tin cậy của consent audit.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc quyết định.
4. Verify bằng ma trận và các acceptance gate.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án tham khảo và trade-offs](solution/README.md).

## Expected Results
Bản thiết kế không làm mất khả năng ghi trong rolling deploy; rollback V2 không cần rollback schema phá huỷ; có phép đo xác nhận backfill và consent nhất quán; gate rõ ràng để ngăn contract sớm.

## Estimated Time
45–55 phút.
