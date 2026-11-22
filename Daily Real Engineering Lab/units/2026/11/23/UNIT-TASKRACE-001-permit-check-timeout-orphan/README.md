# UNIT-TASKRACE-001 — Permit timeout nhưng tác vụ vẫn ghi trạng thái

## Mục tiêu
Điều tra tính nhất quán giữa HTTP deadline và công việc nền trong một dịch vụ xác minh giấy phép; phân biệt phản hồi timeout với trạng thái side effect.

## Bối cảnh thực tế
Cổng xác minh giấy phép công trình trả TIMEOUT trong khoảng 30 ms, nhưng audit thấy một số giấy phép vẫn chuyển sang APPROVED sau đó. Người dùng retry khiến lịch sử xử lý khó giải thích.

## Bạn cần làm gì
Reproduce, ghi tối thiểu hai giả thuyết, tìm bằng chứng về vòng đời công việc sau deadline, chỉnh mã trong `starter/Program.cs`, rồi verify. Giữ nguyên hành vi trả TIMEOUT và tránh side effect muộn.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần dịch vụ ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/reproduce.ps1
./scripts/verify.ps1
```
`reproduce.ps1` chỉ chứng minh lỗi ở starter gốc; `verify.ps1` kiểm tra mã bạn đang sửa.

## Cách reproduce vấn đề
Chạy reproduce với starter chưa sửa; đối chiếu `response`, `atDeadline`, `afterGrace` và kết quả REPRODUCED. Thử chạy lặp lại.

## Những gì cần quan sát
Tại thời điểm trả kết quả, có bao nhiêu write? Sau thời gian chờ thêm, số write thay đổi thế nào? Không coi một HTTP timeout là bằng chứng thao tác đã kết thúc.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [solution/README.md](solution/README.md) sau khi tự sửa.

## Expected Results
Trước: `TIMEOUT` và `afterGrace=1`; sau: `TIMEOUT` và `afterGrace=0`. Nếu không tái hiện, kiểm tra SDK, chạy từ thư mục unit và không sửa bản starter trước khi reproduce.

## Estimated Time
40–50 phút.
