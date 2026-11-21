# UNIT-ASYNCENUM-001 — Luồng cập nhật nơi trú ẩn vẫn chạy sau khi người nhận hủy

## Mục tiêu
Điều tra lifecycle của một luồng dữ liệu bất đồng bộ và chứng minh rằng yêu cầu hủy từ consumer phải chấm dứt công việc producer.

## Bối cảnh thực tế
Dịch vụ điều phối nơi trú ẩn phát các bản cập nhật sức chứa qua async stream. Khi màn hình điều phối đóng, consumer gửi tín hiệu hủy. Log tại backend vẫn ghi nhận các lượt lấy dữ liệu sau thời điểm hủy, gây chi phí và làm chậm worker.

## Bạn cần làm gì
1. Chạy bản starter, lưu output và xác định sự khác biệt giữa tín hiệu hủy và công việc thực sự chấm dứt.
2. Đưa ra ít nhất hai giả thuyết, kiểm tra đường truyền cancellation và sửa phần learner-editable.
3. Chạy verify, kiểm tra luồng bình thường vẫn phát đủ ba bản tin.

## Yêu cầu môi trường
PowerShell 7 hoặc Windows PowerShell 5.1; .NET SDK 8.x. Không cần database, Azure hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```
Nếu execution policy chặn script, chạy `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1`.

## Cách reproduce vấn đề
`./reproduce.ps1` chạy probe với một lượt hủy sau bản tin đầu tiên; script thành công khi quan sát được triệu chứng starter (không phải khi hệ thống đã được sửa).

## Những gì cần quan sát
- Có bao nhiêu bản tin được phát sau khi consumer đã hủy?
- Probe báo `CANCEL_OBSERVED` hay `CANCEL_IGNORED`?
- Lệnh verify có phát hiện regression ở đường đọc bình thường không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/ShelterFeed.cs`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results
Trước sửa: probe phát hiện consumer đã hủy nhưng producer vẫn trả thêm bản tin. Sau sửa: hủy được quan sát trước bản tin tiếp theo; luồng không hủy vẫn trả đúng ba bản tin.

## Estimated Time
45 phút.

