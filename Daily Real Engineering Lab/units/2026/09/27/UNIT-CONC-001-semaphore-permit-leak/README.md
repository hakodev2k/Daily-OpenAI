# UNIT-CONC-001 — Worker dừng tiến triển sau một số lỗi cục bộ

## Mục tiêu
Điều tra một worker xử lý nhiều job với giới hạn concurrency, xác định vì sao throughput về 0 sau vài lỗi riêng lẻ, sửa mà vẫn giữ giới hạn concurrency và failure isolation.

## Bối cảnh thực tế
Một background worker giới hạn tối đa 2 job chạy đồng thời. Khi downstream thỉnh thoảng lỗi, log cho thấy một số job đã fail như dự kiến, nhưng các job phía sau không còn bắt đầu và batch không kết thúc.

## Bạn cần làm gì
1. Reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa.
3. Quan sát thứ tự acquire/start/finish/fail và số job đạt trạng thái terminal.
4. Sửa `starter/Program.cs` mà không bỏ giới hạn concurrency và không nuốt lỗi.
5. Chạy verify.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script thành công khi nó chứng minh starter không hoàn tất toàn bộ 8 job trong deadline.

## Những gì cần quan sát
- Bao nhiêu job đã bắt đầu?
- Bao nhiêu job đã kết thúc hoặc fail?
- Tại thời điểm mất tiến triển, còn job nào đang chờ?
- CPU không cần cao dù batch không hoàn tất.

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
⚠️ Chỉ xem sau khi đã tự điều tra và thử fix: [solution/README.md](solution/README.md)

## Expected Results
Starter: reproduce xác nhận batch mất tiến triển trước khi đủ 8 job đạt terminal state.

Sau fix: `./verify.ps1` phải in `VERIFY:PASS`, cả 8 job đạt terminal state, vẫn không vượt quá 2 job đồng thời.

## Estimated Time
30–45 phút.
