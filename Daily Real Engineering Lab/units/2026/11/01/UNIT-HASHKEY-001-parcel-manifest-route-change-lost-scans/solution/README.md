# Reference Solution — Spoiler

## 1. Symptoms
Sau đổi tuyến, manifest có thể không tìm thấy lượt quét trước đó và lần quét mới làm tăng `TrackedParcels`.

## 2. Evidence
Ba regression checks FAIL ở starter, hai baseline PASS. `ParcelId=101` có `RouteZone` đổi 2 → 9; lookup không tìm entry cũ dù `TrackedParcels` vẫn 1. Quét lại có thể tạo entry thứ hai.

## 3. Root cause
`Dictionary<Parcel,int>` dùng `ParcelComparer`. Starter đưa cả `ParcelId` và `RouteZone` vào `Equals` và `GetHashCode`. Sau insert, `RouteZone` thay đổi khiến cùng object có hash khác, lookup tìm sai bucket. Dictionary thậm chí có thể chứa cùng object ở hai bucket. Lỗi này tái hiện không cần concurrency.

## 4. Why the fix works
Theo contract của bài, `ParcelId` là identity duy nhất, bất biến trong manifest; tuyến là trạng thái mutable. Comparer chỉ dùng `ParcelId` nên hash và equality ổn định khi đổi tuyến. Xem [Manifest.cs](Manifest.cs).

## 5. How to verify
Sau khi tự giải, có thể so sánh với reference:
```powershell
Copy-Item ./solution/Manifest.cs ./starter/Manifest.cs
./scripts/verify.ps1
```
Kỳ vọng 5/5 PASS và exit 0. Sau fix, `reproduce.ps1` báo NOT REPRODUCED và exit 1.

## 6. Alternative fixes
- `Dictionary<int,int>` theo ParcelId; lưu RouteZone ở record riêng. Dễ kiểm soát identity hơn nhưng đổi API.
- Dùng immutable composite key `(TenantId,ParcelId)` nếu ID chỉ unique trong tenant.
- Nếu tuyến thật sự là một phần identity, phải remove/reinsert theo khóa mới và định nghĩa lại hợp đồng lịch sử quét.

## 7. Wrong / tempting fixes
- `ConcurrentDictionary` không sửa key bị mutate.
- Tăng retry hoặc tạo record mới khi lookup miss che triệu chứng, làm sai thống kê.
- `lock` không khắc phục hash đổi sau insert; chỉ xử lý race riêng biệt.
- Quét tuyến tính qua mọi entry là O(n), có thể vẫn để lại dữ liệu trùng.

## 8. Production implications
Một key không ổn định có thể làm sai thống kê, audit và khả năng truy xuất. Cần kiểm tra identity ở domain boundary, kiểm thử mutation và chính sách concurrency riêng nếu hệ thống multi-threaded.

## 9. Trade-offs
Comparer theo ID giữ API hiện tại nhưng phụ thuộc cam kết ID unique. Key dạng số nguyên rõ ràng hơn; composite key cần thiết khi có nhiều tenant. Chọn theo domain contract, không chỉ theo tốc độ.

## 10. What a Senior engineer should notice
Tách identity bất biến khỏi trạng thái mutable; đảm bảo `Equals`/`GetHashCode` nhất quán; verify cả không mất dữ liệu lẫn không gộp nhầm hai kiện khác nhau.
