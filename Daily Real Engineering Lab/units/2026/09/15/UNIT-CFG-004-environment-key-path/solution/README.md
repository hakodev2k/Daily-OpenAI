# Reference Solution — chỉ xem sau khi đã tự thử

## Symptoms
Deployment đặt `ExportOptions__BatchSize=25`, nhưng process vẫn báo `100`.

## Evidence
Biến môi trường tồn tại và process nhận được nó; sai lệch nằm ở cách application truy cập configuration hierarchy.

## Root cause
Environment Variables provider normalize `__` thành `:`. Vì vậy biến `ExportOptions__BatchSize` trở thành configuration key `ExportOptions:BatchSize`. Starter lại đọc `ExportOptions_BatchSize`, một key khác.

## Why the fix works
Đọc `app.Configuration["ExportOptions:BatchSize"]` sử dụng đúng hierarchical key mà provider tạo ra, nên environment-specific value override được fallback.

## How to verify
Sửa `starter/Program.cs`, sau đó chạy `./verify.ps1`. Kết quả phải chứa `Effective export batch size: 25`.

## Alternative fixes
Bind section bằng Options pattern (`GetSection("ExportOptions")`) là lựa chọn tốt hơn khi có nhiều setting liên quan và cần validation.

## Wrong / tempting fixes
- Hard-code `25`: che symptom và phá environment portability.
- Đổi deployment variable thành tên mà code sai đang đọc: có thể làm một môi trường chạy được nhưng phá contract cấu hình phân cấp và dễ drift giữa providers.
- Bỏ fallback `100`: chỉ biến lỗi silent thành null; không sửa contract key.

## Production implications
Configuration drift thường chỉ xuất hiện sau deploy vì local defaults vẫn hợp lệ. Startup validation và log effective non-secret settings giúp phát hiện sớm hơn.

## Trade-offs
Direct key access gọn cho vài setting; strongly typed Options tăng code nhưng cung cấp grouping, validation và refactoring safety.

## Senior engineer should notice
Cần phân biệt external provider representation với canonical configuration key, đồng thời kiểm tra effective configuration tại runtime thay vì chỉ nhìn deployment manifest.
