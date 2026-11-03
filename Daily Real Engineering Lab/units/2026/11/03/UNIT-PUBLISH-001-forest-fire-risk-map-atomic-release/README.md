# UNIT-PUBLISH-001 — Forest Fire Risk Map: Bản đồ ghép nhầm hai phiên bản

## Mục tiêu
Phân tích tính nhất quán của một bản phát hành nhiều tài nguyên và đề xuất giao thức publish có thể kiểm chứng.

## Bối cảnh thực tế
Một cổng cảnh báo cháy rừng cung cấp hai ô bản đồ north/south. Trong khi phát hành dự báo mới, một người trực ca nhìn thấy hai ô thuộc hai đợt tính toán khác nhau.

## Bạn cần làm gì
Chạy bản starter, ghi lại snapshot tại checkpoint, nêu ít nhất hai giả thuyết, sửa thiết kế và chứng minh mỗi lần đọc trả về cùng một release. Đề xuất kế hoạch rollback, cache/CDN và xử lý crash.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 (hoặc chạy trực tiếp dotnet). Không cần cloud, mạng hay credentials.

## Chạy nhanh
Tại thư mục unit: `pwsh ./setup.ps1`, `pwsh ./run.ps1`.

## Cách reproduce vấn đề
Chạy `pwsh ./reproduce.ps1`. So sánh snapshot tại checkpoint với snapshot cuối.

## Những gì cần quan sát
Giá trị `visible`, `consistent`, thời điểm checkpoint và số phiên bản trong một lần đọc. Đo xem bản đồ có thể bị trộn trong quá trình cập nhật hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Chạy `pwsh ./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
70 phút.
