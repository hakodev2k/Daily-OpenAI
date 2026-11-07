# UNIT-LEGAL-001 — Biên nhận nộp hồ sơ tòa án tại thời điểm chốt hạn

## Mục tiêu

Thực hiện một **architecture tabletop lab (L6 / D4)**: từ timeline lỗi và ràng buộc nghiệp vụ, thiết kế đường đi tạo biên nhận có thể kiểm chứng, đánh giá trade-off giữa latency, availability và tính toàn vẹn dữ liệu. Sản phẩm là một Architecture Decision Record (ADR) và failure-test matrix, không phải bài hỏi đáp.

## Bối cảnh thực tế

Cổng e-filing của tòa án nhận hồ sơ sát hạn 17:00. Một số người nộp nhìn thấy biên nhận hợp lệ nhưng bộ phận thụ lý không tìm thấy hồ sơ sau sự cố hạ tầng; một số người khác retry và nhận hai mã hồ sơ. Chưa thể kết luận lỗi từ dashboard HTTP 2xx.

## Bạn cần làm gì

1. Đọc [quy định và constraints](docs/scenario.md), sau đó xem [timeline](evidence/incident-timeline.csv) và [telemetry](evidence/operational-metrics.md).
2. Ghi ít nhất ba hypothesis có bằng chứng ủng hộ/phản bác vào [workspace](workspace/my-architecture.md).
3. Vẽ state machine, request/commit/receipt sequence, data ownership, failover và recovery.
4. So sánh tối thiểu hai phương án; viết ADR, failure-test matrix và SLO.
5. Tự đánh giá bằng [acceptance contract](expected-results/acceptance.md); chỉ sau đó xem reference.

## Yêu cầu môi trường

Chỉ cần Git, trình đọc Markdown và công cụ vẽ sơ đồ bất kỳ. **Không cần .NET SDK, database, cloud account hay dịch vụ trả phí.** Đây là bài thực hành thiết kế dựa trên evidence, không có executable starter.

## Chạy nhanh

```powershell
git pull
cd "Daily Real Engineering Lab/units/2026/10/27/UNIT-LEGAL-001-court-efiling-durable-deadline-receipts"
Get-Content docs/scenario.md
Import-Csv evidence/incident-timeline.csv | Format-Table -AutoSize
```

Mở `workspace/my-architecture.md` để điền quyết định; không sửa evidence.

## Cách reproduce vấn đề

Dùng timeline để tái dựng từng chuỗi A–E: xác định thời điểm request, phản hồi, durable record, failover và retry. Đối chiếu ID biên nhận với record được lưu; đánh dấu trường hợp hệ thống phát biểu điều mà evidence không chứng minh. Đây là **manual incident replay**, không giả vờ có runtime test.

## Những gì cần quan sát

- Chênh lệch giữa HTTP success, record có thể tra cứu và bằng chứng nộp đúng hạn.
- Kết quả retry sau timeout không rõ trạng thái.
- Khác biệt giữa tiếp nhận hồ sơ và quyết định hồ sơ hợp lệ.
- Hành vi khi một failure domain mất khả dụng.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix / thiết kế.
4. Verify theo checklist.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Một kiến trúc có thể bảo vệ bằng evidence](solution/README.md). Không phải đáp án duy nhất.

## Expected Results

[Acceptance contract và các test case](expected-results/acceptance.md). Sau đó thử [requirement change](extensions/change-request.md).

## Estimated Time

80–100 phút (thiết kế 55, failure tests 20, đối chiếu 15).
