# Reference Solution — spoiler

Hai toán hạng đều là `int`, nên phép nhân xảy ra trong 32-bit **trước** khi gán sang `long`. Trong ngữ cảnh unchecked, overflow làm kết quả wrap. Thay dòng tính bằng:

```csharp
long total = checked((long)batch.BagCount * batch.GramsPerBag);
```

Tích lớn nhất của hai `int.MaxValue` vẫn nằm trong `long`. Chạy `./scripts/verify.ps1` trên starter đã sửa. **Wrong fixes:** `(long)(a*b)` cast quá muộn; `Math.Abs` che triệu chứng; chia lô mà cộng vào `int` vẫn overflow. `decimal` hữu ích cho số đo thập phân, nhưng `long` phù hợp với gram nguyên và chính xác hơn `double` trong bài này. Nếu yêu cầu sau này vượt giới hạn `long`, cần kiểm soát giới hạn nghiệp vụ hoặc đổi kiểu số.
