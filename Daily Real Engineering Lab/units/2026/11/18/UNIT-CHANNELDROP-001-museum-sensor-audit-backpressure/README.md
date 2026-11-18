# UNIT-CHANNELDROP-001 — Audit cảm biến bảo tàng mất bản ghi khi hàng đợi quá tải

## Mục tiêu
Tự điều tra hợp đồng delivery của pipeline bất đồng bộ, phân biệt request được chấp nhận với event được lưu bền vững, và kiểm tra overload.

## Bối cảnh thực tế
Hệ thống giám sát nhiệt độ phòng trưng bày nhận ba sự kiện từ cảm biến khi downstream chậm. API báo cả ba đã nhận nhưng báo cáo audit chỉ có hai. Không được đánh mất dữ liệu audit.

## Bạn cần làm gì
Chạy starter, lưu output, viết ít nhất hai hypothesis, xác định điểm mất dữ liệu, sửa pipeline trong starter và kiểm tra mọi event được xử lý đúng thứ tự. Đánh giá trade-off khi consumer bị treo.

## Yêu cầu môi trường
PowerShell 7+ và .NET SDK 8.x; không cần mạng hoặc dịch vụ ngoài.

## Chạy nhanh
`pwsh ./scripts/run.ps1`

## Cách reproduce vấn đề
`pwsh ./scripts/reproduce.ps1` phải báo reproduction thành công khi quan sát được mismatch 3/2 trên starter gốc.

## Những gì cần quan sát
So sánh số event được producer xác nhận, số event consumer nhận, ID bị thiếu và thời điểm consumer bắt đầu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md)

## Expected Results
Reproduce: `Accepted=3 Persisted=2`. Sau sửa: `Accepted=3 Persisted=3`, đủ ID 101,102,103.

## Estimated Time
40 phút.
