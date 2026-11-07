# UNIT-MCARD-001 — Cold-chain telemetry: điều tra metric series vượt ngân sách

## Mục tiêu
Điều tra vì sao telemetry của hệ thống kho lạnh trở nên đắt đỏ và khó truy vấn khi lưu lượng tăng; đưa ra thay đổi có thể kiểm chứng mà vẫn giữ khả năng vận hành và điều tra từng sự kiện.

## Bối cảnh thực tế
Một nền tảng giám sát chuỗi lạnh phục vụ các kho thực phẩm. Sau đợt mở rộng thiết bị, dashboard theo depot đôi lúc thiếu dữ liệu và chi phí metric tăng mạnh. Đội vận hành vẫn phải phân biệt cảnh báo theo vùng, kho và kết quả đánh giá, đồng thời truy vết được một cảm biến cụ thể từ log khi có sự cố.

## Bạn cần làm gì
1. Chạy bản starter và ghi lại số lượng active series, số event và các nhóm vận hành.
2. Tạo ít nhất hai giả thuyết, kiểm tra chính sách telemetry trong starter.
3. Chỉnh sửa **starter/metric-policy.json**; không sửa test hoặc script để làm bài pass.
4. Chạy verify, giải thích trade-off giữa metric aggregation và forensic detail.
5. Đối chiếu reference solution sau khi đã thử.

## Yêu cầu môi trường
PowerShell 7+ trên Windows/macOS/Linux; không cần .NET SDK, cloud account hoặc package ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
pwsh -NoProfile -File ./run.ps1
pwsh -NoProfile -File ./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `reproduce.ps1`; script phải in **REPRODUCED** và chỉ ra rằng số series vượt ngân sách local **36**. Chạy lại để so sánh kết quả có ổn định không.

## Những gì cần quan sát
- Event count và active series có quan hệ như thế nào?
- Các chiều phân tích nào vẫn cần thiết cho dashboard của on-call?
- Việc điều tra theo sensor/request cần dữ liệu nào và trong hệ thống nào?
- Đọc `evidence/incident.md` để xem điều kiện production và nhiễu điều tra.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify bằng `verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
- Starter: reproduction cho thấy series vượt 36, trong khi số event không thay đổi giữa các lần chạy.
- Sau fix: `verify.ps1` PASS, dashboard vẫn phân tích được region/depot/outcome; số series trong fixture nằm trong budget.
- Không dùng thời gian chạy hoặc kết quả production làm điều kiện PASS vì phụ thuộc môi trường.

## Estimated Time
55 phút.
