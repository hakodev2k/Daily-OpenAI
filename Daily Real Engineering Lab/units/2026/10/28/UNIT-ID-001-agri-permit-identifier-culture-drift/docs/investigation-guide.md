# Investigation Guide — không đọc solution trước

## Incident timeline

- 09:00: Partner gửi revision `INSP-204 = RECEIVED`.
- 09:01: Partner gửi revision `insp-204 = CLEARED`.
- 09:02: Partner gửi `INSP-205 = PENDING`.
- 09:05: Hai worker báo tổng số permit khác nhau dù input hash và deployment version giống nhau.

## Thu thập evidence

Chạy `./run.ps1`, `./reproduce.ps1`, rồi `./verify.ps1` (trước khi sửa, verify phải báo FAIL). Ghi count và trạng thái từng permit. So sánh feed, quy tắc revision, identity contract và yếu tố môi trường.

## Hypotheses để kiểm chứng

- Input có thay đổi thứ tự giữa hai worker?
- Quy tắc last-revision-wins không được áp dụng?
- Một dependency hoặc cấu hình theo môi trường ảnh hưởng đến việc xác định hai ID là một?

Đưa ra một thí nghiệm nhỏ có thể loại trừ ít nhất hai giả thuyết. Không đổi dữ liệu để làm test xanh.

## Tiêu chí nghiệm thu

Mọi môi trường mô phỏng phải có đúng hai identity; `INSP-204` là `CLEARED`, `INSP-205` là `PENDING`. Kiểm tra cả tình huống chữ hoa/chữ thường và không nhập nhầm hai mã khác nhau.
