# UNIT-SBUS-001 — Bản ghi sạc của trạm khỏe vẫn bị xử lý trễ

## Mục tiêu
Điều tra backlog của hệ thống xử lý receipt từ các trạm sạc EV; dùng timeline và các phép kiểm tra độc lập để xác định ranh giới tuần tự hóa phù hợp với nghiệp vụ.

## Bối cảnh thực tế
Một operator quản lý nhiều charger. Mỗi charger phải được xử lý receipt theo thứ tự phát sinh, nhưng receipt của các charger khác nhau được phép xử lý độc lập. Sau khi một receipt bị lỗi dữ liệu, dashboard đối soát của cả operator xuất hiện độ trễ bất thường dù các charger còn lại vẫn gửi dữ liệu hợp lệ.

Đây là mô phỏng local, dữ liệu hoàn toàn synthetic; không cần Azure Service Bus.

## Bạn cần làm gì
Chạy baseline, ghi ít nhất ba giả thuyết (ví dụ: lỗi dữ liệu, thiếu worker, giới hạn session, khóa nghiệp vụ), phân tích trace theo cycle và session, sau đó sửa duy nhất code trong `starter/`. Không sửa probe để đạt PASS.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 5.1+ hoặc PowerShell 7. Không cần Docker, cloud hoặc NuGet bên ngoài.

## Chạy nhanh
```powershell
.\setup.ps1
.\run.ps1
.\reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy probe ở chế độ baseline và khẳng định triệu chứng trễ có thể tái hiện. Lệnh này không chứng minh hệ thống đã được sửa. `run.ps1` in toàn bộ session mapping và timeline để điều tra.

## Những gì cần quan sát
- Cycle nhận receipt hợp lệ `B-ok`, `C-ok`, `D-ok` so với receipt lỗi `A-poison`.
- Một receipt lỗi ảnh hưởng tới phạm vi nào? Các receipt của cùng charger có giữ thứ tự không?
- Có bao nhiêu session độc lập được xử lý trong mỗi cycle? Điều gì xảy ra khi retry đạt ngưỡng?
- Dữ liệu nào cần giữ thứ tự tuyệt đối và dữ liệu nào có thể song song?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypotheses tại [workspace](workspace/my-investigation.md).
3. Sửa `starter/SessionPartitioner.cs`; giữ nguyên `probe/`.
4. Chạy `.\verify.ps1` và giải thích vì sao các assertions có ý nghĩa.
5. Chỉ sau đó xem reference.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự thử:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
65 phút — L3 / D4.
