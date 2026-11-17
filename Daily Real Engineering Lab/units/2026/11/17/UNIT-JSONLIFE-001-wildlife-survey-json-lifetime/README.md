# UNIT-JSONLIFE-001 — Camera survey event: dữ liệu biến mất sau ingest

## Mục tiêu
Điều tra ownership và lifetime của dữ liệu JSON khi chuyển công việc từ ingestion sang xử lý bất đồng bộ; chứng minh fix bằng regression test.

## Bối cảnh thực tế
Dịch vụ kiểm kê camera bẫy ảnh nhận JSON từ thiết bị và chuyển event sang hàng đợi xử lý nội bộ. Payload hợp lệ, parser báo thành công, nhưng worker đôi lúc không thể đọc các trường event. Lỗi chỉ xuất hiện khi tách hai giai đoạn xử lý.

## Bạn cần làm gì
Chạy starter, ghi nhận stack trace, phân biệt lỗi parser, queue và lifetime. Đề xuất ít nhất hai giả thuyết; sửa code trong `starter/Program.cs` mà không thay đổi payload hay bỏ qua validation.

## Yêu cầu môi trường
.NET SDK 8.x; không cần Docker, database hay dịch vụ trả phí.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`, quan sát exit code và exception; chạy lại để xác nhận tính xác định. Chạy `./verify.ps1` trước và sau khi tự sửa.

## Những gì cần quan sát
Ghi lại thời điểm ingest trả về, thời điểm worker đọc payload, exception type, stack trace, và liệu payload đầu vào có thay đổi hay không. Phân biệt lỗi functional và lỗi thời điểm thực thi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ xem sau khi tự sửa](solution/README.md).

## Expected Results
Starter: ingest thành công nhưng xử lý event thất bại. Sau fix: worker đọc được `cameraId` và `species`, test chống regression thành công. Không dựa vào timing.

## Estimated Time
35–45 phút.
