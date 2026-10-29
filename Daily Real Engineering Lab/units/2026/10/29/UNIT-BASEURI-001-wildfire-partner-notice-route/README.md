# UNIT-BASEURI-001 — API cảnh báo cháy rừng trả 404 sau nâng cấp đối tác

## Mục tiêu
Điều tra sai lệch giữa smoke test trực tiếp và request do .NET adapter tạo; xác minh hợp đồng HTTP request trên nhiều cấu hình endpoint.

## Bối cảnh thực tế
Một hệ thống cảnh báo cháy rừng gửi bản tin tới API đối tác. Sau cutover v3, smoke test của vận hành nhận HTTP 200, nhưng adapter nhận 404. Endpoint legacy vẫn hoạt động; đối tác xác nhận không downtime.

## Bạn cần làm gì
1. Reproduce, thu thập request URI, status và BaseUrl của ba môi trường.
2. Ghi ít nhất hai hypothesis và evidence phân biệt chúng.
3. Chỉnh sửa duy nhất code người học trong starter/ mà không phá contract legacy.
4. Verify và so sánh kết quả, sau đó mới xem reference solution.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7 hoặc Windows PowerShell. Không cần Azure, Docker, API key, DNS hay kết nối HTTP thật: fake handler xử lý request tại chỗ. SDK targeting packs cần có sẵn hoặc restore được.

## Chạy nhanh
    pwsh -NoProfile -File ./setup.ps1
    pwsh -NoProfile -File ./run.ps1
    pwsh -NoProfile -File ./reproduce.ps1

## Cách reproduce vấn đề
    pwsh -NoProfile -File ./reproduce.ps1

Reproduce trả exit code 0 khi triệu chứng baseline xuất hiện, không có nghĩa code đúng. Nếu đã sửa starter, dùng verify.

## Những gì cần quan sát
So sánh URI thực sự được gửi, HTTP status, BaseUrl và các dữ kiện trong [incident notes](evidence/incident-notes.md). Phân biệt route 404 với lỗi kết nối, TLS, authentication. Ghi điều tra vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng lệnh: pwsh -NoProfile -File ./verify.ps1
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ đọc sau khi tự điều tra và verify](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
40–55 phút (L2 / D4).
