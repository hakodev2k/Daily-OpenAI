# UNIT-SQLNULL-001 — Danh sách cấp phát hạt giống bị rỗng bất thường

## Mục tiêu
Điều tra sự khác biệt giữa kết quả truy vấn và kỳ vọng nghiệp vụ; thu thập bằng chứng SQL trước khi sửa.

## Bối cảnh thực tế
Hợp tác xã cấp hạt giống cho các hộ đủ điều kiện, loại trừ những hộ đã được cấp. Sau đợt nhập dữ liệu từ hệ thống cũ, báo cáo không còn hộ nào đủ điều kiện dù dữ liệu đăng ký vẫn tồn tại.

## Bạn cần làm gì
Chạy dữ liệu mô phỏng, kiểm tra truy vấn báo cáo, viết ít nhất hai giả thuyết, sửa **starter/query.sql** mà không thay đổi dữ liệu và chứng minh kết quả đúng.

## Yêu cầu môi trường
Python 3.10+ (sqlite3 thuộc standard library), PowerShell 5.1+; không cần database server hoặc dịch vụ trả phí.

## Chạy nhanh
Tại thư mục unit: `./scripts/run.ps1`, sau đó `./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`; script xác nhận trạng thái ban đầu có triệu chứng cần điều tra và in số lượng kết quả.

## Những gì cần quan sát
Đối chiếu số hộ đăng ký, bản ghi cấp phát, giá trị dữ liệu nhập và danh sách trả về. Không sửa fixture để làm bài test pass.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/query.sql`.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) — chỉ đọc sau khi tự điều tra.

## Expected Results
Trước sửa: truy vấn trả danh sách rỗng dù có hộ đủ điều kiện. Sau sửa: đúng các hộ chưa được cấp, không trùng và không chứa hộ đã cấp.

## Estimated Time
45–60 phút.
