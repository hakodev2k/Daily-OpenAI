# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Hai request đọc cùng version của profile, sửa hai field độc lập, đều báo save thành công nhưng final document chỉ giữ thay đổi của request ghi sau.

## 2. Evidence
Cả hai snapshot bắt đầu ở cùng version. Write thứ nhất làm document tiến lên version mới. Write thứ hai vẫn dùng snapshot cũ nhưng không có precondition để phát hiện điều đó.

## 3. Root cause
Luồng read-modify-replace cho phép ghi một snapshot stale mà không kiểm tra version. Whole-document replacement của writer sau vì vậy có thể ghi đè thay đổi hợp lệ của writer trước.

## 4. Why the fix works
Dùng optimistic concurrency: write chỉ được chấp nhận khi current version vẫn bằng version mà request đã đọc. Trong MongoDB thực tế, filter có thể gồm `_id` và `version`, đồng thời update tăng version atomically. `MatchedCount == 0` biểu thị conflict.

Trong simulator, thay `Save` bằng logic tương đương:

```csharp
public bool Save(Profile candidate)
{
    if (candidate.Version != _current.Version)
        return false;

    _current = candidate with { Version = candidate.Version + 1 };
    return true;
}
```

Với MongoDB.Driver, ý tưởng tương ứng là filter theo identity + expected version và dùng một atomic update/replace có version increment.

## 5. How to verify
Chạy `./verify.ps1`. Sau fix, writer stale phải bị từ chối thay vì silently overwrite, hoặc nếu bạn chọn merge/retry policy thì final state phải bảo toàn cả hai thay đổi.

## 6. Alternative fixes
Nếu operations chỉ sửa field độc lập, dùng targeted atomic `$set` có thể tránh whole-document overwrite. Với business invariant phức tạp, vẫn cần concurrency precondition hoặc transaction phù hợp.

## 7. Wrong or misleading fixes
Thêm delay chỉ thay đổi interleaving. Retry vô điều kiện có thể tiếp tục ghi stale state. Global lock trong một process không bảo vệ nhiều application instances. Chuyển sang transaction mà không xác định conflict policy có thể tăng complexity mà chưa giải quyết semantics mong muốn.

## 8. Production implications
Lost update đặc biệt nguy hiểm vì request có thể trả 2xx và không tạo exception. Cần telemetry cho conflict rate và policy rõ ràng về retry, merge hoặc trả conflict cho caller.

## 9. Trade-offs
Optimistic concurrency phù hợp khi conflict tương đối hiếm và tránh lock dài. Khi conflict thường xuyên, retry có thể tăng tải; khi updates độc lập, field-level atomic operations thường đơn giản hơn.

## 10. What a Senior engineer should notice
Senior engineer phải xác định concurrency contract trước khi chọn primitive: whole-document replacement hay field-level update, conflict được reject hay merge, ai sở hữu retry policy, và invariant nào phải atomic.