# Reference Solution — UNIT-SEC-004

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Package import hoàn tất nhưng một archive entry có thể tạo file bên ngoài directory mà service được phép ghi.

## 2. Evidence

Starter package chứa một entry hợp lệ và một entry có path đi lên parent directory. `reproduce.ps1` cho thấy file hợp lệ được import, đồng thời `outside.txt` xuất hiện ngoài import root.

## 3. Root cause

Importer tin tưởng `ZipArchiveEntry.FullName` và chỉ `Path.Combine` với destination root. Entry path có traversal segment được filesystem resolve ra vị trí nằm ngoài root. `Path.Combine` không phải là security containment check.

## 4. Why the fix works

Reference implementation canonicalize destination root và target bằng `Path.GetFullPath`, sau đó chỉ extract khi canonical target nằm dưới canonical root. So sánh path dùng semantics phù hợp với Windows và Unix-like systems.

## 5. How to verify

Copy logic tương đương vào `starter/ArchiveImporter.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Verification yêu cầu đồng thời:

- valid theme file vẫn tồn tại và đúng nội dung;
- không có file được tạo ngoài root;
- process trả exit code 0.

## 6. Alternative fixes

- Reject package ngay khi phát hiện bất kỳ unsafe entry nào thay vì skip entry đó. Đây thường là policy tốt hơn cho security-sensitive import flow.
- Extract vào sandbox/temporary directory riêng rồi validate toàn bộ package trước khi promote sang destination cuối.
- Dùng API/library extraction đã có explicit protection phù hợp, nhưng vẫn phải hiểu contract và version cụ thể thay vì giả định library tự bảo vệ mọi archive format.

## 7. Wrong or misleading fixes

- Chỉ reject entry chứa chuỗi `..`: có thể bỏ sót rooted paths, separator variants hoặc normalization edge cases.
- Chỉ dùng `Path.Combine`: combine không chứng minh containment.
- Xóa file ngoài root sau khi extract: boundary đã bị vi phạm và attacker có thể ghi đè target nhạy cảm trước bước cleanup.
- Chỉ validate file extension: extension không giải quyết destination path.

## 8. Production implications

Archive import là trust boundary. Ngoài path containment, production flow nên cân nhắc giới hạn số entry, tổng uncompressed size, compression ratio, duplicate names, overwrite policy, permissions và logging/audit cho package bị reject.

## 9. Trade-offs

Skip unsafe entry giúp import phần hợp lệ nhưng có thể che giấu package độc hại hoặc tạo partial state. Reject toàn package an toàn và dễ reason hơn, nhưng UX có thể kém hơn. Với CMS theme package, reject toàn package thường là lựa chọn dễ bảo vệ hơn.

## 10. What a Senior engineer should notice

Security property quan trọng không phải "path trông có vẻ hợp lệ" mà là **canonical target cuối cùng có còn nằm trong trust boundary hay không**. Validation nên diễn ra trước side effect, và functional verification phải chứng minh fix không làm mất valid behavior.

Reference code: [ArchiveImporter.cs](ArchiveImporter.cs)
