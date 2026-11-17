# UNIT-SEMLEAK-001 — Cổng bốc dỡ vượt giới hạn sau khi yêu cầu bị hủy

## Mục tiêu
Điều tra một lỗi giới hạn số lượt xử lý đồng thời trong hệ thống điều phối kho; dựa trên kết quả chạy và thứ tự sự kiện trước khi sửa.

## Bối cảnh thực tế
Kho có một cổng bốc dỡ chỉ cho phép một xe xử lý tại một thời điểm. Trong giờ cao điểm, một yêu cầu đang chờ bị hủy, rồi một xe khác đến. Dashboard ghi nhận hai xe cùng được cấp quyền vào cổng.

## Bạn cần làm gì
Chạy chương trình mẫu, lưu lại chuỗi sự kiện, nêu ít nhất hai giả thuyết, sửa logic điều phối và chứng minh rằng yêu cầu bị hủy không làm tăng số suất phục vụ.

## Yêu cầu môi trường
.NET SDK 8 trở lên và PowerShell 7 hoặc Windows PowerShell.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet run --project starter/WarehouseGate.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Lệnh kiểm tra sẽ trả exit code khác 0 nếu phát hiện vượt giới hạn.

## Những gì cần quan sát
Ghi lại thứ tự A, B, C; số yêu cầu đang hoạt động; thời điểm hủy B; thời điểm C được phép vào cổng. Chỉ dựa trên evidence, không suy luận từ tên biến.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý từng mức](hints.md).

## Reference Solution
**Spoiler:** [Xem lời giải sau khi tự thử](solution/README.md).

## Expected Results
Trước sửa: `VIOLATION` và exit code 1. Sau sửa: `PASS`, không vượt giới hạn, exit code 0.

## Estimated Time
35 phút.
