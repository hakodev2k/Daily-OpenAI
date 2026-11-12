# Reference Solution (spoiler)

`Task.WhenAny` trả Task hoàn thành đầu tiên kể cả Task faulted, không phải Task thành công đầu tiên. Await Task lỗi khiến kết quả từ backup bị bỏ qua. Duy trì tập pending, quan sát từng completion và chỉ trả lỗi tổng hợp khi mọi Task đều thất bại. Không chữa bằng cách tăng delay, bỏ qua lỗi hoặc dùng `WhenAll` vì nó tăng độ trễ và không chọn kết quả sớm. Production cần cancellation budget, giới hạn fan-out và quan sát lỗi từ các Task còn lại.
