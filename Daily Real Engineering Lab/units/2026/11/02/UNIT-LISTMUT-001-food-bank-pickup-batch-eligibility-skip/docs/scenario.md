# Bối cảnh và contract

Một tổ chức food bank điều phối xe đến nhận các lô hàng được các nhà tài trợ đăng ký. Trước mỗi chuyến xe, ứng dụng nhận `List<DonationLot>` do bước lập lịch sở hữu, lọc các lô không thể vận chuyển rồi trả về danh sách còn lại để cấp slot cho tài xế.

Contract nghiệp vụ:

- `IsQuarantined == true`: không được đưa vào batch.
- `BestBefore < pickupDate`: đã quá hạn, không được đưa vào batch.
- `BestBefore == pickupDate`: vẫn hợp lệ nếu không bị cách ly.
- Các lô hợp lệ phải giữ nguyên thứ tự tương đối.
- Hàm `Prepare` xử lý **in-place** danh sách do caller sở hữu, trả lại chính danh sách đó; không có producer đồng thời chỉnh sửa danh sách trong lúc chạy.
- Không có I/O, database hay giao tiếp bên ngoài; lỗi có thể tái hiện hoàn toàn từ dữ liệu.

Tình huống vận hành: batch gồm hai lô quá hạn liền nhau và các lô hợp lệ phía sau. Dashboard kiểm đếm batch sau khi xử lý nhưng không kiểm tra lại từng lot. Hãy xác định vì sao một lần xử lý có thể không đạt contract, và cách đảm bảo tính đúng đắn khi batch có 0, 1 hoặc nhiều lô không hợp lệ liên tiếp.
