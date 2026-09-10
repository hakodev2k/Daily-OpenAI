# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Request B nhận một buffer mới theo góc nhìn business flow nhưng snapshot ban đầu có thể chứa marker `ALPHA-42` của request A.

## Evidence

- Không có application cache truyền payload giữa hai request.
- Cùng một pool được dùng cho cả hai request.
- Request A trả buffer về pool ngay sau khi xử lý.
- Request B rent buffer trước khi ghi payload mới và thấy dữ liệu cũ.

## Root cause

`ArrayPool<T>` tối ưu allocation bằng cách tái sử dụng array. `Return(array)` không mặc định xóa nội dung. Vì vậy bytes nhạy cảm của request trước có thể còn nguyên khi cùng array được cấp lại cho request sau.

## Why the fix works

Với dữ liệu nhạy cảm, buffer phải được làm sạch trước khi trở lại vùng tái sử dụng. Reference solution dùng:

```csharp
pool.Return(buffer, clearArray: true);
```

Điều này bảo đảm array được clear trước khi pool giữ lại nó.

## How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả phải có `LAB_VERIFY_PASS`, đồng thời payload B vẫn bằng `tenant-b|payload=BETA`.

## Alternative fixes

- `Array.Clear` vùng chứa dữ liệu nhạy cảm trước khi `Return`.
- Không dùng pool cho dữ liệu nhạy cảm nếu allocation pressure không đáng kể và simplicity quan trọng hơn.
- Thiết kế abstraction riêng chịu trách nhiệm ownership + clearing để caller không thể quên policy.

## Wrong or misleading fixes

- Clear buffer ngay sau `Rent`: giảm khả năng leak cho caller hiện tại nhưng vẫn để dữ liệu nhạy cảm nằm trong pool trong khoảng thời gian giữa hai lần rent.
- Gọi `GC.Collect()`: không liên quan đến lifecycle của array đang được pool giữ.
- Tăng kích thước pool: chỉ làm lỗi khó reproduce hơn, không sửa isolation contract.
- Chỉ overwrite phần payload B: phần còn lại của array vẫn có thể chứa bytes cũ nếu downstream đọc quá vùng logical length.

## Production implications

Pooling là optimization có ownership contract. Với secrets, PII, authentication material hoặc tenant-isolated payload, lifecycle của memory phải được xem như một security/correctness boundary chứ không chỉ performance detail.

## Trade-offs

`clearArray: true` có CPU cost tỷ lệ với kích thước array. Không phải mọi buffer đều cần clear. Policy nên dựa trên sensitivity của dữ liệu, kích thước buffer, allocation rate và threat model.

## What a Senior engineer should notice

- Pooling thay đổi lifetime của memory so với lifetime logic của request.
- Logical ownership kết thúc không có nghĩa physical contents biến mất.
- Performance optimization có thể tạo data-isolation risk.
- Verification nên kiểm tra cả confidentiality property lẫn functional correctness.
