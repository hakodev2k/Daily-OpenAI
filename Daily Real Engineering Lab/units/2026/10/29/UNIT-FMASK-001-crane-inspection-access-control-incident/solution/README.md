# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi hypothesis và thử sửa.

## 1. Symptoms

User chỉ có ViewInspection vẫn xuất được certificate khi action mới chưa được khai báo trong policy map. Action đã cấu hình vẫn DENY đúng khi thiếu quyền.

## 2. Evidence

Fixture incomplete thiếu key ExportCertificate; decision trong reproduce là ALLOW. Fixture configured với ApproveInspection yêu cầu cả ViewInspection và ApproveInspection lại DENY với view-only. Vì vậy user claims không tự tăng quyền; sai lệch nằm ở nhánh xử lý action không tìm thấy.

## 3. Root cause

Starter dùng Permission.None (giá trị 0) làm fallback cho action chưa có policy rồi gọi Enum.HasFlag(required). HasFlag(0) trả true vì mọi bit trong mask rỗng đều đã được thỏa mãn. Kết hợp hai lựa chọn này tạo fail-open. Tương tự, policy được cấu hình rõ ràng với mask 0 cũng được ALLOW.

## 4. Why the fix works

Reference code tách riêng ba trạng thái: action không tồn tại => DENY; mask rỗng hoặc chứa bit ngoài tập được định nghĩa => DENY; policy hợp lệ => chỉ ALLOW nếu granted chứa đầy đủ các bit required. Không dùng mask rỗng để biểu thị public endpoint; public action cần contract riêng, được audit và đăng ký tường minh.

## 5. How to verify

Chỉ thay starter/PermissionGate.cs bằng logic tương đương reference (không sửa checks). Chạy ./verify.ps1: phải đạt 9/9 PASS, exit code 0. Sau đó chạy ./reproduce.ps1: symptom không còn, script có thể trả 1 vì được thiết kế để chứng minh lỗi ban đầu.

## 6. Alternative fixes

- Dùng policy object có trạng thái Missing/Invalid/Configured thay cho enum fallback; rõ ràng hơn khi hệ thống có nhiều action.
- Kiểm tra policy tại startup và fail deployment khi thiếu action bắt buộc; giảm rủi ro nhưng vẫn cần runtime fail-closed khi config động.
- Dùng authorization policies chuẩn ASP.NET Core khi ứng dụng là web API, nhưng vẫn phải định nghĩa missing-policy behavior.

## 7. Wrong or misleading fixes

- Chỉ ẩn nút ExportCertificate trên UI: backend vẫn có thể bị gọi trực tiếp.
- Thêm một permission cho mọi user: che triệu chứng, làm tăng quyền ngoài ý muốn.
- Chỉ sửa một action trong config mà không đổi fail-open fallback: lỗi sẽ tái diễn ở action mới khác.
- Chỉ kiểm tra required != None mà không xử lý action chưa đăng ký ở tầng policy: có thể giải quyết fixture hiện tại, nhưng khó audit cấu hình thiếu.

## 8. Production implications

Ghi audit cho deny decision với action name và policy version, không log token nhạy cảm. Kiểm thử matrix cho mỗi action, đặc biệt khi deploy config độc lập với binary. Kiểm tra chính sách từ server, không tin UI. Khi rollout action mới, áp dụng deny-by-default trước khi cho phép gọi.

## 9. Trade-offs

Startup validation chặt giúp phát hiện cấu hình thiếu sớm nhưng có thể chặn deploy nếu action optional. Runtime deny-by-default bảo vệ request nhưng có thể gây 403 khi config chưa đồng bộ. Chọn rõ cơ chế rollback và báo lỗi cấu hình; không biến lỗi cấu hình thành ALLOW.

## 10. What a Senior engineer should notice

Authorization là một contract độc lập với trải nghiệm UI. Giá trị sentinel (0/None) không thể mặc định mang nghĩa policy công khai. Đảm bảo quyền cấp, action registry và policy version nhất quán, thêm regression test cho unknown action và invalid mask. Thử nghiệm chỉ dựa trên happy path của action đã cấu hình sẽ bỏ sót lỗi này.

Reference code: [PermissionGate.cs](PermissionGate.cs).