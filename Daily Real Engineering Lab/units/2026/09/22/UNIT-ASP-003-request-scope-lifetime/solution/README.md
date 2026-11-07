# Reference Solution — Spoiler

## 1. Symptoms
Request đầu tiên và request sau có input tenant khác nhau nhưng consumer có thể quan sát cùng một request context hoặc state không thuộc scope hiện tại.

## 2. Evidence
Starter đăng ký `RequestContext` là scoped nhưng `OrderAuditService` là singleton. Consumer được tạo một lần và giữ dependency qua nhiều request scope.

## 3. Root cause
Đây là captive dependency: singleton giữ một scoped dependency lâu hơn lifetime contract của dependency đó. Khi scope validation bị tắt, container cho phép object graph này và request-specific state không còn được sở hữu bởi request scope như thiết kế.

## 4. Why the fix works
Đăng ký `OrderAuditService` là scoped làm consumer và `RequestContext` cùng sống trong request scope. Mỗi request nhận object graph riêng. Bật `ValidateScopes` còn biến lifetime mismatch thành lỗi cấu hình sớm thay vì silent state leak.

## 5. How to verify
Sửa `starter/Program.cs`, chạy `./verify.ps1`. Cả `tenant-a` và `tenant-b` phải được consumer quan sát đúng và script phải exit 0.

## 6. Alternative fixes
Nếu consumer thực sự phải là singleton, không giữ scoped instance trong field. Thiết kế lại boundary để truyền immutable request data vào method, hoặc tạo scope rõ ràng tại operation boundary khi đó thực sự là ownership đúng. Trong ASP.NET Core request path, giảm lifetime của consumer thường đơn giản và an toàn hơn.

## 7. Wrong / misleading fixes
- Gán lại field tenant trước mỗi call: che symptom nhưng không sửa lifetime ownership và dễ race khi concurrency tăng.
- Dùng static/AsyncLocal để giữ tenant mà không có contract rõ ràng: chuyển lỗi sang global/ambient state.
- Tắt scope validation trong production: loại bỏ guardrail, không giải quyết object graph sai.
- Scale out: chỉ phân tán lỗi sang nhiều process.

## 8. Production implications
Lifetime mismatch có thể gây data isolation bug, logging sai tenant, behavior phụ thuộc request order và lỗi khó reproduce khi concurrency cao. Scope validation nên được dùng như guardrail phù hợp trong development/test.

## 9. Trade-offs
Scoped consumer tạo instance mỗi request nhưng thường là chi phí nhỏ so với correctness. Nếu object khởi tạo rất đắt, hãy tách phần stateless/expensive có thể singleton khỏi phần request state thay vì kéo toàn bộ consumer lên singleton.

## 10. What a Senior engineer should notice
Vấn đề không chỉ là chọn `AddScoped` hay `AddSingleton`; trọng tâm là ownership. Dependency graph không được để object sống lâu giữ object sống ngắn chứa mutable request state. Senior engineer cũng kiểm tra concurrency, scope validation và boundary truyền dữ liệu để ngăn cùng lớp lỗi quay lại.