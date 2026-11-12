# UNIT-MUTDICT-001 — Parcel Routing: bản ghi biến mất sau khi đổi tuyến

## Mục tiêu
Điều tra lỗi tra cứu trong bộ nhớ, thu thập bằng chứng trước và sau một thao tác cập nhật nghiệp vụ, sau đó sửa mà vẫn bảo đảm tra cứu O(1) trung bình.

## Bối cảnh thực tế
Dịch vụ điều phối bưu kiện lưu manifest của các kiện đã nhận. Nhân viên sửa tuyến giao hàng khi xe đổi lịch; ngay sau đó màn hình tra cứu thông báo không tìm thấy kiện dù số lượng bản ghi không đổi.

## Bạn cần làm gì
Chạy starter, ghi lại mã kiện và tuyến trước/sau cập nhật, đề xuất ít nhất hai giả thuyết, sửa đường lưu trữ/tra cứu và chạy kiểm tra hồi quy. Không thay đổi yêu cầu nghiệp vụ: mã kiện duy nhất, tuyến có thể chỉnh sửa.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell, không cần database/network.

## Chạy nhanh
Tại thư mục unit: `./scripts/setup.ps1`; `./scripts/run.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` từ thư mục unit. Script yêu cầu quan sát một ca thất bại có kiểm soát.

## Những gì cần quan sát
So sánh số phần tử trước/sau, kết quả tra cứu bằng cùng tham chiếu và một khóa được dựng lại từ dữ liệu hiện tại. Chú ý khác biệt giữa thay đổi tuyến và thay đổi định danh kiện.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md).

## Expected Results
Starter: tái hiện lỗi lookup nhưng số lượng vẫn là 1. Sau fix: tìm được kiện bằng ID sau khi đổi tuyến, tuyến mới đúng, không tạo bản ghi trùng.

## Estimated Time
35–45 phút.
