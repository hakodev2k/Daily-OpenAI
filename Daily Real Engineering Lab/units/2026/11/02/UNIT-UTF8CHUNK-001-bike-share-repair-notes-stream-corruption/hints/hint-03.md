# Hint 03 — Giữ trạng thái và fail closed
Đọc contract của `System.Text.Decoder` và `UTF8Encoding(false, true)`. Một decoder có trạng thái qua nhiều chunk và flush đúng cuối thông điệp có giúp giữ byte dở dang không? Kiểm tra cả byte lỗi và chuỗi kết thúc chưa hoàn chỉnh.
