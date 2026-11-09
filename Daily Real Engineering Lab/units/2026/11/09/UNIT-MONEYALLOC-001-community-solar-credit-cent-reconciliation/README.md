# UNIT-MONEYALLOC-001 — Đối soát phân bổ tín dụng điện mặt trời đến từng cent

## Mục tiêu
Điều tra sai lệch số tiền trong batch settlement; xây dựng invariant, kiểm thử hoán vị đầu vào và chứng minh kết quả đối soát.

## Bối cảnh thực tế
Hợp tác xã điện mặt trời chia khoản credit cho hộ gia đình theo trọng số sử dụng. Batch đầu tháng được ngân hàng chấp nhận, nhưng tổng các dòng chi trả đôi lúc không bằng tổng settlement. Số tiền hiển thị luôn có hai chữ số thập phân.

## Bạn cần làm gì
Chạy fixture, ghi nhận số liệu đầu vào/đầu ra, đề xuất ít nhất hai giả thuyết, sửa `starter/Program.cs`, giữ nguyên các điều kiện hợp đồng và chạy verify.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell; không cần database, internet hay tài khoản cloud.

## Chạy nhanh
`dotnet run --project starter/CreditLab.csproj`

## Cách reproduce vấn đề
Tại thư mục unit chạy `./reproduce.ps1`. Lệnh dự kiến trả exit code khác 0. Ghi lại settlement total và tổng dòng credit.

## Những gì cần quan sát
Đối chiếu 0.10, 0.02 và 12.37 với số tiền thực phân bổ. Đổi thứ tự hộ và so sánh kết quả theo ID. Kiểm tra mỗi dòng là bội số của 0.01 và không âm.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
Trước sửa: reproduction phát hiện ít nhất một invariant thất bại. Sau sửa: toàn bộ fixture PASS, tổng phân bổ khớp tuyệt đối và kết quả ổn định theo household ID.

## Estimated Time
45 phút.
