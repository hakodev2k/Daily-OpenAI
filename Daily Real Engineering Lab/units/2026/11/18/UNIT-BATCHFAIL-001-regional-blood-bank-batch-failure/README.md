# UNIT-BATCHFAIL-001 — Đối soát upload báo cáo ngân hàng máu

## Mục tiêu
Điều tra tính đúng đắn của báo cáo batch khi partner trả về kết quả hỗn hợp; cải thiện khả năng đối soát mà không upload lại các bản ghi đã được chấp nhận.

## Bối cảnh thực tế
Hệ thống chuyển bốn báo cáo vận chuyển máu sang partner. Dashboard đôi khi ghi nhận không có báo cáo thành công dù partner đã nhận một phần dữ liệu. Hệ thống đang thử retry toàn batch, có nguy cơ gửi trùng.

## Bạn cần làm gì
Reproduce, lưu output, lập ít nhất hai hypothesis, chỉnh sửa `starter/Program.cs`, chạy verify, giải thích retry boundary và observability.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần network, DB hoặc credentials.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter nguyên bản. Script kỳ vọng phát hiện sai lệch giữa báo cáo tổng và các kết quả thực tế đã hoàn thành. Ghi lại mã thoát và console output.

## Những gì cần quan sát
So sánh số lượng ACCEPT/REJECT, danh sách ID, tổng số input, và khả năng đối soát từng ID. Không suy luận thành công chỉ từ exit code của process.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45 phút.
