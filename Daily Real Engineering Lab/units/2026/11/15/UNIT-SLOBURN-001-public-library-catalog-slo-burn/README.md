# UNIT-SLOBURN-001 — Cảnh báo SLO cho cổng tra cứu thư viện công cộng

## Mục tiêu
Từ log tổng hợp theo cửa sổ thời gian, thiết kế và kiểm chứng một chính sách cảnh báo đủ nhanh khi dịch vụ suy giảm nghiêm trọng nhưng không đánh thức on-call vì nhiễu lưu lượng thấp. Phân biệt SLI, SLO, error budget và alert routing.

## Bối cảnh thực tế
Cổng tra cứu thư viện thành phố phục vụ tra cứu đầu sách và đặt chỗ. Sau một đợt thay đổi cảnh báo, đội vận hành báo rằng có ngày bị đánh thức vì vài lỗi nhỏ, nhưng có đợt người dùng không đặt được sách trong nhiều phút mà cảnh báo đến quá muộn. Bạn là engineer phụ trách cải tiến chính sách; chưa được thay đổi production.

## Bạn cần làm gì
1. Đọc `evidence/windows.csv` và hợp đồng đo lường trong `docs/scenario.md`.
2. Điền `workspace/my-investigation.md`: tự tính error ratio và burn rate cho từng cửa sổ; ghi ít nhất hai giả thuyết.
3. Viết policy mới với các mức page/ticket/no-alert, đánh giá từng incident và giải thích trade-off.
4. So sánh với `solution/README.md` sau khi hoàn tất.

## Yêu cầu môi trường
Chỉ cần trình soạn thảo, PowerShell 7 (khuyến nghị) hoặc Excel; không cần Azure, database, tài khoản cloud hay secret.

## Chạy nhanh
```powershell
Import-Csv ./evidence/windows.csv | Format-Table
```
Có thể dùng `./scripts/replay.ps1` để xem số liệu cơ sở, sau đó tự xây policy trong workspace.

## Cách reproduce vấn đề
Đọc policy hiện tại ở `docs/scenario.md`; áp dụng lần lượt lên các cửa sổ trong CSV và so sánh với tác động người dùng. Ghi nhận số page không cần thiết và incident có mức độ nghiêm trọng nhưng bị phân loại sai.

## Những gì cần quan sát
So sánh tỷ lệ lỗi, mẫu số request, khoảng thời gian và số phút tiêu thụ error budget. Đừng mặc định mọi cửa sổ có tỷ lệ lỗi giống nhau đều có mức ưu tiên giống nhau.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử policy mới.
4. Verify bằng toàn bộ case.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý theo cấp độ](hints.md).

## Reference Solution
**Spoiler:** [Phân tích và phương án tham khảo](solution/README.md).

## Expected Results
Policy phải phân loại nhất quán tất cả case, có phương án xử lý khi traffic thấp, ghi rõ giới hạn dữ liệu và các bước rollout an toàn.

## Estimated Time
45–55 phút.
