# After — learner fix

Chạy `./verify.ps1` trên chính `starter/Settlement.Core/SettlementCalculator.cs` đã sửa:

- Ba sales USD 0.05: fee USD 0.03; net USD 0.12.
- Ba refunds USD -0.05: fee USD -0.03; net USD -0.12.
- Hai seller được đối soát độc lập: A fee USD 0.03, B fee USD 0.02.
- Sale + refund có tổng Gross USD 0.05, Fee USD 0.01.
- Sale USD 100.00 vẫn fee USD 10.00.
- Rate ngoài khoảng hợp lệ vẫn bị từ chối.
- In `VERIFIED`, exit code 0. Chạy `./reproduce.ps1` sau fix sẽ không còn tái hiện triệu chứng (exit code 1), đây là kết quả bình thường.
