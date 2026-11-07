# UNIT-OBS-007 — Trace Sampling Blind Spot

## Mục tiêu
Điều tra một incident mà dashboard latency và tập trace đang kể hai câu chuyện khác nhau. Bạn cần thu thập evidence, xây dựng hypothesis, xác định giới hạn của telemetry hiện tại và đề xuất cách kiểm chứng/fix có thể vận hành trong production.

## Bối cảnh thực tế
Partner Order Import API xử lý lưu lượng ổn định. Dashboard cho thấy p99 thỉnh thoảng tăng mạnh trong vài phút, nhưng khi team mở distributed traces để tìm request chậm thì gần như tất cả trace được lưu đều nhanh. CPU, memory và error rate không có spike tương ứng.

Team chưa biết đây là vấn đề downstream dependency, queueing, network, telemetry pipeline hay một giả thuyết khác.

## Bạn cần làm gì
1. Chạy simulator ở trạng thái starter.
2. Reproduce sự khác biệt giữa latency summary và tập trace được giữ lại.
3. Ghi evidence và ít nhất 3 hypothesis vào `workspace/my-investigation.md`.
4. Chỉ sau khi có hypothesis, mở hints nếu cần.
5. Thay đổi learner-editable configuration/code để telemetry giữ được evidence phù hợp cho loại incident này mà không đơn giản lưu 100% mọi request.
6. Chạy verification.
7. So sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription; lab mô phỏng sampling local.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- p50/p95/p99 của toàn bộ request mô phỏng.
- Số trace được giữ lại.
- Phân bố latency của trace được giữ lại.
- Có hay không evidence tương ứng với các request nằm ở tail latency.
- Tỷ lệ telemetry được lưu so với tổng traffic.

Không kết luận root cause chỉ từ một metric hoặc một trace.

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
[Spoiler — chỉ xem sau khi đã tự điều tra và thử fix](solution/README.md)

## Expected Results
Trước khi sửa, summary vẫn phản ánh tail latency nhưng tập trace được giữ lại không cung cấp đủ evidence đại diện cho các request chậm. Sau khi sửa, verification phải chứng minh telemetry có thể giữ lại các request có giá trị chẩn đoán trong khi vẫn giới hạn volume.

## Estimated Time
Khoảng 50 phút.