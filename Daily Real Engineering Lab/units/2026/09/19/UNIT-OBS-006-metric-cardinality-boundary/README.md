# UNIT-OBS-006 — Metric Series Growth Under Normal Traffic

## Mục tiêu
Điều tra vì sao chi phí và số lượng time series của telemetry tăng mạnh dù số endpoint và lưu lượng nghiệp vụ vẫn nằm trong mức dự kiến.

## Bối cảnh thực tế
Một multi-tenant order API vừa bổ sung metric cho request. Sau rollout, dashboard latency vẫn đúng và ứng dụng không lỗi, nhưng số time series tăng gần tuyến tính theo số request. Nhóm vận hành lo ngại telemetry backend sẽ vượt quota khi traffic tăng.

## Bạn cần làm gì
Reproduce hiện tượng, thu thập evidence về số series được tạo, lập hypothesis, sửa starter để metric vẫn hữu ích cho vận hành nhưng số series được giữ trong một giới hạn hợp lý, rồi verify.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Simulator tạo một workload cố định và báo số metric series quan sát được. Script thành công khi triệu chứng tăng series được tái hiện.

## Những gì cần quan sát
- Tổng request không đổi giữa các lần chạy.
- Số route nghiệp vụ là hữu hạn.
- Số series lại lớn bất thường so với số nhóm vận hành cần phân biệt.
- Request vẫn trả kết quả đúng; đây là vấn đề telemetry, không phải functional correctness.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, workload nhỏ tạo số series gần với số request. Sau fix, `verify.ps1` phải báo `VERIFY_PASS`, metric vẫn phân biệt được route và outcome cần thiết nhưng series count bị chặn ở mức thấp.

## Estimated Time
50 phút.
