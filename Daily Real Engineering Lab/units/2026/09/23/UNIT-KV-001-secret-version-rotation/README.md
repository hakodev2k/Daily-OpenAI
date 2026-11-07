# UNIT-KV-001 — Key Vault Secret Rotation Boundary

## Mục tiêu
Điều tra một sự cố sau credential rotation: Azure Key Vault đã có secret version mới nhưng một ASP.NET Core API đang chạy vẫn bị downstream trả 401.

## Bối cảnh thực tế
Payment team rotate credential lúc 22:00. Deployment không đổi. Version mới được tạo thành công. Instance restart sau đó hoạt động bình thường, còn instance chạy liên tục tiếp tục nhận 401.

## Bạn cần làm gì
1. Đọc scenario.
2. Ghi ít nhất ba hypothesis vào workspace.
3. Xác định observation nào phân biệt configuration-loading issue với downstream outage hoặc network issue.
4. Đề xuất fix và refresh lifecycle phù hợp.
5. Chỉ sau đó mới mở hints và reference solution.

## Yêu cầu môi trường
Không cần Azure subscription. Đây là investigation lab dựa trên incident snapshot đã capture.

## Chạy nhanh
Mở `docs/scenario.md`, rồi ghi reasoning vào `workspace/my-investigation.md`.

## Cách reproduce vấn đề
Dùng timeline và hai instance outcomes trong scenario để tái hiện reasoning path của sự cố. Không có live cloud dependency trong lab này.

## Những gì cần quan sát
- Instance nào fail và instance nào recover.
- Thời điểm version thay đổi so với process lifetime.
- Restart thay đổi outcome trong khi network path không đổi.

## Quy tắc làm lab
1. Reproduce reasoning trước.
2. Ghi hypothesis.
3. Đề xuất fix.
4. Verify bằng test plan của bạn.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md)

## Expected Results
Bạn phải giải thích được vì sao restart thay đổi outcome, version mới tồn tại nhưng process cũ vẫn fail, và cách thiết kế rotation/refresh mà không phụ thuộc restart thủ công.

## Estimated Time
45 phút.