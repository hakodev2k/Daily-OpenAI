# UNIT-SER-002 — Audit phê duyệt mất dữ liệu khi lưu JSON

## Mục tiêu
Tái hiện sai lệch giữa event trong bộ nhớ và bản audit persisted, tìm ranh giới gây mất dữ liệu, sửa contract, và xác minh replay an toàn.

## Bối cảnh thực tế
Một dịch vụ kiểm duyệt hồ sơ tuân thủ lưu các event Approved/Rejected thành JSON. Dashboard trong bộ nhớ đúng nhưng báo cáo audit đọc lại từ kho lưu trữ thiếu người duyệt và lý do từ chối. Lab mô phỏng bước lưu trữ bằng JSON trong bộ nhớ, không cần dịch vụ ngoài.

## Bạn cần làm gì
1. Chạy starter, ghi lại hai event đầu vào và payload JSON.
2. Reproduce triệu chứng, đưa ra ít nhất hai hypothesis.
3. Điều tra kiểu dữ liệu được đưa qua serialization boundary và contract replay.
4. Chỉ sửa starter/ApprovalEvents.cs; không sửa Program.cs, scripts, solution.
5. Verify wire shape, round-trip subtype, và unknown event policy.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell 5.1/7+. Không cần database, Docker, Azure hoặc NuGet ngoài.

## Chạy nhanh
    ./setup.ps1
    ./run.ps1

## Cách reproduce vấn đề
    ./reproduce.ps1

Exit code 0 nghĩa là triệu chứng ban đầu được tái hiện. Sau khi sửa, chạy:

    ./verify.ps1

Verify chạy learner-editable starter, không chạy reference solution.

## Những gì cần quan sát
- Trường nào tồn tại trong object nhưng vắng mặt trong persisted JSON?
- Event ID và số lượng event có bị mất không?
- Payload có đủ thông tin để replay đúng event subtype không?
- Một loại event không được hỗ trợ cần được xử lý ra sao?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md) · [Investigation workspace](workspace/my-investigation.md)

## Estimated Time
45–60 phút.
