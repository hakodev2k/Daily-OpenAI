# Reference Solution — Spoiler

**Chỉ đọc sau khi đã reproduce và thử sửa.** File tham khảo: [DepositRefund.reference.cs](DepositRefund.reference.cs). Không phải code được `verify.ps1` sử dụng: verification luôn biên dịch `starter/DepositRefund.cs`.

## 1. Symptoms

Một số bút toán hoàn cọc chênh 0,01; `1.005m` thành `1.00m`, trong khi `1.015m` thành `1.02m`. Giao dịch âm có thể sai đối xứng so với hợp đồng.

## 2. Evidence

Trong starter, `Math.Round(1.005m, 2)` trả `1.00m`, `Math.Round(1.015m, 2)` trả `1.02m`. Cả hai đều là midpoint nhưng phía cent trước đó có tính chẵn/lẻ khác nhau. `3 * 0.335m` tạo `1.005m`, nên lỗi xuất hiện cả khi tổng hợp nhiều thùng.

## 3. Root cause

Overload `Math.Round(decimal, int)` mặc định áp dụng `MidpointRounding.ToEven` (banker's rounding), còn hợp đồng của đối tác yêu cầu midpoint đi xa 0 (`AwayFromZero`). Đây là xung đột chính sách nghiệp vụ, không phải lỗi độ chính xác của `decimal`.

## 4. Why the fix works

Sau khi nhân số lượng có dấu với `unitDeposit`, dùng:

```csharp
return Math.Round(total, 2, MidpointRounding.AwayFromZero);
```

Policy được chỉ định tường minh, áp dụng một lần trên tổng và giữ nguyên hành vi của các giá trị không nằm ở midpoint. Giá trị âm được xử lý đối xứng theo độ lớn.

## 5. How to verify

Chỉ sửa `starter/DepositRefund.cs`, chạy `./verify.ps1`. Xác nhận các ca `1.005m`, `1.025m`, `-1.005m`, `3 * 0.335m`, giá trị không-midpoint, zero và invalid unitDeposit.

## 6. Alternative fixes

- Tách một `MoneyRoundingPolicy` có kiểm thử độc lập nếu nhiều dòng sản phẩm hoặc nhiều đối tác có chính sách khác nhau; tăng tính tường minh nhưng thêm abstraction.
- Dùng integer minor units chỉ khi mọi phép tính trước đó và quy tắc chuyển đổi được xác định rõ; dữ liệu có nửa cent vẫn cần một rounding policy ở biên chuyển đổi.

## 7. Wrong / tempting fixes

- **Chuyển sang `double`**: tăng rủi ro sai số biểu diễn nhị phân; không giải quyết policy.
- **Cộng 0.005 trước khi làm tròn**: sai với số âm, giá trị không-midpoint và dễ lệch tại ranh giới.
- **Làm tròn `unitDeposit` trước phép nhân**: `0.335 → 0.34`, nhân 3 ra `1.02`, khác hợp đồng `1.01`.
- **Bỏ qua 0.01 vì nhỏ**: có thể tích lũy sai lệch trong đối soát và làm mất dấu hiệu lỗi hợp đồng.

## 8. Production implications

Policy phải được version hóa theo hợp đồng đối tác; khi thay đổi, xác định ngày hiệu lực, xử lý bút toán đã ghi sổ và chạy đối soát trên tập giao dịch thực tế đã ẩn danh. Đừng âm thầm tính lại lịch sử bằng policy mới.

## 9. Trade-offs

`AwayFromZero` phù hợp hợp đồng này nhưng không phải mặc định đúng cho mọi hệ thống tài chính. `ToEven` có thể giảm thiên lệch thống kê trong các bài toán khác. Chọn theo hợp đồng, không theo cảm giác.

## 10. What a Senior engineer should notice

Phân biệt **độ chính xác kiểu dữ liệu**, **vị trí làm tròn**, và **chính sách midpoint**; yêu cầu contract tests với cả số âm và aggregation; coi thay đổi rounding là thay đổi business behavior cần rollout và reconciliation.

## References

- [Microsoft Learn — Math.Round](https://learn.microsoft.com/en-us/dotnet/api/system.math.round)
- [Microsoft Learn — MidpointRounding](https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding)
