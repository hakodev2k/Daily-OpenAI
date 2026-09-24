# UNIT-BLOB-001 — Blob Upload Mất Nội Dung Sau Bước Inspection

## Mục tiêu
Điều tra một pipeline upload tài liệu mà request thành công nhưng object được lưu không có nội dung như mong đợi; luyện reasoning về state/lifecycle giữa các stage xử lý.

## Bối cảnh thực tế
Một document-ingestion API nhận file, chạy bước inspection để tính checksum/đọc metadata, sau đó chuyển cùng dữ liệu sang storage adapter. Telemetry báo upload thành công, không có exception, nhưng downstream đọc object và nhận 0 byte.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi evidence và ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Sửa code trong `starter/` mà không bỏ bước inspection.
4. Chạy `verify.ps1` để chứng minh nội dung upload đúng và checksum vẫn đúng.
5. Sau đó mới so sánh reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị
- Không cần Azure subscription; storage được mô phỏng local.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script phải chứng minh input có dữ liệu, inspection hoàn tất, operation báo thành công nhưng stored payload không khớp input.

## Những gì cần quan sát
- input length trước pipeline
- checksum được tạo ở bước inspection
- số byte storage adapter thực sự nhận
- trạng thái của object sau upload
- stage nào làm thay đổi điều kiện đầu vào của stage kế tiếp

Không thay đổi storage adapter chỉ để ép test pass; hãy xác định contract giữa các stage.

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
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
35 phút.