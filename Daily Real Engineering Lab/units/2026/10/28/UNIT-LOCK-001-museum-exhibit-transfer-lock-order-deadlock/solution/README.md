# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Hai transfer ngược chiều không hoàn thành; CPU thấp, không có exception nghiệp vụ.

## 2. Evidence
Fixture sử dụng hai thread nền và `Barrier` để tái hiện một interleaving cụ thể. Bản gốc ghi `DEADLOCK_DETECTED`, exit code 2. Mỗi thread đã giữ một monitor mà thread kia cần.

## 3. Root cause
`Transfer` khóa source rồi destination. NORTH→SOUTH và SOUTH→NORTH đồng thời tạo vòng chờ NORTH→SOUTH→NORTH (circular wait). Đây là deadlock do lock-order inversion.

## 4. Why the fix works
Mọi operation lấy hai monitor theo cùng total order bằng `Gallery.Code`, không phụ thuộc chiều chuyển. Không thể hình thành vòng chờ. Việc cập nhật cả hai kho vẫn nằm trong critical section. [Reference code](GalleryInventory.cs).

## 5. How to verify
Chỉ sửa `starter/GalleryInventory.cs`, sau đó chạy `./verify.ps1`. Kỳ vọng exit code 0 và `PASS: both transfers completed; NORTH=10; SOUTH=10; TOTAL=20`. `reproduce.ps1` chỉ dùng cho bản gốc.

## 6. Alternative fixes
Một lock chung dễ hiểu nhưng serialize toàn bộ transfer. Database transaction với canonical row-lock order thích hợp hơn khi nhiều replica cùng cập nhật; cần isolation, timeout và retry có giới hạn. Actor/single-writer hợp lý nếu phù hợp throughput và vận hành.

## 7. Wrong or tempting fixes
Tăng timeout chỉ kéo dài treo; `Task.Run` hay tăng ThreadPool không loại circular wait; bỏ lock làm hỏng invariant; retry vô hạn tăng contention.

## 8. Production implications
`Monitor` chỉ bảo vệ trong **một process**, không bảo đảm invariant khi nhiều pod ghi chung database. Cần telemetry về operation ID, lock wait, transaction deadlock, timeout và correlation ID.

## 9. Trade-offs
Total order giảm deadlock nhưng vẫn có contention; phải thống nhất ordering cho **mọi** code path, giảm phạm vi critical section, và không `await` bên trong C# `lock`.

## 10. Senior engineer should notice
Chứng minh liveness **và** correctness; phân biệt deadlock với starvation; kiểm tra lock graph; nêu rõ ranh giới process/distributed; tránh tối ưu trước khi có evidence.

### Fixture note
`Barrier` và callback `afterFirstResourceAcquired` chỉ để tạo lịch chạy có kiểm soát, không phải thiết kế production. Khi dùng lock order thống nhất, callback có thể timeout vì thread thứ hai chưa vào first lock; fixture vẫn hoàn thành và invariant giữ nguyên.
