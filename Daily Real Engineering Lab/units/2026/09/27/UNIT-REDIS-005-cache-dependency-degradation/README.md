# UNIT-REDIS-005 — Cache Dependency Degradation Under Partial Outage

## Mục tiêu
Điều tra một content-delivery read path hoạt động bình thường khi cache khỏe nhưng bắt đầu trả lỗi trong một khoảng outage ngắn, dù authoritative content source vẫn hoạt động.

## Bối cảnh thực tế
Một CMS content API dùng Redis như lớp tăng tốc cho nội dung đã publish. Trong một incident ngắn, cache node có timeout trong vài request rồi tự phục hồi. Monitoring cho thấy source chính vẫn đọc được dữ liệu, nhưng một phần request client nhận lỗi.

## Bạn cần làm gì
1. Chạy starter ở trạng thái ban đầu.
2. Reproduce incident bằng script được cung cấp.
3. Ghi evidence và ít nhất ba hypothesis vào workspace/my-investigation.md.
4. Xác định dependency nào giữ authoritative data và dependency nào chỉ tăng tốc.
5. Sửa starter/ để request vẫn đúng trong outage mô phỏng nhưng healthy cache vẫn thực sự có giá trị.
6. Chạy verify.ps1.
7. Chỉ sau khi tự thử mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần Redis server; lab dùng deterministic local cache simulator.

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
`./reproduce.ps1`

Script mô phỏng 8 request tuần tự. Cache không phản hồi ở request 4 và 5, sau đó phục hồi.

## Những gì cần quan sát
- Request nào fail và request nào vẫn trả đúng content.
- Số lần authoritative origin được gọi.
- Cache có hoạt động lại sau khi dependency phục hồi hay không.
- Failure có xảy ra ở read path, write path, hay cả hai.
- Behavior sau khi sửa có giữ nguyên cancellation semantics và functional result hay không.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before:
- Request bình thường trước outage trả đúng content.
- Một outage ngắn của cache làm một số request fail dù origin vẫn khỏe.
- Cache tự hoạt động lại sau outage.

After:
- Tất cả request trả đúng content trong kịch bản mô phỏng.
- Origin chỉ chịu thêm tải trong khoảng cần thiết.
- Khi cache phục hồi, read path lại hưởng lợi từ cache.
- Cache failure không được biến thành lý do mặc định để bỏ qua cancellation hoặc các lỗi lập trình khác.

## Estimated Time
40–55 phút.
