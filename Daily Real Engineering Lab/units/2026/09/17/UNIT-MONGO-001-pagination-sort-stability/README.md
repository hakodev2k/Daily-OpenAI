# UNIT-MONGO-001 — Pagination trả record lặp và bỏ sót

## Mục tiêu
Điều tra một lỗi phân trang dữ liệu MongoDB chỉ xuất hiện khi nhiều document có cùng giá trị sắp xếp.

## Bối cảnh thực tế
Một API lịch sử hoạt động ổn với dữ liệu nhỏ. Khi dữ liệu tăng, QA phát hiện đôi lúc cùng một record xuất hiện ở hai page liên tiếp, trong khi record khác biến mất khỏi toàn bộ kết quả. Không có exception và tổng số document trong database vẫn đúng.

## Bạn cần làm gì
1. Chạy starter để tái hiện hai page liên tiếp.
2. Ghi ít nhất hai giả thuyết trước khi sửa.
3. Quan sát thứ tự record và xác định invariant cần có giữa các lần phân trang.
4. Sửa implementation trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra nhiều bộ dữ liệu có giá trị sort trùng nhau.
6. Chỉ xem `solution/README.md` sau khi đã tự điều tra.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị
- Không cần MongoDB server; starter mô phỏng semantics cần thiết bằng dữ liệu local deterministic.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script tạo tập dữ liệu trong đó nhiều bản ghi có cùng timestamp, sau đó đọc page 1 và page 2 theo logic hiện tại.

## Những gì cần quan sát
- ID nào xuất hiện nhiều hơn một lần.
- ID nào không xuất hiện dù thuộc phạm vi hai page.
- Thứ tự có đủ thông tin để tạo một sequence ổn định hay không.

## Expected Results
**Before:** ít nhất một dataset cho thấy duplicate hoặc missing item giữa hai page.

**After:** các page liên tiếp không overlap, không bỏ sót record và giữ thứ tự deterministic cho cùng snapshot dữ liệu.

## Hints
Xem lần lượt `hints/hint-01.md`, `hint-02.md`, `hint-03.md` nếu cần.

## Reference Solution
Chỉ mở `solution/README.md` sau khi đã reproduce, ghi hypothesis và thử fix.

## Estimated Time
30–45 phút.