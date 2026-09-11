# UNIT-REDIS-001 — Redis Client Churn Under Concurrent Requests

## Mục tiêu
Điều tra một API đọc cache có functional result đúng nhưng latency tăng mạnh khi concurrent traffic tăng. Tập trung vào lifecycle của Redis client, connection reuse và evidence trước khi tối ưu.

## Bối cảnh thực tế
Một pricing API dùng Redis cho hot reads. Ở traffic thấp mọi thứ bình thường. Khi chạy burst, p95 tăng, số connection handshake tăng theo request count và cache server chưa hề quá tải CPU.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1`.
2. Quan sát elapsed time, số client được tạo và số handshake.
3. Ghi ít nhất hai hypothesis.
4. Sửa code trong `starter/`.
5. Chạy `./verify.ps1`.
6. Sau đó mới xem `solution/README.md`.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell
- Không cần Redis thật; lab dùng deterministic simulator để cô lập connection-lifecycle cost.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy 24 request song song qua cùng service. Mỗi request đều trả đúng giá, nhưng simulator ghi nhận connection setup lặp lại nhiều lần và tổng thời gian vượt ngưỡng mong đợi.

## Những gì cần quan sát
- Business result vẫn đúng.
- Connection setup count tăng gần với request count.
- CPU work rất nhỏ so với thời gian chờ setup.
- Bottleneck nằm ở lifecycle/resource reuse, không phải cache lookup logic.

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
> Spoiler: chỉ mở sau khi đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results
**Before:** kết quả đúng nhưng handshake/client creation tăng mạnh theo concurrency.

**After:** các request dùng lại shared connection-oriented client; handshake count không còn tỷ lệ với request count; functional result giữ nguyên.

## Estimated Time
30–45 phút.