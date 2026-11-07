# UNIT-READY-001 — Bảng chỗ trống trung tâm tránh nóng sai khi khởi động

## Mục tiêu

Điều tra lỗi đồng bộ dữ liệu ban đầu, thu thập evidence, đề xuất hypothesis, sửa và verify tính đúng đắn cả đường success và fault.

## Bối cảnh thực tế

Dashboard cộng đồng hiển thị số ghế trống ở các trung tâm tránh nóng. Sau deploy, một số request đầu trả dữ liệu trống dù đối tác có snapshot hợp lệ. Khi đối tác nhanh, lỗi không xuất hiện; đôi khi lỗi tải snapshot cũng không được báo tới reader.

## Bạn cần làm gì

Chạy starter và quan sát ba tình huống nguồn chậm, nhanh và lỗi. Ghi tối thiểu hai hypothesis, nêu cách bác bỏ. Chỉ sửa starter/, kiểm tra rằng dữ liệu và lỗi được thể hiện đúng.

## Yêu cầu môi trường

.NET SDK 8.x và PowerShell, không cần cloud, database, Docker hay NuGet ngoài SDK.

## Chạy nhanh

~~~powershell
cd 'Daily Real Engineering Lab/units/2026/11/02/UNIT-READY-001-cooling-center-occupancy-startup-readiness'
./setup.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề

Chạy ./reproduce.ps1 trên starter chưa sửa. Exit code 0 nghĩa là triệu chứng được xác nhận, không có nghĩa hệ thống đúng.

## Những gì cần quan sát

So sánh trạng thái trả về với thời điểm nguồn dữ liệu thực sự sẵn sàng; xem fast path và failure path. Giải thích vì sao kiểm thử dựa vào thời gian chạy thật dễ bỏ sót vấn đề.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Verify bằng ./verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Lời giải](solution/README.md), chỉ đọc sau khi tự điều tra.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). Verify kiểm tra code learner sửa trong starter/, không chỉ reference.

## Estimated Time

60–80 phút. L4 / D4.
