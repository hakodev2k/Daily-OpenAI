# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Starter integration test xanh khi lưu hai `UserAccount` có cùng `NormalizedEmail`, trong khi model khai báo unique index và production relational database từ chối trạng thái đó.

## 2. Evidence
Điểm quan trọng không phải assertion mà là test provider: starter dùng EF Core InMemory. Provider này phù hợp cho một số test logic nhưng không phải relational database engine và không thực thi relational unique constraint như SQLite/SQL Server.

## 3. Root cause
Test đang kiểm chứng một database invariant bằng một provider không cung cấp invariant đó. Vì vậy màu xanh của test không phải bằng chứng rằng persistence contract tương thích với production.

## 4. Why the fix works
Dùng SQLite in-memory giữ test local và nhanh nhưng đi qua relational provider. `EnsureCreatedAsync()` tạo unique index từ EF model; lần insert duplicate sau đó bị database từ chối.

## 5. How to verify
Copy hai file reference bên dưới vào `starter/`, rồi chạy `./verify.ps1`. Test suite phải xanh vì duplicate case **expect** `DbUpdateException`, đồng thời happy path vẫn persist được hai email khác nhau.

## 6. Alternative fixes
- Testcontainers + SQL Server/PostgreSQL: fidelity cao hơn khi production phụ thuộc behavior riêng của engine, đổi lại startup và operational cost cao hơn.
- Dedicated integration database: phù hợp CI phức tạp nhưng cần isolation/cleanup tốt.
- Unit test application-level duplicate check: hữu ích như lớp bảo vệ sớm, nhưng không thay thế database uniqueness vì concurrent requests vẫn cần invariant ở persistence layer.

## 7. Wrong / tempting fixes
- Đổi assertion từ `2` thành `1`: không làm provider enforce constraint.
- Catch mọi exception và cho test pass: che failure thay vì chứng minh contract.
- Chỉ mock repository trả duplicate error: test application reaction được, nhưng không chứng minh EF model/database schema thực thi uniqueness.
- Bỏ unique index để production giống test: phá invariant nghiệp vụ.

## 8. Production implications
False-green integration tests đặc biệt nguy hiểm với unique/FK/check constraints, transaction behavior, raw SQL và provider-specific query translation. Chọn test double theo behavior cần chứng minh, không theo tiêu chí “chạy nhanh nhất” một cách mặc định.

## 9. Trade-offs
SQLite in-memory là middle ground tốt cho lab này: local, nhanh, relational. Nó vẫn không tương đương SQL Server ở mọi type mapping, isolation, collation hoặc SQL dialect. Những contract phụ thuộc engine thật cần test trên engine thật.

## 10. Senior engineer should notice
Test fidelity là một kiến trúc test decision. Hỏi: “Failure này thuộc application semantics hay infrastructure semantics?” Sau đó chọn boundary và provider đủ thật để failure có thể xuất hiện.

Reference files: `ProviderFidelity.Tests.csproj` và `UserPersistenceTests.cs`.