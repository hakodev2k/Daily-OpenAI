# UNIT-CSVDEC-001 — Grain Weighbridge CSV Import

## Mục tiêu
Điều tra một sai lệch dữ liệu nhập khẩu theo môi trường và chứng minh contract về khối lượng chính xác.

## Bối cảnh thực tế
Trạm cân thu mua ngũ cốc nhận CSV từ đối tác; cùng một file chạy trên hai máy có kết quả khác nhau. Chênh lệch khối lượng ảnh hưởng đối soát hóa đơn.

## Bạn cần làm gì
Chạy bản starter, lưu bằng chứng, đưa ra ít nhất hai giả thuyết, sửa code trong `starter/` và xác minh bằng script độc lập.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần cloud hay database.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy hai culture với cùng payload, yêu cầu hai kết quả đều hợp lệ và giống nhau; bản starter ban đầu phải không đạt contract.

## Những gì cần quan sát
Đối chiếu giá trị đầu vào, culture của tiến trình, trạng thái parse và số kg trả về. Không thay dữ liệu nguồn để che lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và phân tích](solution/README.md).

## Expected Results
Trước: ít nhất một môi trường không thỏa contract. Sau: mọi culture trong test đều nhập `1250.75` kg chính xác; dữ liệu sai định dạng bị từ chối.

## Estimated Time
40 phút.
