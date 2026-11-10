# Reference solution — Spoiler

## Evidence
Ban đầu thứ tự `5,4,3,2,1`. Sau trang đầu `5,4`, thêm `6` ở đầu: OFFSET 2 trả `4,3`, nên `4` trùng; tập kết quả không còn là snapshot.

## Root cause
OFFSET mô tả vị trí tương đối, không mô tả danh tính bản ghi. Concurrent insert làm dịch vị trí các bản ghi còn lại.

## Fix cho fixture
Trong `Pages`, lấy snapshot có thứ tự **trước** callback, rồi chia trang từ snapshot thay vì query lại danh sách đang thay đổi:

```csharp
var snapshot = permits.OrderByDescending(p => p.CreatedAt)
                      .ThenByDescending(p => p.Id)
                      .Select(p => p.Id).ToArray();
betweenPages(permits);
return snapshot;
```

Đây là mô hình minh họa snapshot; với dữ liệu lớn, không nên tải toàn bộ ID vào RAM.

## Production decision
Sử dụng keyset cursor có `(CreatedAt, Id)` để xác định vị trí cuối, `ORDER BY CreatedAt DESC, Id DESC`, và điều kiện trang kế tiếp `CreatedAt < @lastTime OR (CreatedAt = @lastTime AND Id < @lastId)`. Truyền upper watermark của phiên duyệt để loại bản ghi mới hơn mốc khởi đầu. Cần định nghĩa rõ liệu cập nhật/xóa bản ghi cũ có được phản ánh; watermark đơn thuần không tạo database snapshot lịch sử. Muốn snapshot chặt chẽ qua nhiều request cần snapshot/materialized result hoặc cơ chế versioning, với chi phí lưu trữ và lifecycle.

## Wrong fixes
- Tăng OFFSET khi thấy trùng: có thể bỏ sót bản ghi khi delete đồng thời.
- Chỉ `ORDER BY CreatedAt`: timestamp bằng nhau gây thứ tự không xác định.
- Thêm `DISTINCT` ở client: che trùng nhưng không khôi phục bản ghi bị bỏ sót.

## Verify
`./verify.ps1` phải PASS sau khi sửa starter. Chạy nhiều lần với fixture cố định. Thảo luận thêm index `(CreatedAt DESC, Id DESC)`, tính tương thích cursor và điều kiện expiration.
