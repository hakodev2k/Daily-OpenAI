# UNIT-ZIPSLIP-001 — Ranh giới giải nén hồ sơ bảo tàng

## Mục tiêu
Điều tra sự cố import ZIP, thiết lập hợp đồng an toàn cho đường dẫn đầu ra, và chứng minh bằng regression tests.

## Bối cảnh thực tế
Hệ thống số hóa hiện vật nhận gói ZIP do đối tác gửi. Một gói được chấp nhận nhưng sau import, hồ sơ ở thư mục bên cạnh bị thay đổi. Log chỉ ghi "import succeeded".

## Bạn cần làm gì
Reproduce bằng fixture có kiểm soát; thu thập danh sách entry và vị trí file được tạo; ghi ít nhất hai giả thuyết; sửa mã trong `starter/ArchiveImporter.cs` mà không bỏ khả năng import gói hợp lệ; chạy verify.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ hoặc shell tương đương; không cần dịch vụ ngoài.

## Chạy nhanh
Tại thư mục unit: `pwsh ./reproduce.ps1`; sau khi sửa: `pwsh ./verify.ps1`.

## Cách reproduce vấn đề
Chạy `pwsh ./reproduce.ps1`. Script tạo ZIP cục bộ, kiểm tra triệu chứng và dọn sandbox. Không chạy với thư mục chứa dữ liệu thật.

## Những gì cần quan sát
So sánh đường dẫn entry với đường dẫn file thực tế. Ghi nhận tệp nào xuất hiện ngoài thư mục import. Theo dõi cách xử lý entry bình thường, entry thư mục và entry bất thường.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
Trước: fixture đối kháng tạo tệp ngoài import root. Sau: fixture bị từ chối, không có ghi file ngoài root; ZIP hợp lệ vẫn import đúng.

## Estimated Time
60–90 phút.
