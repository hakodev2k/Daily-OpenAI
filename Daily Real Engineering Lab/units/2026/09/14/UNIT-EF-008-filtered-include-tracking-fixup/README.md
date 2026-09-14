# UNIT-EF-008 — Filtered Include Tracking Surprise

## Mục tiêu

Điều tra một truy vấn EF Core trả về collection navigation có phần tử nằm ngoài điều kiện filter, dù SQL/query nhìn qua có vẻ hợp lý.

## Bối cảnh thực tế

Một endpoint support cần tải khách hàng cùng các đơn hàng đang mở. Trong cùng request, service đã chạy một truy vấn khác để lấy lịch sử đơn hàng phục vụ audit. Ở môi trường test đơn giản endpoint thường đúng, nhưng với một số request thực tế collection `Orders` lại chứa cả đơn đã đóng.

## Bạn cần làm gì

- Reproduce hiện tượng từ starter.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Kiểm tra query, trạng thái tracking và collection navigation.
- Sửa starter để contract của endpoint luôn chỉ chứa đơn đang mở.
- Chạy `verify.ps1` để xác nhận fix.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-EF-008-filtered-include-tracking-fixup"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Query đầu tiên tải dữ liệu lịch sử thành công.
- Query sau có filter cho collection navigation.
- Kết quả object graph cuối cùng vẫn chứa item không thỏa điều kiện mà endpoint mong muốn.
- Không cần có exception; failure là contract dữ liệu sai.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã tự điều tra và thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

**Before**
- Reproduction xác nhận response object graph có đơn đã đóng.

**After**
- Verification xác nhận collection chỉ chứa đơn đang mở.
- Audit query vẫn có thể tồn tại mà không làm sai contract endpoint.

## Estimated Time

30–50 phút.
