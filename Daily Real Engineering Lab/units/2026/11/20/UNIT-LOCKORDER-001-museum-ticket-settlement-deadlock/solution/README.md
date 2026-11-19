# Reference solution — chỉ xem sau khi thử

## Root cause
Hai worker giữ monitor khác nhau trước khi gặp barrier, sau đó mỗi worker đòi monitor mà worker còn lại đang giữ. Đây là circular wait. Không có exception và không nhất thiết tiêu thụ CPU. Barrier chỉ là công cụ đồng bộ thời điểm để tái hiện interleaving; không nên giữ nó bên trong critical section trong bản sửa.

## Cách sửa
Đặt thứ tự khóa nhất quán theo `Counter.Id`, giữ kiểm tra số dư và hai phép cập nhật trong cùng critical section. Thay nội dung `starter/Program.cs` bằng [Program.cs tham khảo](Program.cs) sau khi đã thử.

## Vì sao hiệu quả
Cả hai worker đều khóa counter Id nhỏ trước, counter Id lớn sau; không thể hình thành chu trình chờ khóa. Tính toàn vẹn tổng vé được kiểm tra sau `Task.WhenAll`.

## Wrong fixes
- Tăng timeout: không giải quyết circular wait.
- Thêm `Task.Delay`: chỉ làm lỗi ít xuất hiện, không có bảo đảm.
- Chỉ dùng `Interlocked` cho từng Balance: không bảo đảm chuyển vé gồm hai cập nhật là một thao tác nguyên tử.
- Bỏ khóa hoàn toàn: có thể tạo số dư sai khi có nhiều chuyển vé.

## Trade-offs
Lock ordering dễ kiểm chứng cho bộ nhớ trong một process nhưng không thay thế transaction ở nhiều instance/database. Khi mở rộng nhiều counter, cần định nghĩa thứ tự toàn cục và đánh giá contention. Với production, cân nhắc log thời gian chờ và kiểm tra invariant trên workload rộng hơn.

