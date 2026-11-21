# UNIT-CULTUREPARSE-001 — Dữ liệu cảm biến thời tiết đổi nghĩa giữa hai region

## Mục tiêu
Điều tra lỗi import dữ liệu khi cùng payload đi qua hai môi trường khác nhau; xác định bằng chứng, sửa luồng xử lý và bảo vệ hợp đồng dữ liệu bằng regression tests.

## Bối cảnh thực tế
Một mạng trạm đo thời tiết gửi giá trị nhiệt độ dạng text qua giao thức cố định. Một số trạm được chuyển sang worker chạy tại khu vực khác. Sau triển khai, dashboard ghi nhận một số bản tin bị từ chối, một số giá trị bất thường. Payload đầu vào không đổi.

## Bạn cần làm gì
1. Chạy starter với hai cấu hình môi trường.
2. Ghi nhận output, exit code và giả thuyết trước khi mở hints.
3. Sửa duy nhất mã trong `starter/` để cả hai môi trường xử lý cùng một payload nhất quán.
4. Chạy `verify.ps1`; phân tích các input bất thường và trade-off của việc chấp nhận định dạng cũ.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7 (`pwsh`). Không cần Docker, cloud hoặc NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit: `pwsh ./reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `pwsh ./reproduce.ps1`. Script gọi cùng chương trình với `en-US` và `fr-FR`; so sánh kết quả cho các bản tin `S-41|12.75`, `S-42|-3.50`.

## Những gì cần quan sát
- Có cùng số bản tin được nhận ở cả hai lần chạy không?
- Có khác biệt về giá trị đo hoặc lỗi import không?
- Log hiển thị môi trường và raw payload thế nào?
- Phân biệt lỗi định dạng đầu vào với lỗi tính toán phía sau.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md).

## Reference Solution
**Spoiler:** Chỉ xem sau khi hoàn tất thử nghiệm: [solution](solution/README.md).

## Expected Results
Trước fix: cùng bản tin có thể được xử lý khác nhau theo cấu hình môi trường. Sau fix: bản tin hợp lệ cho cùng kết quả, bản tin không hợp lệ bị từ chối nhất quán; không làm tròn hoặc thay đổi đơn vị đo.

## Estimated Time
50 phút.
