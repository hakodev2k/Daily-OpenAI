# UNIT-XXE-001 — Equipment Return XML: Private Note Appears in a Partner Receipt

## Mục tiêu
Điều tra một sự cố nhập XML từ đối tác, thu thập bằng chứng về ranh giới tin cậy và xây dựng regression test cho dữ liệu không đáng tin cậy.

## Bối cảnh thực tế
Hệ thống quản lý cho thuê thiết bị xây dựng nhận phiếu trả thiết bị dạng XML từ nhà vận chuyển. Sau một lần tích hợp, trường ghi chú của một phiếu trả có nội dung nội bộ không do đối tác cung cấp. Bộ phận vận hành cần xác định nguồn dữ liệu trước khi tiếp tục nhận batch.

## Bạn cần làm gì
- Reproduce tình huống bằng canary **giả lập** được tạo trong thư mục tạm.
- Ghi lại output, phân biệt lỗi dữ liệu, logging và parser.
- Đưa ra ít nhất hai hypothesis và chọn phép thử phân biệt.
- Sửa **starter/ReceiptXmlParser.cs** để XML hợp lệ vẫn được xử lý nhưng input không tin cậy không thể truy cập tài nguyên ngoài phạm vi tài liệu.
- Chạy verification trên chính starter đã sửa, rồi so sánh reference solution.

## Yêu cầu môi trường
- Windows PowerShell 7+ hoặc Windows PowerShell 5.1.
- .NET SDK 10.0.100 (hoặc bản vá SDK 10.0 tương thích).
- Không cần database, cloud, mạng hay credentials.

## Chạy nhanh
Từ thư mục unit:

```powershell
.\setup.ps1
.\run.ps1
```

## Cách reproduce vấn đề
```powershell
.\reproduce.ps1
```
Harness chỉ tạo một file canary giả trong temp, không truy cập dữ liệu thật; file được xóa khi kết thúc.

## Những gì cần quan sát
- Dòng `VALID_RECEIPT` đối với XML hợp lệ.
- Dòng `EXTERNAL_ENTITY` đối với một phiếu XML bất thường.
- Sự khác biệt giữa nguồn XML do đối tác gửi và dữ liệu xuất hiện sau import.
- Dữ liệu trong [evidence/incident.md](evidence/incident.md) là bằng chứng **mô phỏng**, không phải log đã chạy trong môi trường này.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ xem [solution/README.md](solution/README.md) sau khi đã tự điều tra và thử sửa.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
65 phút. Dùng [workspace/my-investigation.md](workspace/my-investigation.md) để lưu bằng chứng và quyết định.
