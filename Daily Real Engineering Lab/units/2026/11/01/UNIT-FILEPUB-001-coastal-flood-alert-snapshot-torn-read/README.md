# UNIT-FILEPUB-001 — Bản tin cảnh báo triều cường bị đọc dở khi cập nhật

## Mục tiêu

Điều tra và sửa quy trình công bố snapshot JSON để dashboard không đọc bản tin chưa hoàn chỉnh, kể cả khi exporter gặp sự cố.

## Bối cảnh thực tế

Một hệ thống cảnh báo ngập ven biển cập nhật bản tin mỗi giờ. Nhiều dashboard cùng đọc file JSON. Đôi khi dashboard nhận bản tin rỗng; khi exporter dừng bất thường, bản tin hợp lệ cuối cùng biến mất. Yêu cầu vận hành: reader luôn thấy một snapshot hoàn chỉnh và lỗi trước khi công bố không phá hủy bản cũ.

## Bạn cần làm gì

1. Chạy starter, ghi lại bằng chứng của reader khi writer đang tạm dừng.
2. Tái hiện exporter bị lỗi và kiểm tra bản tin còn lại.
3. Viết ít nhất hai giả thuyết và chọn phương án sửa.
4. Chỉ sửa starter/SnapshotPublisher.cs, giữ nguyên API Publish.
5. Chạy verify.ps1, ghi kết quả và đánh giá trade-off trên Windows/NTFS, network share và power loss.

## Yêu cầu môi trường

.NET SDK 8.x, PowerShell 5.1+ hoặc 7; không cần Azure, Docker hay NuGet bên thứ ba. global.json dùng SDK 8.0.100 với latestFeature roll-forward trong 8.x.

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/11/01/UNIT-FILEPUB-001-coastal-flood-alert-snapshot-torn-read"
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Harness sử dụng file giả trong thư mục tạm, kiểm tra reader trùng thời điểm công bố và một lượt exporter phát sinh exception. reproduce.ps1 trả exit code 0 khi **triệu chứng ban đầu** được chứng minh.

## Những gì cần quan sát

- DURING: số byte và nội dung reader nhìn thấy trong khoảng writer tạm dừng.
- AFTER: bản tin mới có được công bố hoàn chỉnh không?
- INTERRUPTED: bản tin hợp lệ cũ có còn sau lỗi không?
- TEMP: có file tạm bị bỏ lại không?
- Phân biệt hợp đồng reproduce và verify; không dựa vào thời gian chạy.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [reference solution](solution/README.md) sau khi đã tự điều tra.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time

60 phút. Ghi chép vào [workspace/my-investigation.md](workspace/my-investigation.md).
