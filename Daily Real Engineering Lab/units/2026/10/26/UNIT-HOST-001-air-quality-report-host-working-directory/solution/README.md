# Reference Solution — SPOILER

## 1. Symptoms
Manual run thành công; scheduler dùng cùng artifact nhưng ném `FileNotFoundException`. Health-check không đi qua report pipeline.

## 2. Evidence
Template có trong project và được MSBuild copy vào output. Diagnostic cho thấy process working directory khác nhau. Stack trace của scheduler trỏ đến một đường dẫn không nằm trong thư mục artifact. Disk-free và HTTP health-check là tín hiệu nhiễu.

## 3. Root cause
`Path.Combine("templates", "summary.txt")` tạo relative path. `File.ReadAllText` giải quyết đường dẫn dựa trên `Environment.CurrentDirectory`, giá trị do launcher quyết định. Manual run tình cờ có template ở working directory, khiến lỗi bị che giấu.

## 4. Why the fix works
Sử dụng [reference source](FileTemplateProvider.cs) để neo đường dẫn vào `AppContext.BaseDirectory` (application base). MSBuild copy template vào `templates/` bên dưới output; cùng binary hoạt động độc lập với working directory.

## 5. How to verify
```powershell
Copy-Item ./solution/FileTemplateProvider.cs ./starter/ReportWorker/FileTemplateProvider.cs -Force
./verify.ps1
```
`verify.ps1` kiểm tra **starter learner đã chỉnh sửa**, không chỉ reference solution. Gate `reproduce.ps1` được thiết kế cho starter gốc nên sau fix sẽ không còn báo REPRODUCED.

## 6. Alternative fixes
- `IHostEnvironment.ContentRootPath` phù hợp ứng dụng Generic Host nếu deployment contract gắn content với host root.
- Absolute path qua configuration phù hợp template do operations quản trị ngoài artifact.
- Cấu hình scheduler working directory là workaround có thể chấp nhận khi contract đã quy định rõ nhưng dễ drift.

## 7. Wrong / tempting fixes
- Copy template vào `C:\Windows\System32`: phụ thuộc máy, tạo operational debt.
- Nuốt `FileNotFoundException` rồi xuất báo cáo rỗng: silent data loss.
- `Directory.SetCurrentDirectory` toàn process: global mutable state ảnh hưởng thư viện khác.
- Retry hoặc scale out: không chữa lỗi deterministic path resolution.

## 8. Production implications
Phân biệt packaged resources và external configuration. CI nên kiểm tra output artifact, deployment giữ cấu trúc file, telemetry log resolved path mà không lộ bí mật.

## 9. Trade-offs
`AppContext.BaseDirectory` tốt cho packaged content nhưng không nên áp đặt cho cấu hình external. `ContentRootPath` phụ thuộc host configuration; absolute configured path linh hoạt nhưng tăng yêu cầu quản trị và permission.

## 10. What a Senior engineer should notice
Đây là lỗi contract giữa build artifact và process launcher, không phải vấn đề hiệu năng hay I/O retry. Verification cần biến đổi execution context để chứng minh invariant, thay vì chỉ test happy path từ IDE.
