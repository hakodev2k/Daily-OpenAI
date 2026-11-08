# UNIT-POOLBUF-001 — Ảnh khảo sát đầm lầy bị sai checksum sau xử lý bất đồng bộ

## Mục tiêu
Điều tra lỗi dữ liệu không ổn định trong pipeline xử lý ảnh .NET; xây dựng bằng chứng và regression test có thể chạy lặp lại.

## Bối cảnh thực tế
Ứng dụng ghi nhận khảo sát đầm lầy nhận byte thumbnail từ thiết bị ngoại tuyến. Trong một ca tải cao, bản ghi đã được xác nhận tiếp nhận nhưng nội dung được lưu không khớp checksum của thiết bị. Tỷ lệ lỗi tăng khi có thêm công việc chạy đồng thời.

## Bạn cần làm gì
Reproduce lỗi; ghi lại byte mong đợi, byte nhận được và thời điểm từng bước. Đề xuất ít nhất hai giả thuyết, sửa mã trong `starter/Program.cs`, chứng minh không mất dữ liệu và không giữ tài nguyên quá lâu.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần mạng, cloud hay database.

## Chạy nhanh
```powershell
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Lệnh phải trả exit code khác 0 với starter gốc; dùng pool mô phỏng để không phụ thuộc timing hoặc tải máy.

## Những gì cần quan sát
So sánh chuỗi byte gốc với payload cuối, thứ tự các mốc trong log, kết quả kiểm tra integrity và exit code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./scripts/verify.ps1` trên starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md).

## Expected Results
Trước: payload cuối không khớp 4 byte gốc; chương trình báo FAIL. Sau: payload chính xác và kiểm tra PASS qua nhiều lượt.

## Estimated Time
50 phút — L2 / D4.
