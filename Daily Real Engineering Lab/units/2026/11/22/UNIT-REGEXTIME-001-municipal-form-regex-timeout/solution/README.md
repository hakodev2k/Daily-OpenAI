# Reference Solution — Spoiler

## Root cause
Biểu thức `^(a+)+$` có nested quantifier. Với chuỗi nhiều `a` rồi `!`, backtracking engine phải thử rất nhiều cách phân chia các nhóm trước khi thất bại. Không có timeout làm thời gian CPU khó dự đoán.

## Bản sửa
Thay nội dung `ReferenceCodeValidator.IsValid` trong starter bằng:
```csharp
public static bool IsValid(string code)
{
    if (string.IsNullOrEmpty(code) || code.Length > 64) return false;
    foreach (char c in code)
        if (c != 'a') return false;
    return true;
}
```
Đây là một phép kiểm tra tuyến tính, giới hạn input. Nếu business contract cần regex phức tạp, ưu tiên `RegexOptions.NonBacktracking` khi tương thích và đặt `matchTimeout` rõ ràng, xử lý `RegexMatchTimeoutException` theo chính sách reject/telemetry.

## Kiểm chứng
Chạy `./verify.ps1` trên **starter đã sửa**; `./reproduce.ps1` trước và sau để so elapsedMs. Không đánh giá bằng test chỉ chạy reference.

## Wrong fixes
- Tăng số lượng worker: chỉ che triệu chứng, tăng tiêu thụ CPU.
- Chỉ giới hạn 10.000 ký tự: vẫn cho phép input ngắn kích hoạt thời gian tăng theo cấp số nhân.
- Catch mọi exception rồi trả 200: mất tín hiệu và sai hợp đồng API.

## Trade-offs
Linear validator nhanh, rõ nhưng chỉ phù hợp grammar đơn giản. Với grammar phức tạp, cần định nghĩa input contract, lựa chọn engine phù hợp, timeout, metrics và load test có giới hạn.
