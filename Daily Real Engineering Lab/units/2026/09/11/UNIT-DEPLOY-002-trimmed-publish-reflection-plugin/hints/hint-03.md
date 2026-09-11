# Hint 3

`PublishTrimmed` có thể loại bỏ code không được xem là reachable. Một hướng sửa bền vững là tạo **explicit registration/factory path** mà compiler/trimmer nhìn thấy; một hướng khác là khai báo preservation contract phù hợp khi reflection là yêu cầu thật sự.