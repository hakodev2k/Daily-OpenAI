# UNIT-PRIORITYFIFO-001 — Thứ tự phiếu bảo trì máy phát điện

## Mục tiêu
Điều tra contract thứ tự xử lý của hàng đợi có mức độ khẩn cấp.

## Bối cảnh thực tế
Bệnh viện huyện tiếp nhận các phiếu kiểm tra máy phát điện dự phòng. Severity nhỏ hơn phải được xử lý trước; các phiếu cùng severity phải tuân theo thứ tự đến. Hệ thống đôi khi xuất phiếu cùng severity sai thứ tự.

## Bạn cần làm gì
Reproduce, ghi evidence và hypothesis, sửa starter, chạy verify, so sánh reference.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần cloud, database hoặc secrets.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter nguyên bản. Script trả 0 khi xác nhận có lỗi dự kiến.

## Những gì cần quan sát
Đối chiếu Expected và Actual theo cả severity và sequence; ghi bằng chứng, không suy đoán từ tên API.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md)

## Expected Results
Trước sửa: reproduce báo symptom. Sau sửa: verify trả 0 trên starter đã chỉnh.

## Estimated Time
35–45 phút.
