# UNIT-CALPROV-001 — Hồ sơ chất lượng nước thay đổi sau khi đã ký

## Mục tiêu
Điều tra một sai lệch dữ liệu đã phát hành, đưa ra phương án kiến trúc có thể tái dựng báo cáo lịch sử và xử lý hiệu chỉnh hợp lệ mà không xóa dấu vết.

## Bối cảnh thực tế
Trung tâm quan trắc hồ chứa công bố báo cáo chất lượng nước hàng ngày. Một bản báo cáo đã được ký số, nhưng sau khi kỹ thuật viên cập nhật thông tin thiết bị, cùng mã báo cáo lại cho ra kết quả khác dù không có mẫu mới.

## Bạn cần làm gì
1. Đọc [timeline và dữ liệu đầu vào](evidence/timeline.md); tự tính lại hai lần xuất báo cáo.
2. Viết ít nhất ba giả thuyết, chọn bằng chứng để bác bỏ từng giả thuyết.
3. Thiết kế mô hình dữ liệu, luồng phát hành, sửa sai, kiểm toán và cách truy xuất báo cáo đã ký.
4. Kiểm tra phương án bằng [ma trận tình huống](verification/acceptance.md) trước khi mở lời giải.

## Yêu cầu môi trường
Chỉ cần trình soạn thảo Markdown; có thể dùng bảng tính hoặc SQL tùy chọn. Không yêu cầu Azure, cloud hay dữ liệu thật.

## Chạy nhanh
Sao chép bảng trong `evidence/timeline.md` vào ghi chú và thực hiện hai lần tính theo thời điểm T1, T2. Không thay đổi dữ liệu đầu vào.

## Cách reproduce vấn đề
Tính giá trị công bố của báo cáo R-204 ở T1, sau đó thực hiện đúng sự kiện cấu hình tại T2 và tính lại R-204 bằng cùng truy vấn đọc hiện hành. So sánh kết quả và audit trail.

## Những gì cần quan sát
Ghi số đo thô, phiên bản cấu hình áp dụng, giá trị tính toán, hash của bản phát hành và thông tin về người phê duyệt ở từng lần đọc. Đánh giá hệ quả với báo cáo đã ký.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Kiến trúc tham chiếu](solution/README.md).

## Expected Results
Một quyết định kiến trúc có bảng dữ liệu, ràng buộc bất biến, mô hình correction và kết quả đối chiếu đầy đủ cho các tình huống trong verification.

## Estimated Time
55–75 phút.
