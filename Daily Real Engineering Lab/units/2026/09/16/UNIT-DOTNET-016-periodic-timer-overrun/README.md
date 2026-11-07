# UNIT-DOTNET-016 — Periodic work falls behind its schedule

## Mục tiêu
Điều tra lifecycle của một periodic background worker khi mỗi lần xử lý đôi lúc lâu hơn interval được cấu hình.

## Bối cảnh thực tế
Một worker đồng bộ dữ liệu chạy theo chu kỳ. Khi downstream chậm, dashboard cho thấy các lần chạy sau không còn giữ được khoảng cách vận hành mà team kỳ vọng, và shutdown đôi lúc phải chờ lâu hơn dự kiến.

## Bạn cần làm gì
Chạy starter, thu thập timestamp, viết hypothesis về scheduling semantics, sau đó sửa worker để contract về cadence và cancellation rõ ràng, rồi verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Script chạy worker với workload có thời lượng vượt interval ở một số iteration và kiểm tra timeline.

## Những gì cần quan sát
Quan sát khoảng cách giữa thời điểm bắt đầu/kết thúc từng iteration và hành vi khi cancellation xảy ra.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Sau đó mới xem solution.

## Hints
Xem `hints.md`.

## Reference Solution
Xem `solution/README.md` sau khi đã thử.

## Expected Results
Starter thể hiện cadence không khớp contract vận hành. Fixed state giữ contract đã chọn và dừng có kiểm soát.

## Estimated Time
35–50 phút.