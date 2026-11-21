# UNIT-TAXROUND-001 — Sai lệch thuế khi đối soát từng dòng hóa đơn

## Mục tiêu
Điều tra sai lệch giữa hóa đơn POS và báo cáo cuối ngày; refactor một phép tính tiền mà không làm thay đổi hợp đồng nghiệp vụ.

## Bối cảnh thực tế
Một chợ cộng đồng dùng POS tính thuế trên từng dòng hàng. Báo cáo tổng hợp từ dịch vụ .NET đôi khi lệch 0,01 đơn vị tiền tệ, đặc biệt ở giỏ hàng giá thấp và phiếu hoàn trả. Đội vận hành yêu cầu chứng cứ có thể tái hiện trước khi sửa.

## Bạn cần làm gì
Chạy hệ thống, ghi lại số liệu và hypothesis, xác định ranh giới nghiệp vụ tính thuế, chỉnh sửa **starter/Program.cs**, sau đó chạy bộ kiểm chứng. Giải thích tại sao thay đổi không gây sai lệch ở refund và nhiều mức thuế.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần database, internet hay secret.

## Chạy nhanh
```powershell
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./scripts/reproduce.ps1`; script yêu cầu bộ kiểm tra phát hiện ít nhất một khác biệt. Ghi lại các case PASS/FAIL.

## Những gì cần quan sát
Đối chiếu giá trị từ từng case với số dự kiến; kiểm tra dấu của hoàn tiền, hành vi ở midpoint và kết quả tổng hợp.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trên starter.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước sửa: ít nhất một case FAIL, exit code khác 0. Sau sửa: toàn bộ năm case PASS, exit code 0; không thay đổi case hợp lệ.

## Estimated Time
45 phút.
