# UNIT-INTOVF-001 — Báo cáo điện mặt trời bị âm khi tăng số lượng điểm đo

## Mục tiêu
Tái hiện sự khác biệt giữa tổng hợp dữ liệu nhỏ và dữ liệu lớn, thu thập bằng chứng ở các ngưỡng dữ liệu, sửa đúng tính toán và bảo vệ invariant trước khi phát hành báo cáo.

## Bối cảnh thực tế
Một hợp tác xã điện mặt trời tổng hợp sản lượng theo ngày từ các điểm đo để cấp tín dụng năng lượng. Bản chạy thử với ít điểm đo luôn đúng, nhưng bản tổng hợp toàn khu vực đôi khi cho kết quả âm dù tất cả số đo đều dương. Phần xuất báo cáo nhận một giá trị kiểu `long`; nhóm vận hành chưa tìm được nguyên nhân.

## Bạn cần làm gì
1. Chạy starter, tái hiện kết quả ở lô nhỏ và lô lớn.
2. Ghi ít nhất hai giả thuyết, số liệu thực tế, điểm ngưỡng và cách loại trừ giả thuyết sai.
3. Chỉ chỉnh `starter/EnergySettlement.cs`; không thay đổi dữ liệu kiểm chứng trong `Program.cs`.
4. Chạy `verify.ps1` trên **starter đã sửa**. Kiểm tra kết quả ở hai phía của ngưỡng, dữ liệu rỗng và dữ liệu không hợp lệ.
5. Sau đó mới so sánh với lời giải.

## Yêu cầu môi trường
- .NET SDK 8 và PowerShell 5.1+ hoặc PowerShell 7+.
- Không cần Docker, database, Azure hay NuGet package ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` **trước khi sửa**. Script trả exit code 0 chỉ khi chứng minh được sai lệch dự kiến ở lô lớn trong khi lô nhỏ đúng. Đây là xác nhận **bug còn tồn tại**, không phải xác nhận đã fix.

## Những gì cần quan sát
- Sản lượng dự kiến và thực tế của 12.000 so với 48.000 điểm đo.
- Tính nhất quán của tổng khi tăng kích thước lô, đặc biệt quanh ranh giới được kiểm thử.
- Dữ liệu đầu vào đều không âm; hãy xác định phép tính nào cần được kiểm tra.
- Sau sửa, cả kết quả nghiệp vụ và điều kiện dữ liệu hợp lệ phải được giữ nguyên.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy `./verify.ps1`; ghi lại bằng chứng.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và code tham khảo](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
35–45 phút (L1 / D3).
