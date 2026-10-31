# UNIT-GIT-002 — CI báo sai phạm vi thay đổi khi hai nhánh tách xa

## Mục tiêu

Điều tra change detection trong CI của một monorepo, xác định vì sao danh sách file liên quan Pull Request không khớp với thay đổi của feature branch, sửa script và chứng minh không làm mất thay đổi hợp lệ.

## Bối cảnh thực tế

Nhóm vận hành cổng đăng ký trợ cấp cộng đồng dùng script PowerShell để chọn project cần build/test. Một thay đổi nhỏ của module Enrollment khiến pipeline chọn thêm module mà nhóm khác vừa cập nhật trên `main`. Báo cáo impact review trở nên gây hiểu nhầm và CI mất thêm thời gian.

Fixture mô phỏng ba lịch sử nhánh; không truy cập GitHub thật và không sửa Git config toàn cục.

## Bạn cần làm gì

1. Chạy fixture và thu thập danh sách file mà script hiện tại trả về.
2. Vẽ commit graph, ghi ít nhất hai hypothesis trước khi mở hints.
3. Chỉnh **duy nhất** `starter/change-detection.ps1` để tính đúng phạm vi thay đổi của feature.
4. Chạy verification trên cả ba lịch sử, gồm trường hợp feature không có commit mới.
5. Đánh giá điều kiện áp dụng cho CI thật.

## Yêu cầu môi trường

- Git CLI 2.28+ trong PATH, PowerShell 5.1+ hoặc PowerShell 7+.
- Không cần .NET SDK, NuGet, Docker, cloud hay GitHub account.
- `setup.ps1` chỉ tạo/xóa `workspace/fixtures/` bên trong unit.

## Chạy nhanh

Mở PowerShell tại thư mục unit:

```powershell
.\setup.ps1
.\run.ps1
.\reproduce.ps1
```

Trên macOS/Linux với PowerShell 7: `pwsh -NoProfile -File ./reproduce.ps1`.

## Cách reproduce vấn đề

Chạy `.\reproduce.ps1` **trước khi chỉnh starter**. Script tái tạo fixture và xác nhận hiện tượng: một file chỉ sửa trên base vẫn xuất hiện trong impact list của feature.

## Những gì cần quan sát

- Kết quả case-a, case-b, case-c.
- Commit graph, branch tips và file đổi ở từng nhánh.
- File nào thuộc feature, file nào chỉ thuộc base.
- Sau khi chỉnh starter, `.\verify.ps1` phải PASS cả ba case.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng script riêng.
5. Chỉ sau đó xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra và sửa:** [Giải pháp tham chiếu](solution/README.md).

## Expected Results

- [Before](expected-results/before.md) · [After](expected-results/after.md)
- [My Investigation](workspace/my-investigation.md)

## Estimated Time

55 phút. Validation khi xuất bản: **static-review**; chưa thực thi Git/PowerShell trong môi trường phát hành.
