# UNIT-JSONENUM-001 — Marine sample intake: trạng thái bất thường từ partner

## Mục tiêu
Điều tra hợp đồng dữ liệu JSON tại boundary hệ thống tiếp nhận mẫu biển; đánh giá cách xử lý trạng thái không mong đợi.

## Bối cảnh thực tế
Đối tác gửi cập nhật trạng thái mẫu nghiên cứu biển. Một số payload khiến hệ thống nhận dữ liệu không nhất quán với danh mục trạng thái đã phê duyệt.

## Bạn cần làm gì
Chạy fixture, ghi evidence và hai hypothesis, sửa parser trong `starter/Program.cs`, chứng minh trạng thái hợp lệ vẫn được chấp nhận.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần database hay dịch vụ ngoài.

## Chạy nhanh
Tại thư mục lab chạy `./run.ps1`. Exit code khác 0 khi contract chưa đạt.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` để xác nhận starter có fixture fail. Ghi case ID, accepted và exit code.

## Những gì cần quan sát
So sánh kết quả chấp nhận bốn payload với expected. Phân biệt dữ liệu parse được với dữ liệu hợp lệ theo business contract.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Chạy `./verify.ps1` trên starter đã sửa.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — lời giải](solution/README.md)

## Expected Results
Starter có fixture FAIL; sau sửa cả bốn PASS, `contract_failures=0`, exit 0.

## Estimated Time
35–45 phút.
