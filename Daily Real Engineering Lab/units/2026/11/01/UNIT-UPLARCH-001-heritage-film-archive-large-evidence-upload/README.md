# UNIT-UPLARCH-001 — Cổng nộp phim tư liệu gặp lỗi khi nhận tệp lớn

## Mục tiêu
Đưa ra quyết định kiến trúc upload tệp lớn cho ASP.NET Core và Azure Blob Storage dựa trên bằng chứng định lượng, an toàn dữ liệu và vận hành. Đây là **L5 Design Decision / D4**: có thể có nhiều phương án hợp lý nếu chứng minh được các ràng buộc.

## Bối cảnh thực tế
Kho lưu trữ phim tư liệu tiếp nhận tệp gốc từ các trung tâm bảo tồn. Khi nhiều đối tác nộp đồng thời, một số phiên thất bại, khách hàng phải tải lại từ đầu và tài liệu chưa kiểm duyệt đôi khi xuất hiện trong danh sách xem trước. Nhóm chỉ có ngân sách vận hành hạn chế; mọi tệp được lưu giữ 7 năm.

## Bạn cần làm gì
1. Chạy mô hình hiện tại và ghi lại bằng chứng: memory, thời gian hoàn tất, số instance và các control còn thiếu.
2. Đề xuất ít nhất hai phương án có thể triển khai; chọn một và giải thích phương án bị loại.
3. Chỉnh sửa **workspace/architecture.json**, mô tả cơ chế resume, xác minh integrity, quyền truy cập, quét malware, audit và cleanup.
4. Chạy kiểm tra định lượng, sau đó tự review các giả định và trade-off. Script không thể thay thế security review hoặc load test thực.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7+; không cần Azure subscription, Docker, .NET SDK hoặc dịch vụ trả phí. Các con số là mô hình capacity giả định, không phải benchmark production.

## Chạy nhanh
Từ thư mục unit:

    powershell -NoProfile -ExecutionPolicy Bypass -File ./setup.ps1
    powershell -NoProfile -ExecutionPolicy Bypass -File ./reproduce.ps1
    powershell -NoProfile -ExecutionPolicy Bypass -File ./verify.ps1

Nếu dùng PowerShell 7, thay powershell bằng pwsh.

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên cấu hình gốc. Đọc evidence/incident-metrics.csv và starter/constraints.json. Không sửa starter; chỉ chỉnh workspace/architecture.json.

## Những gì cần quan sát
So sánh ngân sách memory theo instance, thời gian hoàn tất 16 phiên đồng thời, chi phí instance, trạng thái upload dang dở và kiểm soát publish. Phân biệt giới hạn capacity với thiếu sót trong lifecycle/security.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix / quyết định.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi tự làm và verify.

## Expected Results
- [Before](expected-results/before.md): mô hình gốc không đạt một số ràng buộc.
- [After](expected-results/after.md): một hoặc nhiều phương án có thể đạt capacity và các control; cần review kiến trúc bằng lập luận.

## Estimated Time
75–100 phút. Ghi kết quả vào [my-investigation.md](workspace/my-investigation.md).
