# UNIT-CSVQUOTE-001 — Bản ghi CSV nhiều dòng làm lệch kiểm kê cây giống

## Mục tiêu
Điều tra lỗi import CSV từ đối tác, phân biệt bản ghi logic và dữ liệu đầu vào thực tế, rồi xây dựng regression tests cho cả tình huống bình thường và bất thường.

## Bối cảnh thực tế
Vườn ươm chuyển dữ liệu lô cây giống qua CSV. Một đợt nhập chứa 3 lô hợp lệ nhưng dashboard chỉ nhận 2 lô và báo một dòng dữ liệu lạ. Không có thay đổi database hay network.

## Bạn cần làm gì
Chạy mẫu, lưu kết quả, nêu ít nhất hai giả thuyết, điều tra parser và sửa **starter/Program.cs** mà không thay đổi fixture hay expected assertions.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần database hay Internet sau khi có SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter chưa sửa. Script kỳ vọng một kiểm thử regression thất bại và sẽ báo `REPRODUCED` nếu thấy lỗi đúng như fixture.

## Những gì cần quan sát
So sánh số lô đầu vào với số lô đã parse, giá trị `Note` của lô thứ hai, và diagnostic của parser. Đừng suy luận từ số dòng trong text editor.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Sửa starter, giữ nguyên dữ liệu kiểm thử.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó đọc reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước sửa: fixture hợp lệ bị lỗi; sau sửa: 3 bản ghi đúng, bảo toàn dấu phẩy, dấu ngoặc kép, newline; input hỏng bị từ chối; fixture cơ bản không regression.

## Estimated Time
45 phút.
