# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce, ghi giả thuyết và thử sửa.

## 1. Symptoms
Báo cáo bữa ăn học đường ghi thiếu nguồn thất bại trong cùng lượt tổng hợp. Lượt hủy bị phân loại thành sự cố tích hợp.

## 2. Evidence
Fixture có hai `Task.FromException` độc lập, một `Task.FromResult`; starter trả về một diagnostic. Fixture `Task.FromCanceled` không truyền cancellation cho caller. Cả hai hiện tượng có thể kiểm tra không cần network.

## 3. Root cause
`await Task.WhenAll(...)` ném một exception khi aggregate faulted; điều này **không có nghĩa** chỉ một task bị lỗi. Starter chỉ lưu `exception.Message` từ nhánh `catch`, bỏ qua tập hợp lỗi của task tổng hợp. Nhánh `catch (Exception)` còn bắt cả `OperationCanceledException`, biến hủy thành kết quả lỗi thông thường.

## 4. Why the fix works
Giữ reference đến `combined = Task.WhenAll(pending)`. Khi aggregate faulted, đọc `combined.Exception.Flatten().InnerExceptions` để giữ tất cả fault; định dạng chẩn đoán có cấu trúc và sắp xếp để dễ đối chiếu. Khi aggregate chỉ bị canceled, rethrow `OperationCanceledException`. Chỉ publish khi tất cả nguồn thành công.

Mã tham khảo: [SupplierReport.cs](SupplierReport.cs). File này **không** được harness compile; người học tự sửa `starter/SupplierReport.cs`.

## 5. How to verify
Từ unit root, chạy `pwsh -File ./verify.ps1`. Harness kiểm tra: tổng bình thường bằng 7; hai lỗi được giữ đủ bất kể thứ tự; cancellation được propagate. Sau sửa, `reproduce.ps1` không còn tìm thấy lỗi và trả exit 1.

## 6. Alternative fixes
- Quan sát từng task riêng sau khi chúng hoàn tất và gom `Exception` theo source; phù hợp nếu cần gắn metadata riêng cho mỗi nhà cung cấp.
- Dùng result type biểu diễn success/failure/cancel cho từng source, rồi aggregate có chủ đích; hữu ích nếu partial success là yêu cầu nghiệp vụ được định nghĩa rõ.
- Một `Task.WhenAll` với callback telemetry per-source cũng được, nhưng phải bảo đảm callback thread-safe và không làm mất correlation.

## 7. Wrong or misleading fixes
- Chỉ log exception từ `catch`: vẫn mất những fault còn lại.
- Retry toàn bộ batch ngay: có thể làm tăng tải, nhân đôi tác dụng phụ và không cải thiện chẩn đoán.
- Bắt `OperationCanceledException` rồi trả `Published=false`: caller không còn phân biệt hủy có chủ đích với incident.
- Publish kết quả nguồn thành công khi thiếu nguồn bắt buộc: vi phạm hợp đồng báo cáo; chỉ phù hợp nếu business đã chấp thuận partial report.

## 8. Production implications
Ghi `runId`, `supplierId`, `errorCode`, fault count, cancellation count và correlation ID. Không log payload dị ứng hay exception message thô có dữ liệu nhạy cảm. Chỉ tăng incident metric khi fault thực sự; đo retry theo nguồn và xác định idempotency trước khi thử lại.

## 9. Trade-offs
Chờ tất cả task giúp thu đủ lỗi nhưng có thể kéo dài latency nếu một nguồn treo; cần timeout/cancellation policy riêng ở từng dependency. Fail-fast có thể giảm latency nhưng mất khả năng thu đủ bằng chứng. Lựa chọn phụ thuộc SLA, yêu cầu audit và mức độ bắt buộc của từng nguồn.

## 10. What a Senior engineer should notice
Exception được ném từ `await` là **giao diện control flow**, không phải toàn bộ evidence của các task song song. Cần phân biệt fault, cancellation và partial result; thiết kế telemetry và điều kiện publish theo hợp đồng nghiệp vụ thay vì dựa vào một message.
