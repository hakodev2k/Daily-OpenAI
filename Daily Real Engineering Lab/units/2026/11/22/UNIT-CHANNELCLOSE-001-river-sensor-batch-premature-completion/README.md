# UNIT-CHANNELCLOSE-001 — Trạm quan trắc sông mất bản ghi khi đóng batch

## Mục tiêu
Điều tra vòng đời producer/consumer bất đồng bộ trong .NET; chứng minh tính toàn vẹn của dữ liệu bằng kiểm thử có kiểm soát.

## Bối cảnh thực tế
Một gateway tổng hợp dữ liệu từ ba trạm quan trắc sông mỗi phút. Khi hệ thống chạy ở chế độ thu thập nhanh, dashboard nhận ít bản ghi hơn số lượng trạm đã xác nhận gửi. Không có exception rõ ràng trong log tổng hợp.

## Bạn cần làm gì
Chạy starter, ghi lại số bản ghi dự kiến và thực nhận, đưa ra ít nhất hai giả thuyết, sửa đường xử lý trong `starter/Program.cs`, rồi chạy verification. Không thay đổi bộ test hoặc bỏ qua bản ghi.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ (hoặc chạy `dotnet run --project starter -- --verify` trực tiếp).

## Chạy nhanh
`pwsh ./reproduce.ps1`

## Cách reproduce vấn đề
Chạy `pwsh ./reproduce.ps1`; lưu lại `expected`, `received`, `missing` và `producerErrors`. Chạy lại để kiểm tra tính ổn định.

## Những gì cần quan sát
Các trạm đều có cùng số lượng bản tin đầu vào, nhưng số bản tin đến đích không khớp. Phân biệt số producer hoàn tất, thời điểm reader kết thúc và dữ liệu đã thực sự ghi thành công.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** chỉ mở [lời giải](solution/README.md) sau khi đã tự sửa.

## Expected Results
Starter: thiếu bản ghi hoặc báo lỗi producer. Sau fix: `received=expected=12`, không trùng ID, không có producer error. Kết quả không phụ thuộc timing máy.

## Estimated Time
45–60 phút.
