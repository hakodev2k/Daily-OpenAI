# Reference Solution — Spoiler

Root cause là entity được materialize ở trạng thái detached nên phép gán `Price` chỉ thay đổi object CLR; DbContext gọi `SaveChangesAsync` không có tracked change để persist.

Một fix đơn giản trong scenario này là để update query được tracking:

```csharp
var product = await updateDb.Products
    .SingleAsync(x => x.Id == 1);

product.Price = 129m;
await updateDb.SaveChangesAsync();
```

Sau materialization, entity bắt đầu ở state `Unchanged`. Khi `Price` thay đổi, EF Core change detection nhận ra thay đổi và `SaveChangesAsync` persist dữ liệu.

Một phương án khác khi entity thực sự đến từ boundary detached:

```csharp
updateDb.Attach(product);
product.Price = 129m;
updateDb.Entry(product).Property(x => x.Price).IsModified = true;
await updateDb.SaveChangesAsync();
```

## Trade-offs

- Tracking query phù hợp khi use case đọc rồi sửa trong cùng unit of work.
- Read-only flow có thể dùng no-tracking để giảm overhead.
- `Update(entity)` có thể đánh dấu rộng hơn mức cần thiết; property-level modification an toàn hơn cho patch semantics.
- Không nên bật tracking cho mọi query chỉ để tránh bug này; query semantics phải theo use case.
