# UNIT-LINQ-008 — Một batch, hai lần enumerate, hai snapshot khác nhau

## Mục tiêu
Điều tra một batch export có kết quả tổng hợp và dữ liệu xuất ra không nhất quán dù không có exception.

## Bối cảnh thực tế
Một scheduled billing job lấy các invoice đang chờ xử lý. Job tính tổng tiền để ghi audit, sau đó ghi từng invoice ra export. Trong môi trường thật, nguồn dữ liệu có thể thay đổi giữa các bước.

## Bạn cần làm gì
Reproduce, ghi hypothesis, xác định contract của sequence trong một business operation, sửa starter và verify mà không thay đổi yêu cầu nghiệp vụ.

## Yêu cầu môi trường
.NET 8 SDK.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script yêu cầu starter biểu hiện inconsistency một cách deterministic.

## Những gì cần quan sát
So sánh số lần nguồn dữ liệu được đọc, giá trị audit total và các invoice thực sự được export. Tập trung vào evidence thay vì đoán từ tên API.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
`hints/hint-01.md` → `hint-02.md` → `hint-03.md`.

## Reference Solution
Xem `solution/README.md` sau khi đã tự thử.

## Expected Results
Trước fix: hai bước trong cùng batch không nhất thiết quan sát cùng tập invoice. Sau fix: audit và export phải dùng cùng một logical snapshot, đồng thời nguồn chỉ được đọc một lần cho operation đó.

## Estimated Time
35 phút.