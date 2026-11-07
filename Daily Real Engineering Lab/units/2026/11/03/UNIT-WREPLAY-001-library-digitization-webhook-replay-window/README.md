# UNIT-WREPLAY-001 — Webhook hợp lệ nhưng xử lý lại sự kiện số hóa

## Mục tiêu
Điều tra ranh giới xác thực và xử lý đúng một lần theo business event trong webhook.

## Bối cảnh thực tế
Hệ thống thư viện nhận webhook từ đối tác số hóa. Đối tác retry khi không nhận ACK; báo cáo cho thấy một số tài liệu được cập nhật nhiều lần, kể cả bản ghi cũ.

## Bạn cần làm gì
Chạy starter, ghi nhận ba quyết định, phân tích rủi ro, đề xuất chính sách chống replay và duplicate delivery. Không thay đổi payload giả lập để che lỗi.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell; không cần Internet hay credentials thật.

## Chạy nhanh
`pwsh ./run.ps1`

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` phải trả exit code khác 0 trên starter nguyên bản.

## Những gì cần quan sát
So sánh số lần side effect với số delivery hợp lệ; ghi nhận phản hồi với cùng delivery ID và timestamp cũ. Xác định điều gì được chứng minh bởi chữ ký và điều gì chưa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
Starter nhận cả ba yêu cầu và ghi side effect 3 lần; bản sửa phải chặn delivery trùng, timestamp hết hạn và vẫn nhận sự kiện mới.

## Estimated Time
45 phút.
