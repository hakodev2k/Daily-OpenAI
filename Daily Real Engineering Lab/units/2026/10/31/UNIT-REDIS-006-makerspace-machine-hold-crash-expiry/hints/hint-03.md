# Hint 03 — Hợp đồng thao tác

Hãy tìm cách thực hiện điều kiện “chỉ tạo nếu chưa tồn tại” **và** gắn TTL trong cùng một thao tác atomic. Trong Redis thật, tra cứu biến thể `SET` với `NX` và `EX`/`PX`. Kiểm tra hành vi khi lệnh đã commit nhưng client mất kết nối trước khi nhận phản hồi.
