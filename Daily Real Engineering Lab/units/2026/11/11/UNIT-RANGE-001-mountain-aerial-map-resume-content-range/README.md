# UNIT-RANGE-001 — Bản đồ cứu hộ tải tiếp bị sai dữ liệu

## Mục tiêu
Điều tra lỗi integrity khi ứng dụng .NET tiếp tục tải bản đồ địa hình sau mất kết nối.

## Bối cảnh thực tế
Máy tính bảng của đội cứu hộ giữ sẵn phần đầu của một gói bản đồ 12 byte. Khi kết nối trở lại, dịch vụ trả về HTTP 206 nhưng file ghép không khớp bản gốc. Dashboard vẫn báo hoàn thành.

## Bạn cần làm gì
Chạy kịch bản, ghi bằng chứng offset, status và số byte; đưa ra ít nhất hai giả thuyết; sửa code trong `starter/` mà không thay đổi hợp đồng dữ liệu; kiểm tra cả luồng hợp lệ và luồng bất thường.

## Yêu cầu môi trường
.NET SDK 8.x; Windows PowerShell 5.1+ hoặc PowerShell 7. Không cần mạng, Docker hoặc dịch vụ trả phí.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./scripts/reproduce.ps1`; ghi lại giá trị byte của bản đồ sau ghép và kết quả đối chiếu.

## Những gì cần quan sát
Quan sát response code, header, số byte có sẵn trước khi tải và vị trí bắt đầu payload nhận về. Lưu kết quả vào `workspace/my-investigation.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem sau khi tự điều tra](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45–60 phút (L2 / D4).
