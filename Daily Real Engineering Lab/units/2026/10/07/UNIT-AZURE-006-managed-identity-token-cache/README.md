# UNIT-AZURE-006 — Managed Identity Token Cache Boundary

## Mục tiêu
Điều tra một service gọi downstream bằng access token, chạy ổn lúc đầu nhưng bắt đầu nhận 401 sau thời gian dài.

## Bối cảnh thực tế
Một worker lấy token từ credential provider và gọi API nội bộ. Deploy mới luôn chạy tốt; lỗi chỉ xuất hiện sau khi process sống đủ lâu.

## Bạn cần làm gì
1. Chạy starter để reproduce timeline.
2. Ghi hypothesis dựa trên timestamp/token evidence.
3. Sửa code trong `starter/`.
4. Chạy `verify.ps1`.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 7+.

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
`./reproduce.ps1`

## Những gì cần quan sát
Quan sát token được dùng ở từng request, thời điểm hiện tại mô phỏng và status trả về. Không giả định downstream outage nếu evidence không hỗ trợ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45–60 phút.
