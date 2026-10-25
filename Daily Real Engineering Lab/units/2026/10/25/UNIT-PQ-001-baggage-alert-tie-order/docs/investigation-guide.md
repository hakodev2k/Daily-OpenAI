# Investigation Guide

1. Ghi lại arrival order và severity từ starter/Program.cs; kiểm tra dữ liệu đầu vào có bị thay đổi hay không.
2. Chạy demo, reproduce, verify riêng biệt và ghi exit code từng lệnh. Reproduce thành công không đồng nghĩa verify thành công.
3. Phân loại lỗi theo hai invariant: global priority và per-priority FIFO.
4. Đọc contract của collection trước khi giả định nó giữ thứ tự của các phần tử cùng priority.
5. Thử một thay đổi nhỏ tại DispatchPlanner.Plan, chạy lại toàn bộ verify. Không sửa test oracle.
6. Đề xuất cách đo tỷ lệ đảo thứ tự và thêm correlation/arrival sequence trong hệ thống production.
