# UNIT-AENUM-001 — Luồng quét kiểm định thang máy vẫn gửi dữ liệu sau khi hủy

## Mục tiêu
Điều tra luồng dữ liệu bất đồng bộ .NET 8; đo sự khác biệt giữa yêu cầu hủy và producer thực sự dừng, rồi kiểm chứng bằng test tái lập.

## Bối cảnh thực tế
Ứng dụng kiểm định thang máy nhận các bản ghi từ thiết bị cầm tay. Sau khi nhân viên hủy phiên, dashboard vẫn xuất hiện thêm các bản ghi.

## Bạn cần làm gì
1. Reproduce, ghi lại số bản ghi nhận được sau tín hiệu hủy.
2. Đặt ít nhất hai hypothesis, lần theo token từ consumer sang producer.
3. Chỉ sửa `starter/ShipmentScanStream.cs` để đáp ứng contract.
4. Verify trên chính starter đã sửa, đánh giá các wrong fixes và regression.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8.x. Không cần Docker, database, network hay NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` phải exit 0 **khi starter còn tái hiện lỗi**. `./scripts/verify.ps1` phải exit 0 **khi starter đã được sửa đúng**. Không dùng solution thay cho starter.

## Những gì cần quan sát
Dòng `RESULT`, `cancellationObserved` và `received`. Consumer yêu cầu hủy ngay sau bản ghi đầu tiên; contract là không có bản ghi tiếp theo. Ghi evidence trước khi thay đổi code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
50 phút (L2 / D4). [Investigation workspace](workspace/my-investigation.md).
