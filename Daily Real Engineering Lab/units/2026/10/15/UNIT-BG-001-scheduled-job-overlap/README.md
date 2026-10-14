# UNIT-BG-001 — Background job xử lý trùng khi tải tăng

## Mục tiêu
Điều tra một job xử lý batch trên .NET 8: xác định vì sao số batch đang xử lý đồng thời tăng khi downstream chậm; đưa ra evidence, sửa vòng đời job và kiểm chứng tính tuần tự cũng như shutdown.

## Bối cảnh thực tế
Một worker đồng bộ trạng thái đơn hàng từ hệ thống đối tác. Bình thường mỗi đợt xử lý kết thúc nhanh. Khi đối tác chậm, dashboard báo nhiều batch đang chạy cùng lúc, log xuất hiện các sự kiện xử lý đan xen. Team nghi ngờ retry, duplicate delivery hoặc một bản deployment thứ hai.

## Bạn cần làm gì
1. Chạy bản baseline để reproduce.
2. Ghi số lượng batch bắt đầu, `maxActive`, và thứ tự `BATCH_START`/`BATCH_COMPLETE`.
3. Viết tối thiểu ba hypotheses; chọn evidence để loại trừ từng giả thuyết.
4. Chỉ sửa `starter/JobRunner.cs`. Không sửa harness, baseline hay scripts.
5. Chạy `verify.ps1` và giải thích vì sao fix vẫn đúng khi shutdown giữa batch.

## Yêu cầu môi trường
- .NET 8 SDK; PowerShell 7+ hoặc Windows PowerShell.
- Không cần database, Docker hay Azure.

## Chạy nhanh
```powershell
./reproduce.ps1
./verify.ps1
```
`verify.ps1` dự kiến FAIL trước khi bạn sửa starter.

## Cách reproduce vấn đề
`reproduce.ps1` chạy baseline cố định, không bị ảnh hưởng bởi sửa đổi trong starter. Harness tạo downstream chậm có kiểm soát, quan sát 180ms và in counters. Không phụ thuộc network hoặc timing chính xác của máy.

## Những gì cần quan sát
- `maxActive` và số lần `BATCH_START` khi một batch vẫn đang chờ.
- Sự khác biệt giữa thời gian chờ downstream và nhịp gọi job.
- Liệu việc dừng scheduler có chờ xử lý còn dang dở hay không.
- Evidence nào chứng minh overlap mà không cần suy luận từ log timestamp đơn thuần.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
55–70 phút.
