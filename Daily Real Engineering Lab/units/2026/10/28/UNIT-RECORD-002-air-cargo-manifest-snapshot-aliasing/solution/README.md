# Reference Solution — chỉ xem sau khi đã reproduce và tự sửa

## 1. Symptoms
Manifest đầu tiên được chụp khi mới có `BAG-01`, nhưng sau scan tiếp theo lại chứa thêm `BAG-02`. Người đọc còn có thể thêm một bag tag không qua nghiệp vụ scan, khiến manifest đang hoạt động thay đổi.

## 2. Evidence
`first_snapshot_count_after_next_scan=2`, `reader_could_mutate_snapshot=True`, `active_manifest_unchanged_by_reader=False`. Timestamp và chuyến bay độc lập vẫn đúng, nên đây không phải lỗi thời gian hay định tuyến chuyến bay.

## 3. Root cause
C# `record` hỗ trợ `with` bằng **shallow copy**. Thuộc tính `IReadOnlyList<string>` không làm cho object thực tế trở thành immutable: starter chứa `List<string>` có thể sửa. Nhiều bản record cùng trỏ đến một danh sách; cast sang `IList<string>` có thể ghi xuyên qua lớp interface chỉ đọc.

## 4. Why the fix works
Tại mỗi `ScanBag`, tạo một danh sách mới từ nội dung cũ, thêm tag mới rồi xuất bản một read-only wrapper trên mảng riêng. Các snapshot cũ giữ nguyên object dữ liệu đã xuất bản. `Capture` có thể dùng `with` cho timestamp vì collection phía trong đã không còn bị mutate.

## 5. How to verify
Chạy `./verify.ps1` **sau khi sửa starter/**. Để tự kiểm tra reference:
```powershell
dotnet run --project ./solution/ManifestAudit.csproj -- verify
```
Kỳ vọng mọi điều kiện đều True theo [after](../expected-results/after.md). Bài kiểm tra learner không phụ thuộc vào solution project.

## 6. Alternative fixes
- Tạo deep copy mỗi khi `Capture` và chỉ trả về collection read-only. Có thể đủ nếu tất cả đường đọc đều đi qua Capture; phải đánh giá các reference đã thoát ra trước đó.
- Sử dụng `ImmutableArray<string>` / immutable collections với immutable update, phù hợp nếu có nhiều consumer và yêu cầu snapshot integrity cao; cần cân nhắc allocation và chi phí dependency.

## 7. Wrong / tempting fixes
- Chỉ thay `List<string>` thành `IReadOnlyList<string>` trong khai báo: interface không khóa object thực tế.
- Chỉ dùng `with` để tạo record mới: không sao chép sâu các reference property.
- Chỉ copy tại một điểm xuất dữ liệu nhưng vẫn trả mutable reference ở điểm khác: invariant không được đảm bảo toàn hệ thống.

## 8. Production implications
Manifest audit có thể được ký số, lưu kho, hoặc dùng làm căn cứ đối soát. Dữ liệu biến đổi sau khi chụp phá vỡ provenance và có thể tạo tranh chấp nghiệp vụ. Với nhiều thread, cần thêm synchronization/atomic publication; bản reference hiện chỉ mô phỏng single-thread.

## 9. Trade-offs
Copy-on-write là O(n) mỗi lần scan, phù hợp với fixture nhỏ nhưng không mặc nhiên phù hợp chuyến bay có hàng trăm nghìn bag tags. Với throughput cao, cân nhắc immutable persistent data structures, append-only event log hoặc snapshot tại checkpoint. Tránh tối ưu trước khi đo.

## 10. Senior engineer should notice
Phải xác định **ownership** và ranh giới xuất bản dữ liệu. Contract `IReadOnlyList<T>` không bảo đảm transitive immutability. Cần test cả tính ổn định theo thời gian lẫn khả năng mutate từ phía consumer, và đánh giá race condition nếu production concurrent.

Reference implementation: [ManifestService.cs](ManifestService.cs).
