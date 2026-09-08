# UNIT-HTTP-002 — Retry vượt quá latency budget của request

## Mục tiêu
Điều tra một lỗi resilience trong integration HTTP: mỗi attempt đều có timeout riêng, nhưng tổng thời gian của request vẫn vượt xa latency budget mà business cho phép.

## Bối cảnh
Một service gọi Tax API. Team cấu hình tối đa 3 attempts, mỗi attempt timeout 300 ms. Business yêu cầu toàn bộ dependency call phải kết thúc trong 500 ms. Khi downstream chậm, request thực tế kéo dài gần 900 ms trước khi fail.

## Nhiệm vụ
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses trước khi đọc hints.
3. Phân biệt `per-attempt timeout` và `overall deadline`.
4. Sửa `starter/Program.cs` để toàn bộ operation tuân thủ 500 ms budget.
5. Giữ retry bounded và không swallow cancellation.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

## Chạy
```powershell
./run.ps1
```

## Reproduce
```powershell
./reproduce.ps1
```

## Quan sát
- Số attempts thực sự chạy.
- Thời gian elapsed toàn operation.
- Mỗi timeout có reset lại budget hay không.
- Cancellation nào đại diện cho deadline tổng thể.

## Expected behavior
Operation phải dừng trong khoảng latency budget tổng thể, không chỉ timeout từng attempt.

## Actual behavior
Starter reset timeout cho từng attempt, nên retry kéo tổng latency vượt budget.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference solution
Chỉ xem sau khi tự sửa: `solution/README.md`.

## Estimated time
30–45 phút.
