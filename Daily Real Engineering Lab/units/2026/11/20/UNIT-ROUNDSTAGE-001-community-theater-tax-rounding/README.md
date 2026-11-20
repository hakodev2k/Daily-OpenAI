# UNIT-ROUNDSTAGE-001 — Báo cáo thuế vé nhà hát lệch một xu

## Mục tiêu
Điều tra chênh lệch giữa tổng thuế trên hóa đơn và số thuế gửi sang hệ thống kế toán, xây dựng kiểm chứng ở biên số học.

## Bối cảnh thực tế
Nhà hát cộng đồng bán nhiều loại vé. Báo cáo cuối ngày cho biết số thuế phải nộp khác tổng thuế hiển thị trên từng dòng hóa đơn dù giá vé, thuế suất và số lượng đều giống nhau.

## Bạn cần làm gì
Chạy bản starter, lưu bằng chứng đầu ra, đưa ra ít nhất hai giả thuyết, sửa code trong `starter/` và chạy xác minh.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần dịch vụ trả phí.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit; ghi lại các giá trị `InvoiceTax`, `LedgerTax`, `Delta`.

## Những gì cần quan sát
Đầu ra của từng dòng, tổng hóa đơn và số chuyển sổ; xác định liệu sự khác biệt có ổn định khi chạy lại hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Starter: báo cáo không khớp tổng thuế hóa đơn. Sau sửa: tất cả tình huống regression khớp, giữ chính sách thuế đã công bố.

## Estimated Time
40–55 phút.
