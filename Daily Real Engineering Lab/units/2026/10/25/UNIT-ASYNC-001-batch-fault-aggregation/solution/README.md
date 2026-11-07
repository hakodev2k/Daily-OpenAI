# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
North hoàn tất 42 rows; West timeout và East sai schema. Báo cáo cấp batch lại chỉ có một lỗi generic và không có kết quả thành công.

## 2. Evidence
Đối chiếu **evidence/incident.md**, **./reproduce.ps1** và trạng thái của từng Task. Hai task Faulted, một task RanToCompletion; báo cáo không phản ánh các trạng thái đó. CPU, DB health và 404 quản trị không giải thích sự chênh lệch.

## 3. Root cause
**await Task.WhenAll(...)** ném một exception khi nhiều task faulted, mặc dù task tổng hợp có thể giữ AggregateException chứa nhiều lỗi. Khối catch của starter chỉ ghi exception được ném và trả ngay báo cáo cấp batch, bỏ qua cả kết quả thành công lẫn các lỗi khác. Ngoài ra catch (Exception) chuyển cancellation thành lỗi đối tác; lời gọi adapter ném đồng bộ còn khiến vòng tạo task dừng sớm.

## 4. Why the fix works
[BatchRunner.cs](BatchRunner.cs) biến mọi lời gọi adapter thành một Task có thể quan sát, chờ cả nhóm đến trạng thái kết thúc, rồi duyệt từng Task để giữ nguồn, dữ liệu thành công và mọi inner exception. Cancellation được ưu tiên truyền lên caller thay vì ghi thành lỗi nguồn. Trường hợp adapter ném trước khi trả Task cũng được quy về đúng nguồn.

## 5. How to verify
Trước khi so sánh, chạy **./verify.ps1** trên starter do bạn tự sửa. Nếu muốn kiểm chứng reference sau đó:
~~~powershell
Copy-Item ./starter/BatchRunner.cs ./starter/BatchRunner.mine.cs.bak
Copy-Item ./solution/BatchRunner.cs ./starter/BatchRunner.cs -Force
./verify.ps1
~~~
Sau đó khôi phục file từ **BatchRunner.mine.cs.bak** nếu cần. Không coi việc chạy reference thay cho bài làm của bạn.

## 6. Alternative fixes
- **WhenAll + inspect per-task** (reference): dễ hiểu, bảo toàn tính song song và báo cáo từng nguồn.
- **Mỗi adapter tự trả discriminated Result**: hợp lý nếu lỗi đối tác là kết quả nghiệp vụ dự kiến; cần định nghĩa rõ cancellation và lỗi lập trình.
- **Từng Task có wrapper bắt lỗi**: cho phép stream báo cáo theo nguồn, nhưng cần kiểm soát lỗi bị nuốt và hợp đồng retry.

## 7. Wrong / tempting fixes
- **Chỉ log ex.ToString() trong catch**: có thêm stack trace nhưng vẫn mất lỗi khác và thành công.
- **Đổi thành foreach await từng nguồn**: có thể làm thay đổi concurrency, dừng ở lỗi đầu nếu không bắt riêng.
- **Task.Run quanh từng adapter**: không giải quyết aggregation, còn tạo thêm scheduling.
- **catch (Exception) rồi trả success rỗng**: biến cancellation và lỗi lập trình thành dữ liệu bình thường.

## 8. Production implications
Báo cáo partial success cần hợp đồng rõ: batch có thể degraded dù một số feed hợp lệ. Gắn partner ID vào logs/traces, dùng structured events, tránh log secrets/payload nhạy cảm. Retry chỉ nguồn lỗi khi side effects cho phép và có idempotency. Không đánh đồng lỗi nghiệp vụ với lỗi điều phối.

## 9. Trade-offs
Thu thập tất cả kết quả tăng độ chính xác chẩn đoán nhưng chờ nguồn chậm nhất. Với SLA nghiêm ngặt, cần deadline chung, cancellation, giới hạn concurrency và chính sách partial result. Reference chọn **cancellation thắng mixed outcomes**; hệ thống thực tế có thể chọn lưu partial report trước khi propagate, miễn contract rõ.

## 10. Senior engineer should notice
Sự khác biệt giữa **exception surfaced by await** và **all task outcomes**; lifecycle của cancellation; ngoại lệ đồng bộ trước Task; tác động của thay đổi semantics đối với monitoring, alerting, retry và dữ liệu đã ghi.

## Official references
- [Task.WhenAll documentation](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenall)
- [Task.Exception documentation](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.exception)
- [Task.IsCompletedSuccessfully](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.iscompletedsuccessfully)
