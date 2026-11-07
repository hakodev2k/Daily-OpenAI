# Reference Solution — chỉ xem sau khi tự reproduce và sửa

## 1. Symptoms
Invoice usage-based billing lệch 0,01–0,02 so với partner dù event count ổn định.

## 2. Evidence
INV-201 có 4 raw charge 0.335, local=1.36 nhưng partner=1.34. INV-205 với giá trị cent chính xác không lệch.

## 3. Root cause
Starter làm tròn mỗi raw event trước khi cộng; hợp đồng yêu cầu cộng raw charges rồi làm tròn tại invoice boundary. `sum(round(x))` có thể khác `round(sum(x))`.

## 4. Why fix works
`solution/InvoiceCalculator.cs` dùng `rawCharges.Sum()`, sau đó `decimal.Round(total,2,MidpointRounding.AwayFromZero)`. Không sửa audit data.

## 5. Verify
```powershell
./verify.ps1
dotnet run --project solution/BillingReconciliation.Reference.csproj -- verify
```
Lệnh đầu kiểm tra starter người học chỉnh; reference chỉ để đối chiếu. `reproduce.ps1` không còn exit 0 sau fix.

## 6. Alternatives
Nếu hợp đồng định nghĩa line-item rounding, policy khác có thể hợp lệ. Currency scale khác 2 cần versioned contract; có thể dùng minor units sau settlement nhưng không cắt precision raw events.

## 7. Wrong / tempting fixes
- `double`: sai số nhị phân.
- Tăng số chữ số UI: không sửa settlement.
- Sửa raw events: phá audit.
- Retry hay thêm lock: không có evidence race.
- Đổi rounding mode tùy ý: vi phạm contract.

## 8. Production implications
Contract tests, policy version, audit raw sum, currency, settlement amount và rounding mode. Correction của invoice đã phát hành phải có audit trail.

## 9. Trade-offs
Rounded line items hiển thị có thể không cộng đúng settlement; nếu bắt buộc cần quy tắc phân bổ residual cents. Tax/currency có thể có precision boundary khác.

## 10. Senior engineer should notice
Precision boundary là business invariant. Kiểm tra grouping invariance, reconciliation, rollback và remediation trước hotfix.

## Official references
- https://learn.microsoft.com/en-us/dotnet/api/system.decimal.round
- https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding
