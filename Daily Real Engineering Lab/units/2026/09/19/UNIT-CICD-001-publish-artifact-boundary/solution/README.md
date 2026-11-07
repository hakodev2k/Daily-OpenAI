# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
CI build thành công nhưng deployment package có thể chứa file không thuộc release hiện tại hoặc khác output đã được kiểm chứng.

## 2. Evidence
Manifest của `publish` chỉ chứa output của release hiện tại, trong khi package được tạo từ workspace rộng hơn và nhận thêm residue.

## 3. Root cause
Pipeline không có artifact boundary rõ ràng: bước deploy tái dựng package từ mutable build workspace thay vì promote chính publish output đã tạo cho release.

## 4. Why the fix works
Chỉ package/copy từ `publish` làm cho tập file deployment phụ thuộc vào một output xác định. Workspace residue không còn tham gia vào release.

Trong starter, thay source của vòng copy từ `workspace` sang `publish`:

```csharp
foreach (var file in Directory.GetFiles(publish))
    File.Copy(file, Path.Combine(package, Path.GetFileName(file)), true);
```

## 5. How to verify
Chạy `./verify.ps1`. Manifest package phải bằng manifest publish và output có `VERIFY_PASS`.

## 6. Alternative fixes
CI thực tế nên tạo một immutable archive từ publish output, lưu artifact với build/release identity và để mọi environment download đúng artifact đó. Có thể ký hoặc lưu checksum manifest để kiểm chứng provenance.

## 7. Wrong or misleading fixes
Xóa riêng `old-plugin.dll` chỉ xử lý một residue đã biết. Clean workspace giúp giảm rủi ro nhưng vẫn giữ thiết kế deploy phụ thuộc workspace. Build lại ở mỗi environment phá nguyên tắc promote cùng một artifact và làm rollback khó tái lập.

## 8. Production implications
Artifact drift khiến staging và production có thể chạy binary khác nhau dù cùng commit, làm incident analysis và rollback thiếu tin cậy.

## 9. Trade-offs
Immutable artifacts cần storage, retention và naming/version policy. Đổi lại release có provenance rõ, deploy nhanh hơn và rollback deterministic hơn.

## 10. What a Senior engineer should notice
Build success không phải deployment reproducibility. Cần xác định artifact boundary, provenance, retention, checksum và nguyên tắc build once — promote many để cùng một output đi qua các environment.
