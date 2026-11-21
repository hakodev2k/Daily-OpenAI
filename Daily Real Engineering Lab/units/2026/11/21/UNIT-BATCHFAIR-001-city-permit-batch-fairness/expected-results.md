# Kết quả mong đợi

## Trước

Với 8 worker, 48 việc A đến phút 0 lấp kín sáu đợt phút 0–5. U01 đến phút 1 nhưng không được bắt đầu trước phút 6; thời gian chờ 5 phút, vượt SLO 3 phút. U02 cũng phải đợi tới phút 6 (4 phút). Throughput vẫn đạt 8 việc/phút khi backlog đủ lớn.

## Sau

Một chính sách hợp lệ bắt đầu U01 không muộn hơn phút 4 và U02 không muộn hơn phút 5, giữ 8 worker hữu dụng khi backlog có đủ việc; chứng minh fairness và bound starvation. Nếu sử dụng strict priority, phải nêu biện pháp chống bỏ đói normal. Không thay đổi workMinutes hoặc loại bỏ việc để làm đẹp số liệu.

## Troubleshooting

Nếu kết quả FIFO khác, kiểm tra định nghĩa arrivalMinute, thứ tự tie-break theo id và việc worker chỉ nhận task khi đã rảnh.
