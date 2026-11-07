# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Navigation vẫn đúng về chức năng, nhưng số database command tăng theo số section. Với 8 section, starter tạo 9 reader command cho workload.

## 2. Evidence

Command counter cho thấy một query lấy danh sách section và thêm một query cho từng section để lấy featured page. Khi số section tăng, command count tăng theo cùng tỷ lệ.

## 3. Root cause

Workload materialize danh sách section trước, sau đó thực hiện thêm một EF Core query bên trong vòng lặp. Đây là một dạng N+1/query amplification: một query cha kéo theo N query con.

## 4. Why the fix works

Đưa việc chọn featured page vào cùng một set-based projection để EF Core dịch toàn bộ shape thành SQL. Database có thể xử lý tập dữ liệu trong một command thay vì application phát command lặp lại theo từng section.

Ví dụ một hướng sửa:

```csharp
var navigation = db.Sections
    .AsNoTracking()
    .Where(x => x.IsActive)
    .OrderBy(x => x.SortOrder)
    .Select(section => new NavigationItem(
        section.Name,
        db.Pages
            .Where(page => page.SectionId == section.Id && page.IsPublished)
            .OrderBy(page => page.SortOrder)
            .Select(page => page.Slug)
            .FirstOrDefault()))
    .ToList();
```

## 5. How to verify

Chạy:

```powershell
./scripts/verify.ps1
```

Kết quả phải giữ `Items=8`, `MissingFeatured=0` và `Commands<=2`.

## 6. Alternative fixes

- Query toàn bộ section và featured page cần thiết trong hai set-based queries rồi join/group ở memory. Cách này vẫn bounded roundtrip và đôi khi dễ kiểm soát SQL hơn.
- Nếu navigation là dữ liệu đọc nhiều và thay đổi ít, có thể cache kết quả sau khi đã sửa query shape; cache không nên là cách che N+1.
- Với model có navigation relationship phù hợp, projection qua navigation property cũng có thể cho SQL tốt hơn.

## 7. Wrong or misleading fixes

- Tăng connection pool chỉ làm tăng tài nguyên cho nhiều roundtrip, không loại bỏ amplification.
- `Task.WhenAll` nhiều query song song có thể giảm wall-clock time trong vài trường hợp nhưng tăng áp lực connection/database và giữ nguyên vấn đề N query.
- Thêm cache ngay lập tức có thể làm benchmark đẹp hơn nhưng root cause vẫn còn khi cache miss hoặc invalidation xảy ra.
- Đổi sang `AsNoTracking()` không giải quyết roundtrip count; starter đã dùng no-tracking.

## 8. Production implications

N+1 thường khó thấy ở dev vì dataset nhỏ. Ở production, latency, database CPU, connection usage và network chatter có thể tăng cùng cardinality. Một endpoint đơn lẻ vẫn trả đúng nên monitoring chỉ theo error rate dễ bỏ sót vấn đề.

## 9. Trade-offs

Một query lớn không luôn luôn tốt hơn mọi trường hợp. Projection cần chọn đúng fields để tránh cartesian explosion hoặc payload quá lớn. Với nhiều collection relationship, split query có thể hợp lý hơn. Mục tiêu là bounded, evidence-based database interaction chứ không phải ép mọi thứ thành đúng một SQL command.

## 10. What a Senior engineer should notice

Senior engineer nên kiểm tra query count cùng latency, hiểu cardinality ảnh hưởng workload thế nào, xem SQL được sinh ra thay vì chỉ nhìn LINQ, và chọn query shape dựa trên dữ liệu thực tế. Khi tối ưu, phải giữ functional contract và chứng minh improvement bằng measurement thay vì intuition.
