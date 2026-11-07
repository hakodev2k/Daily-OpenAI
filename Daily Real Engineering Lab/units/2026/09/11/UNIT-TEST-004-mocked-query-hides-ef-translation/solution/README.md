# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Một test dùng `List<Product>.AsQueryable()` trả đúng kết quả, trong khi query tương đương qua EF Core SQLite không materialize được.

## 2. Evidence

Cùng `ProductSearch.Search` nhận hai `IQueryable<Product>` khác provider. LINQ-to-Objects thực thi delegate .NET bình thường; EF Core phải dịch expression tree sang SQL trước khi database thực thi.

## 3. Root cause

Starter gọi custom method `Normalize(p.Name)` bên trong predicate. LINQ-to-Objects có thể chạy method này cho từng object. Relational EF Core provider không có translation cho arbitrary application method đó, nên unit test bằng `AsQueryable()` không kiểm tra contract mà production thực sự phụ thuộc vào.

## 4. Why the fix works

Reference solution chuẩn hóa search term ở application side trước khi tạo query, còn expression chạy trên column chỉ dùng các operation provider có thể dịch (`Trim`, `ToLower`, `Contains` với SQLite/EF Core phiên bản được pin). Vì vậy query vẫn được thực thi ở database thay vì kéo toàn bộ rows về process.

Quan trọng hơn, regression test phù hợp phải chạy qua relational provider để kiểm tra translation boundary.

## 5. How to verify

Chạy `../verify.ps1`. Script chạy chính `starter/` sau khi learner sửa và yêu cầu cả memory path lẫn SQLite path trả `count=1`.

## 6. Alternative fixes

- Chuẩn hóa dữ liệu khi ghi và lưu thêm normalized/search column nếu business search semantics ổn định.
- Dùng provider-specific function/collation phù hợp nếu yêu cầu case/accent semantics cần chính xác hơn.
- Dùng integration test với database engine production thật cho các query có provider-specific behavior quan trọng.

## 7. Wrong or misleading fixes

- Gọi `AsEnumerable()` hoặc `ToList()` trước predicate chỉ để custom method chạy được: có thể chuyển filtering về application và gây tải dữ liệu lớn.
- Mock `DbSet` kỹ hơn nhưng vẫn dựa trên LINQ-to-Objects: vẫn không chứng minh translation.
- Chỉ assert expression shape: test implementation detail thay vì behavior trên provider.

## 8. Production implications

Query translation là một boundary runtime. Test strategy phải phản ánh boundary có rủi ro: business logic thuần có thể unit-test; query quan trọng nên có relational integration coverage.

## 9. Trade-offs

SQLite in-memory nhanh và self-contained nhưng không giống SQL Server/PostgreSQL ở mọi function, collation và type behavior. Với query provider-specific, thêm test trên production database engine là hợp lý dù CI tốn hơn.

## 10. What a Senior engineer should notice

`IQueryable<T>` không phải một collection abstraction trung tính. Nó đại diện cho một query program được provider diễn giải. Test double thay provider có thể thay luôn semantics cần kiểm chứng. Chọn test boundary theo failure mode, không chỉ theo tốc độ test.