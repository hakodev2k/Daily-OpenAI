# UNIT-COSMOS-002 — Query nhanh ở dev nhưng RU tăng mạnh khi dữ liệu phân mảnh

## Mục tiêu
Điều tra một read path trên Cosmos DB có kết quả đúng nhưng chi phí request tăng mạnh khi số tenant và partition tăng.

## Bối cảnh thực tế
Một API tra cứu đơn hàng hoạt động tốt với dữ liệu dev nhỏ. Sau khi production tăng số tenant, cùng một request bắt đầu tiêu thụ nhiều RU hơn đáng kể dù số record trả về vẫn rất ít. Không có exception và latency chưa phải lúc nào cũng cao.

## Bạn cần làm gì
1. Chạy starter để tạo evidence mô phỏng query trên nhiều logical partition.
2. So sánh số partition được chạm, số item trả về và RU estimate.
3. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
4. Sửa learner-editable query path để giữ đúng business result nhưng giảm phạm vi đọc.
5. Chạy `verify.ps1`.
6. Chỉ xem reference solution sau khi đã tự điều tra.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy dataset deterministic gồm nhiều tenant và in ra `PARTITIONS_SCANNED`, `ITEMS_RETURNED` và `RU_ESTIMATE`.

## Những gì cần quan sát
- Kết quả business có đúng không.
- Một lookup cho một tenant chạm bao nhiêu logical partition.
- RU estimate thay đổi thế nào khi số partition tăng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** lookup trả đúng một order nhưng phải kiểm tra nhiều partition và RU estimate cao hơn cần thiết.

**After:** lookup vẫn trả đúng order nhưng phạm vi đọc được thu hẹp rõ rệt; verification không phụ thuộc timing máy.

## Estimated Time
35–55 phút.