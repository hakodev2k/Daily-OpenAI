# UNIT-HTTP-001 — HTTP client degrades after repeated partial reads

## Mục tiêu
Điều tra một HTTP integration chạy ổn lúc đầu nhưng dần mất khả năng phục vụ request khi workload lặp lại.

## Bối cảnh thực tế
Một background worker gọi pricing service theo batch. Mỗi call chỉ cần status và một phần metadata; ở môi trường dev vài request đầu đều thành công. Khi chạy batch dài, throughput giảm rồi request bắt đầu timeout dù downstream giả lập vẫn phản hồi nhanh.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi hypothesis trước khi sửa.
3. Dùng evidence từ output để xác định resource nào không quay lại trạng thái reusable.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra cả correctness lẫn khả năng chạy nhiều vòng.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` build và chạy workload lặp với một handler mô phỏng connection capacity hữu hạn. Không cần network hoặc service ngoài.

## Những gì cần quan sát
- Số request thành công trước khi workload dừng tiến triển.
- Counter `LeasedSlots` sau mỗi vòng.
- Downstream latency giả lập vẫn ổn trong khi client-side capacity thay đổi.

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
[Reference Solution](solution/README.md) — spoiler, chỉ xem sau khi đã thử.

## Expected Results
Trước fix, workload không hoàn tất toàn bộ batch và capacity bị giữ lại. Sau fix, toàn bộ batch hoàn tất và capacity trở về trạng thái ban đầu.

## Estimated Time
35–55 phút.