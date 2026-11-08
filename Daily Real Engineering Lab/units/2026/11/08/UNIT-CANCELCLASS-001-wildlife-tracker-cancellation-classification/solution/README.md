# Reference Solution — chỉ đọc sau khi tự sửa

## Symptoms và evidence
CallerCanceled hiển thị TimedOut dù caller token đã canceled; DependencyTimeout cũng hiển thị TimedOut. Số attempt đều là 1, nên lỗi không phải retry storm.

## Root cause
Một catch rộng cho OperationCanceledException không phân biệt cancellation do caller yêu cầu với cancellation của dependency. Exception type một mình không đủ xác định nguồn; cần quan sát caller token tại boundary.

## Vì sao fix hoạt động
Filter `catch (OperationCanceledException) when (callerToken.IsCancellationRequested)` được ưu tiên trước catch còn lại. Canceled được báo riêng, dependency timeout giữ TimedOut. Lưu ý trong hệ thống thực, timeout riêng nên dùng linked CTS có timeout policy rõ ràng; trường hợp đồng thời cancel và timeout cần định nghĩa ưu tiên trong contract.

## Verify
Chạy `./verify.ps1` sau khi sửa starter; yêu cầu cả ba scenario khớp và Attempts = 1. Có thể so sánh `dotnet run --project solution/TrackerLab.csproj -- verify`.

## Alternative fixes
Trả propagation bằng `throw` cho caller cancellation thay vì status nếu API contract quy định exception. Hoặc sử dụng typed outcome từ transport để phân loại timeout không phụ thuộc exception.

## Wrong fixes / tempting fixes
Không tăng timeout mặc định: điều đó không thay đổi việc caller chủ động cancel. Không catch tất cả exception rồi trả TimedOut: sẽ che giấu lỗi khác. Không retry OperationCanceledException vô điều kiện: có thể làm việc tiếp sau khi caller hủy.

## Production implications và trade-offs
Gắn `cancellation.origin` trong structured telemetry, nhưng không ghi dữ liệu định danh nhạy cảm. Theo dõi canceled vs timeout riêng, alert trên timeout thật; thiết kế policy rõ khi hai nguyên nhân đồng thời. Senior engineer cần kiểm tra token propagation, contract hủy, budget retry, và độ tin cậy của metric trước khi thay đổi SLO.
