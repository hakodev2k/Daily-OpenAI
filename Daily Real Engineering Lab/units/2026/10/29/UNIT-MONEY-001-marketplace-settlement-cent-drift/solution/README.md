# Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix

## 1. Symptoms

Seller statement lệch một hoặc vài cent so với tổng các phí đã hạch toán theo transaction, đặc biệt ở sales USD 0.05 và refunds.

## 2. Evidence

Ba transaction USD 0.05, fee 10%: mỗi chứng từ phải có phí USD 0.01. Tổng phí ledger là USD 0.03; starter xuất USD 0.02. Khi giao dịch USD 100.00, cả hai cách tình cờ khớp, nên chỉ test happy path sẽ bỏ sót lỗi.

## 3. Root cause

Starter cộng Gross theo seller trước, rồi mới tính và làm tròn Fee trên tổng. Nhưng contract yêu cầu ghi nhận phí riêng cho từng transaction, làm tròn tại transaction boundary. Nói chung `Round(Sum(x)) != Sum(Round(x))`. Đây là sai lệch **thứ tự phép toán theo contract**, không phải lỗi floating-point: code đã dùng `decimal`.

## 4. Why the fix works

Tính `decimal.Round(e.Amount * feeRate, 2, MidpointRounding.AwayFromZero)` cho từng entry rồi `Sum` trong seller group. Giữ `Gross` là tổng amount và `Net = Gross - Fee`. Tham khảo [SettlementCalculator.cs](SettlementCalculator.cs). Cách này bảo toàn khả năng đối soát statement với các chứng từ và đối xứng dấu cho refund.

## 5. How to verify

Tự sửa file trong `starter/`, sau đó chạy `./verify.ps1`. Verification dùng project `checks/` tham chiếu trực tiếp starter, không build reference solution. Các fixture bao gồm micro-sales, micro-refunds, nhiều seller, sale/refund hỗn hợp, normal sale và invalid rate.

## 6. Alternative fixes

Có thể lưu `PostedFee` như một giá trị immutable trong transaction ledger ngay tại thời điểm posting, rồi statement chỉ cộng `PostedFee`. Đây thường là thiết kế mạnh hơn cho audit khi tỷ lệ phí hoặc chính sách rounding thay đổi; cần schema migration và xác định thời điểm snapshot. Nếu không lưu, phải đảm bảo replay dùng đúng phiên bản contract/rate lịch sử.

## 7. Wrong / tempting fixes

- Đổi `decimal` sang `double`: thêm binary floating-point error, không sửa thứ tự nghiệp vụ.
- Đổi sang `MidpointRounding.ToEven`: làm thay đổi chính sách, không bảo đảm reconciliation.
- Cộng một cent vào cuối statement: che sai lệch và gây bất nhất audit, đặc biệt nhiều seller/refund.
- Làm tròn gross trước: gross không phải nguồn gây sai lệch và có thể làm mất thông tin.

## 8. Production implications

Lưu currency, fee rate, rounding policy version và fee đã posted khi cần audit; xử lý refund theo tham chiếu transaction gốc; bảo đảm reconciliation trước khi payout. Dùng fixed-point decimal hoặc integer minor units theo currency và contract, không dùng quy tắc USD cho JPY/KWD.

## 9. Trade-offs

Per-transaction rounding tạo kết quả có thể khác fee tính trên toàn seller batch; đó là lựa chọn nghiệp vụ, không phải phép toán nào luôn đúng. Nếu hợp đồng quy định batch-level fee, starter có thể hợp lệ — nhưng khi đó ledger, statement và test contract phải cùng một chính sách. Lưu phí từng transaction tốn storage nhưng giúp traceability và replay.

## 10. Senior engineering observation

Không kết luận lỗi do precision chỉ vì lệch cent. Đối chiếu nguồn sự thật, granularity của posting, policy version, refund symmetry và tính bất biến của sổ cái trước khi sửa. Cần contract tests độc lập với implementation, kiểm tra tổng nhiều seller và negative values.

## Follow-up challenge

Thêm một fee-policy version thay đổi theo ngày nhưng không làm thay đổi kết quả của transaction đã posted. Viết regression chứng minh historical replay ổn định.
