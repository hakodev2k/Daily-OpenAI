# Dữ liệu điều tra (synthetic, cố định)

Dữ liệu cho một cửa sổ 8 phút 20 giây: **1.000.000 requests**, **1.000 errors** (0,1%). Giả định độc lập giữa outcome và hash trace-id trong phương án head-sampling.

- 1.000 errors gồm 850 HTTP 5xx và 150 timeout; 80 errors là cảnh báo ưu tiên P0.
- 999.000 successes; 1 trace hoàn chỉnh trung bình 40 KiB sau nén.
- Ngân sách lưu: **tối đa 80.000 traces** / cửa sổ; lưu 30 ngày.
- Head sampler hiện tại giữ 5% trace-id đồng nhất; mỗi trace được chọn có đủ spans khi mọi service cùng tuân quyết định.
- Chỉ 47 error traces xuất hiện trong kho trong cửa sổ quan sát (dao động mẫu), nhưng metrics đếm đủ 1.000 lỗi.
- Phần lớn lỗi xảy ra sau khi request đã đi qua ingress. Đánh giá outcome chỉ ở cuối request.
- Tail collector giả định giữ toàn bộ spans trong RAM tới khi có outcome; mức trần an toàn của collector: 15.000 traces đang chờ, tối đa 20 giây.
- Trễ collector không được chặn đường xử lý cảnh báo; collector hỏng không được làm request thất bại.
- Privacy: payload chứa địa chỉ dân cư, số điện thoại; trace export không được chứa payload thô.
- Nhánh gateway -> routing -> notification có 3 service; partial traces không được tính là hoàn chỉnh.

## Mẫu dự kiến theo xác suất
Với head-sampling 5%, kỳ vọng giữ 50/1.000 error traces và 49.950 successes, tổng 50.000. Số 47 là số quan sát thực tế, không phải lỗi tính toán.

## Điều kiện acceptance
- Chứng minh tổng retained traces không vượt 80.000 theo mô hình đã chọn.
- Tối đa hóa khả năng điều tra errors, đặc biệt P0, mà không làm mất observability metrics.
- Nêu rủi ro tail buffer, partial trace, PII, chi phí và kịch bản collector overload.
- Phân biệt giá trị kỳ vọng với cam kết tuyệt đối; thiết kế giám sát và rollback.
