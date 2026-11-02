# Reference Solution — spoiler

**Chỉ đọc sau khi đã reproduce, lập hypothesis và tự sửa.**

## 1. Symptoms

Batch báo `Received=3` nhưng không lưu phiếu nào và không ghi nhận scan bị từ chối. Với nguồn replayable, importer hoạt động bình thường.

## 2. Evidence

`OneShotFeed` có `EnumerationCount=2` và `Remaining=0`; destination trống. Đây không phải lỗi kết nối lưu trữ hay business validation vì cùng fixture replayable lưu được 2 scan.

## 3. Root cause

`IEnumerable<T>` không cam kết replayable. `scans.Count()` duyệt nguồn nếu không có đường tối ưu collection-count. `OneShotFeed` tiêu thụ `Queue<T>` trong lần duyệt đầu. Vòng `foreach` thứ hai không còn phần tử, dù `Received` đã được tính là 3.

## 4. Why the fix works

Chỉ duyệt nguồn một lượt; tăng `received` ngay khi đọc từng scan, sau đó validate và lưu hoặc reject. Invariant sau khi xử lý bình thường: `Received = Stored + Rejected`. Mẫu code tương ứng trong [ScanImportService.cs](ScanImportService.cs).

## 5. How to verify

Sửa **`starter/ScanImportService.cs`**, không sửa test và không thay `OneShotFeed`. Chạy `./verify.ps1`; test phải pass trên one-shot, replayable, empty và invalid batch, với `EnumerationCount=1`. Sau khi sửa, `./reproduce.ps1` có thể không còn báo REPRODUCED — đó là đúng mục đích của script reproduce nguyên trạng.

## 6. Alternative fixes

- Materialize `scans.ToArray()` đúng một lần rồi dùng snapshot cho count và processing: hợp lý nếu batch nhỏ, cần replay, và có giới hạn kích thước.
- Single-pass processing + counting (reference): bộ nhớ O(1) ngoài destination, hợp với feed dài và streaming.
- Transactional staging trước khi acknowledge: cần khi hệ thống thật yêu cầu atomic batch và recovery sau crash; không bắt buộc trong mô phỏng nhỏ này.

## 7. Wrong or misleading fixes

- Bỏ counter `Received` hoặc đổi thành `destination.Count`: che giấu số scan bị từ chối và mất invariant.
- Gọi `Count()` lần nữa sau vòng lặp: tiếp tục duyệt nguồn, không giải quyết được one-shot contract.
- Retry toàn bộ batch mà không có idempotency: có thể tạo duplicate khi destination đã lưu một phần.
- Tăng số worker hoặc thêm cache: không liên quan đến bằng chứng.

## 8. Production implications

Chốt ranh giới ownership của stream, phân biệt accepted/processed/persisted/acknowledged, kiểm soát batch idempotency, failure partial commit và observability. Không acknowledge upstream chỉ vì đã đếm input; cần điều kiện lưu trữ phù hợp hợp đồng.

## 9. Trade-offs

Single-pass giảm memory và số lượt đọc nhưng không có snapshot để replay khi fail giữa chừng. Materialization cho phép validation toàn batch và replay, đổi lại tăng memory và độ trễ; với nguồn không bounded có nguy cơ OOM. Durable inbox/outbox phù hợp khi yêu cầu at-least-once, nhưng phức tạp hơn đáng kể.

## 10. Senior engineering insight

Kiểu `IEnumerable<T>` mô tả khả năng duyệt, không mô tả tính repeatable hay side-effect-free. Thiết kế API cần công bố rõ streaming/single-use, quyết định acknowledgment boundary và kiểm thử bằng nguồn không replayable; test chỉ dùng `List<T>` có thể che lỗi nhiều năm.
