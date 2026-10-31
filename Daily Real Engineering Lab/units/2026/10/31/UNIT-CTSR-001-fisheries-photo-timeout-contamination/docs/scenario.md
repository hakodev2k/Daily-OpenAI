# Scenario và evidence contract

| Ảnh | Partner simulator | Mong đợi |
|---|---|---|
| PHOTO-001 | Chờ đến khi deadline hủy | TimedOut |
| PHOTO-002 | CompletedTask ngay | Accepted |
| PHOTO-003 | CompletedTask ngay | Accepted |
| PHOTO-004 | Caller đã hủy | OperationCanceledException |

Deadline mỗi ảnh là 250 ms. Dùng cùng service instance, xử lý tuần tự. Không có network hay randomness. Khi debug, ghi `IsCancellationRequested` và kết quả cho từng ảnh. Không coi ba nhãn timeout là bằng chứng ba request đều chậm.

Bài này là simulator chạy local; không khẳng định đã đo trên production.
