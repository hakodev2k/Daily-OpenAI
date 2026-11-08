# UNIT-QUOTAS-001 — API quota công bằng giữa các tenant

## Mục tiêu
Đánh giá admission control của API đa tenant khi upstream có giới hạn 8 request trong cửa sổ 2 giây; thiết kế chính sách công bằng và chứng minh bằng replay.

## Bối cảnh thực tế
Một cổng nhận số liệu đồng hồ nước phục vụ đối tác gửi batch lớn, phòng khám và trường học. Tất cả cùng sử dụng upstream có giới hạn xử lý. SLA yêu cầu các tenant nhỏ không bị loại toàn bộ khi tenant lớn gửi burst.

## Bạn cần làm gì
Chạy trace, lập bảng accepted/rejected theo tenant, đưa ra ít nhất hai chính sách quota khả thi; chọn một chính sách và sửa `starter/Program.cs`. Giữ giới hạn tổng 8, nhận đủ hai request của mỗi tenant `clinic` và `school`, và không loại toàn bộ tenant `bulk`. Ghi trade-off về thứ tự, fairness, throughput, Retry-After và key tenant đáng tin cậy.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell.

## Chạy nhanh
```powershell
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Script chạy replay và kiểm tra biểu hiện ban đầu. Không chỉnh starter trước bước này.

## Những gì cần quan sát
Đối chiếu mã 200/429 và số request được nhận của từng tenant; ghi số liệu trước khi sửa. Thứ tự input là thứ tự đến, không nhất thiết là thứ tự xử lý tối ưu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phân tích và phương án tham chiếu](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` chạy trực tiếp code người học sửa trong `starter/`, không kiểm tra riêng reference.

## Estimated Time
50 phút.
