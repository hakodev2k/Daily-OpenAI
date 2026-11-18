# UNIT-PAGECURSOR-001 — Trang cảnh báo chất lượng nước bị thiếu và trùng bản ghi

## Mục tiêu
Điều tra một luồng đọc dữ liệu phân trang khi có bản ghi mới xuất hiện giữa hai lần lấy trang; chứng minh kết quả bằng trace và test, sau đó đề xuất contract phân trang ổn định.

## Bối cảnh thực tế
Dashboard của trung tâm giám sát chất lượng nước lấy từng trang 3 cảnh báo từ API. Nhân viên nhận thấy một số cảnh báo xuất hiện hai lần, trong khi một số cảnh báo không được xem xét. Sự cố dễ tái hiện khi thiết bị mới gửi dữ liệu giữa hai request. API vẫn trả HTTP 200 và tổng số bản ghi hợp lệ.

## Bạn cần làm gì
1. Chạy mô phỏng, ghi lại các trang và ID.
2. Xác định các giả thuyết cạnh tranh (sort, cập nhật dữ liệu, retry, filter).
3. Thiết kế lại contract để mỗi phiên duyệt có thứ tự xác định và điểm tiếp tục rõ ràng.
4. Chứng minh không trùng, không bỏ sót bản ghi thuộc tập dữ liệu đã được chốt theo yêu cầu.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần Docker, database hoặc dịch vụ trả phí.

## Chạy nhanh
Từ thư mục unit:
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy chương trình starter với một chuỗi request có bản ghi mới chen vào giữa hai trang. Ghi lại ID theo từng trang và so sánh với tập cảnh báo ban đầu.

## Những gì cần quan sát
- Có ID lặp giữa hai trang không?
- Có cảnh báo nào thuộc tập ban đầu không bao giờ được trả về không?
- Thứ tự bản ghi khi cùng thời điểm có nhất quán không?
- Có sự khác nhau giữa số bản ghi ban đầu và số ID duy nhất được xử lý không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-1.md) · [Hint 2](hints/hint-2.md) · [Hint 3](hints/hint-3.md)

## Reference Solution
**Spoiler:** Chỉ đọc sau khi tự điều tra: [Lời giải](solution/README.md).

## Expected Results
Starter: ít nhất một ID lặp hoặc thiếu trong chuỗi phân trang. Sau khi sửa: tất cả cảnh báo thuộc tập ban đầu xuất hiện đúng một lần, theo thứ tự xác định; bản ghi mới không làm lệch phiên duyệt.

## Estimated Time
55–75 phút (L3 / D4).
