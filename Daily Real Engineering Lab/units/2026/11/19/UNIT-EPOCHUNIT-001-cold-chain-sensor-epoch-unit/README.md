# UNIT-EPOCHUNIT-001 — Gateway nhiệt độ báo sai tuổi dữ liệu

## Mục tiêu
Điều tra lỗi interoperability giữa hai nguồn timestamp và thiết kế regression contract cho luồng cảnh báo.

## Bối cảnh thực tế
Kho vaccine nhận dữ liệu nhiệt độ từ gateway A (Unix milliseconds) và gateway B (Unix seconds). Một số cảnh báo bị đánh dấu là quá cũ hoặc có ngày ngoài phạm vi vận hành. Người vận hành không thể tái hiện từ timestamp hiển thị trên dashboard.

## Bạn cần làm gì
Tái hiện kết quả, lập bảng bằng chứng cho từng gateway, đưa ít nhất hai giả thuyết, sửa code trong starter và chạy verification.

## Yêu cầu môi trường
.NET SDK 8, PowerShell, không cần database hoặc cloud.

## Chạy nhanh
`dotnet run --project starter/Telemetry.csproj`

## Cách reproduce vấn đề
`./reproduce.ps1` (hoặc `powershell -ExecutionPolicy Bypass -File ./reproduce.ps1`).

## Những gì cần quan sát
So sánh trạng thái của A-17, B-22, A-18; ghi nhận raw epoch, thời điểm được diễn giải và kết quả phân loại. Cảnh báo B-22 là phép đo mới nhưng đang bị đánh giá sai.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước: A-17 ACCEPT, B-22 STALE, A-18 FUTURE. Sau: A-17 ACCEPT, B-22 ACCEPT, A-18 FUTURE.

## Estimated Time
45 phút.
