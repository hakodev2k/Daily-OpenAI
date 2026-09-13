# UNIT-ARCH-002 — Order đã commit nhưng integration event có thể biến mất

## Mục tiêu

Thiết kế boundary đáng tin cậy giữa thay đổi dữ liệu trong SQL và việc phát integration event, đồng thời giải thích trade-off giữa độ tin cậy, độ phức tạp vận hành và semantics giao nhận message.

## Bối cảnh thực tế

Một B2B ordering service lưu trạng thái đơn hàng vào SQL Server rồi phát `OrderApproved` sang message broker để Inventory và Billing xử lý tiếp. Bình thường hệ thống chạy ổn, nhưng sau một lần process restart giữa request, đội vận hành phát hiện có order ở trạng thái `Approved` mà downstream không hề nhận event tương ứng.

Không được thay business requirement bằng cách bỏ event hoặc chuyển toàn bộ workflow thành synchronous HTTP call.

## Bạn cần làm gì

1. Đọc scenario và incident evidence.
2. Xác định failure window và boundary hiện tại.
3. Đề xuất ít nhất 3 phương án kiến trúc có thể triển khai.
4. Chọn một phương án chính và ghi rõ assumptions, failure behavior, retry/idempotency strategy và operational cost.
5. Hoàn thành `workspace/decision-record.md` trước khi xem reference solution.

## Yêu cầu môi trường

Không cần service trả phí hay runtime bên ngoài. Đây là Design Decision Lab dựa trên timeline và constraints có sẵn trong repository.

## Chạy nhanh

Mở lần lượt:

```text
README.md
docs/scenario.md
evidence/incident-timeline.md
workspace/decision-record.md
```

## Cách reproduce vấn đề

Không có executable reproduction. Dùng incident timeline để reconstruct trình tự trạng thái trước và sau failure point, sau đó xác định state nào đã durable và state nào chưa có bằng chứng durable.

## Những gì cần quan sát

- Request đã trả success hay chưa tại từng mốc.
- Database state có tồn tại sau restart hay không.
- Broker có record tương ứng hay không.
- Retry của request có thể tạo side effect mới hay không.
- Phương án đề xuất xử lý duplicate và partial failure thế nào.

## Quy tắc làm lab

1. Reconstruct incident trước.
2. Ghi ít nhất 3 hypothesis/architecture option.
3. Chọn boundary và semantics mong muốn.
4. Tự review failure path của phương án đã chọn.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã hoàn thành decision record.

[Reference Solution](solution/README.md)

## Expected Results

Sau lab, bạn phải giải thích được vì sao một request có thể để lại database state hợp lệ nhưng không tạo được integration event đáng tin cậy, đồng thời bảo vệ được lựa chọn kiến trúc trước các câu hỏi về duplicate delivery, retry, recovery, monitoring và operational complexity.

## Estimated Time

Khoảng 50 phút.
