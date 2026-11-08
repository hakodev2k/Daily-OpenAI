# UNIT-DECIMAL-001 — Food bank donation ledger: lệch 1 cent

## Mục tiêu
Điều tra sai lệch số tiền trong pipeline nhập khoản quyên góp; phân biệt representation, rounding policy và thời điểm aggregate.

## Bối cảnh thực tế
Một food bank nhận CSV từ nhiều điểm thu. Bảng đối soát hằng ngày đôi khi lệch 0.01 USD so với tổng được tính theo chính sách tài chính: mỗi giao dịch được làm tròn tới cent theo AwayFromZero trước khi cộng.

## Bạn cần làm gì
Chạy ứng dụng hiện tại, lưu output, xác định giao dịch tạo chênh lệch và đề xuất hai hypothesis trước khi sửa. Giữ nguyên input contract (chuỗi số tiền theo invariant culture); đảm bảo tổng độc lập thứ tự dòng.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database hay dịch vụ ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet run --project starter/DonationLedger.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` với starter chưa chỉnh sửa. Script kiểm tra output so với hợp đồng đối soát và phải phát hiện chênh lệch.

## Những gì cần quan sát
Ghi lại tổng xuất ra, tổng theo expected contract và mức sai khác. Thử thay thứ tự các giao dịch, xem tác động đến kết quả. Không sửa expected result để làm test xanh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
[Cảnh báo spoiler — solution](solution/README.md)

## Expected Results
Reproduce: thất bại có chủ đích với starter. Verify: chỉ pass khi learner code tính đúng các boundary cases và input hợp lệ.

## Estimated Time
35–45 phút.
