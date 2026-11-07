# UNIT-GIT-001 — Cổng cấp phép: CI không xác định được file thay đổi của PR

## Mục tiêu

Điều tra một change-detection gate trong CI, phân biệt dữ liệu Git thực sự có trong checkout với phạm vi thay đổi mà pipeline cần kiểm tra. Đưa ra bản sửa có thể kiểm chứng bằng test hành vi.

## Bối cảnh thực tế

Nhóm backend .NET vận hành cổng cấp phép xây dựng. Pipeline chỉ chạy kiểm thử nghiệp vụ nếu PR chạm vào mã xử lý quy tắc hoặc tài liệu liên quan. Trên máy phát triển, change gate hoạt động; trên runner mới, gate trả lỗi trước khi chọn được test. PR thử nghiệm thay đổi đúng hai file, không phải toàn bộ repository.

## Bạn cần làm gì

1. Tái hiện lỗi với fixture Git cục bộ, không cần GitHub token hay network.
2. Thu thập trạng thái checkout, commit graph và output của change gate.
3. Viết ít nhất hai hypothesis, xác định assumption sai.
4. Sửa file learner-editable tại starter/detect-changes.ps1, không sửa fixture và không sửa baseline.
5. Chạy verify.ps1; chỉ xem solution sau khi tự thử.

## Yêu cầu môi trường

- Windows PowerShell 5.1+ hoặc PowerShell 7+
- Git CLI 2.28+ có trong PATH
- Không cần Docker, Azure, NuGet hoặc dịch vụ trả phí
- Script setup chỉ tạo và xóa thư mục .lab-runtime bên trong unit

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/10/27/UNIT-GIT-001-permit-pr-shallow-history-change-gate"
./reproduce.ps1
# Chỉnh starter/detect-changes.ps1
./verify.ps1
~~~

Nếu PowerShell chặn script, dùng phiên shell được cấu hình cho local scripts theo chính sách máy; không cần thay đổi execution policy toàn hệ thống.

## Cách reproduce vấn đề

Chạy ./reproduce.ps1. Script tự tạo một remote Git cục bộ, nhánh main, nhánh PR giả lập và checkout cho runner. Đọc evidence/ci-output.md rồi đối chiếu với trạng thái thực tế trong .lab-runtime/shallow.

## Những gì cần quan sát

- Một checkout có thể đọc HEAD nhưng không tính được tập file thay đổi.
- Fixture có hai file thay đổi đã biết; gate hiện tại không xuất được tập này.
- Cần chứng minh kết quả không phụ thuộc vào thứ tự liệt kê hoặc thông báo lỗi của Git.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trên starter.
4. Verify với cả base hợp lệ và base không tồn tại.
5. Chỉ sau đó mới xem reference solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) và [reference script](solution/detect-changes.ps1) sau khi tự điều tra.

## Expected Results

Trước khi sửa: reproduce báo REPRODUCED, change gate lỗi. Sau khi sửa: verify báo PASS và tập thay đổi chính xác gồm docs/permit-rules.md cùng src/PermitEngine/Rules.cs; base SHA không hợp lệ phải bị từ chối.

## Estimated Time

50–60 phút, L2 Investigation / D4.
