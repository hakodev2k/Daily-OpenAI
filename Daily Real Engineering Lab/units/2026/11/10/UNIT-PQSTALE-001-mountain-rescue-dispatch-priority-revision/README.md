# UNIT-PQSTALE-001 — Lệnh cứu hộ khẩn cấp bị xử lý sau yêu cầu thường

## Mục tiêu
Điều tra sai lệch thứ tự dispatch trong một worker .NET, thu thập bằng chứng, đề xuất và kiểm chứng chính sách ưu tiên khi mức độ khẩn cấp thay đổi.

## Bối cảnh thực tế
Trung tâm điều phối trực thăng cứu hộ miền núi nhận yêu cầu với severity 1 (khẩn cấp nhất) đến 9 (ít khẩn cấp). Một ca được nâng mức ưu tiên trước khi điều phối, nhưng dashboard ghi nhận ca ít khẩn cấp hơn được dispatch trước. API cập nhật trả về thành công.

## Bạn cần làm gì
1. Chạy kịch bản và lưu trace gồm incident ID, severity tại thời điểm cập nhật và thứ tự dispatch.
2. Viết ít nhất hai giả thuyết; phân biệt vấn đề cập nhật trạng thái với vấn đề lựa chọn công việc.
3. Chỉnh sửa `starter/DispatchQueue.cs` để đáp ứng contract trong `expected-results/contract.md`.
4. Chạy `verify.ps1`; giải thích trade-off về bộ nhớ và fairness.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7+; không cần cloud, database hoặc NuGet ngoài SDK.

## Chạy nhanh
`pwsh ./run.ps1`

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` (exit 0 chỉ khi triệu chứng được quan sát đúng).

## Những gì cần quan sát
Trace có thể hiện severity hiện tại đã được cập nhật; hãy đối chiếu với thứ tự dispatch. Kiểm tra cả trường hợp không thay đổi mức độ và nhiều lần cập nhật.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify bằng `verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
[Xem contract](expected-results/contract.md). Không yêu cầu timing phụ thuộc máy.

## Estimated Time
60–75 phút (L3 / D4).
