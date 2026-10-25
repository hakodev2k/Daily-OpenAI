# Sau lab — cơ chế và quyết định kỹ thuật

**Task.WhenAll** hoàn tất khi mọi Task đầu vào hoàn tất. Nếu một hoặc nhiều task Faulted, Task tổng hợp Faulted; nếu không có lỗi nhưng có task bị cancel, Task tổng hợp Canceled. Await của một Task faulted thường ném một exception, không phải danh sách đầy đủ. Vì vậy **catch (Exception ex)** ở điểm await không tự động cung cấp báo cáo từng nguồn.

Cần tách **trạng thái điều phối** (toàn batch) khỏi **kết quả nguồn** (mỗi Task). Đọc Task.Exception của từng Task Faulted để lấy thông tin, nhưng không truy cập Task.Result khi Task chưa RanToCompletion. **Task.Exception** là AggregateException; Flatten chỉ phù hợp khi muốn chuẩn hóa các inner exceptions, và có thể làm mất cây nguyên nhân nếu hệ thống cần giữ cấu trúc.

Cancellation không phải mặc định là lỗi của đối tác. Chọn policy rõ: ở lab này, cancellation được propagate, kể cả khi một số task đã faulted; production có thể persist partial outcomes trước khi propagate. Tránh catch toàn bộ rồi nuốt lỗi không có audit trail.

Đây là mô phỏng local, không tạo HTTP request thật. Khi áp dụng vào hệ thống production, bổ sung deadline, phân biệt timeout và caller cancellation, retry có kiểm soát, tracing theo nguồn, giới hạn concurrency và kiểm thử trên adapter thực.

Tham khảo chính thức: [Task.WhenAll](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenall), [Task cancellation](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation).
