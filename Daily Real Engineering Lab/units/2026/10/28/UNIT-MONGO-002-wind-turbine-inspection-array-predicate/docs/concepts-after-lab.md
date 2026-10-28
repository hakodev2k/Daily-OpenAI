# Sau lab: MongoDB array query semantics

Với một document chứa array of embedded documents, các dotted predicates độc lập có thể khớp các phần tử khác nhau. `$elemMatch` ràng buộc tất cả điều kiện bên trong vào **ít nhất một phần tử duy nhất**.

Ví dụ:
```javascript
// Hai điều kiện độc lập; có thể khớp ở hai inspection khác nhau.
{ "inspections.type": "brake", "inspections.status": "FAILED" }

// Hai điều kiện trên cùng một inspection.
{ inspections: { $elemMatch: { type: "brake", status: "FAILED" } } }
```

Hãy phân biệt:
- correctness của kết quả query
- projection (chỉ quyết định field trả về)
- indexing (tối ưu truy cập, không sửa predicate sai)
- idempotency của worker tạo ticket (không giải quyết false-positive từ query sai)

Thử thay đổi thứ tự các phần tử `inspections` trong fixture và kiểm tra rằng logic đúng không phụ thuộc vào thứ tự.
