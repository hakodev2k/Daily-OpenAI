# UNIT-GETORADD-001 — Biên nhận thu phí bị phát hành hai lần khi tải đồng thời

## Mục tiêu
Điều tra một sự cố concurrency trên .NET, phân biệt giá trị cache trả về với side effect của hệ thống phát hành biên nhận và thiết kế verification theo business invariant.

## Bối cảnh thực tế
Cổng thu phí dùng một khóa giao dịch để tránh cấp lại biên nhận. Trong đợt tăng tải, audit của hệ thống phát hành ghi nhận hai số serial cho cùng một lượt qua trạm, trong khi hai request API đều nhận cùng một mã biên nhận.

## Bạn cần làm gì
Reproduce, ghi lại evidence, đề xuất ít nhất hai hypothesis, sửa đường code trong `starter/` và xác minh số lần phát hành thực tế. Không thay đổi contract của hệ thống phát hành.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần dịch vụ ngoài.

## Chạy nhanh
Từ thư mục unit: `./scripts/setup.ps1`, `./scripts/run.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Script thành công khi phát hiện incident được kiểm soát, **không** có nghĩa code đã được sửa.

## Những gì cần quan sát
So sánh `issued` với `distinctReturned`; ghi lại số lượng side effect và kết quả API. Kiểm tra vì sao monitoring chỉ đếm response có thể bỏ sót lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/Program.cs`.
4. Chạy `./scripts/verify.ps1` để kiểm tra đường code vừa sửa.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md)

## Expected Results
Trước fix: reproduction hiển thị hai lần phát hành cho một giao dịch; sau fix: verify xác nhận đúng một lần phát hành và hai caller nhận cùng serial. Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
60–75 phút.
