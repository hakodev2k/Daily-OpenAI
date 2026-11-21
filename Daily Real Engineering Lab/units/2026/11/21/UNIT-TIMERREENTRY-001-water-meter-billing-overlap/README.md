# UNIT-TIMERREENTRY-001 — Trùng chu kỳ đối soát đồng hồ nước

## Mục tiêu
Điều tra chứng từ bị ghi hai lần khi hệ thống scheduled billing bị chậm.

## Bối cảnh thực tế
Worker gọi gateway định kỳ để đối soát đồng hồ nước. Khi gateway chậm, cùng một batch phát sinh nhiều chứng từ.

## Bạn cần làm gì
Reproduce, thu thập evidence, ghi hai hypothesis, sửa `starter/Program.cs` và verify; đánh giá giới hạn khi chạy nhiều replicas.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần network, database hoặc secret.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script trả exit 0 khi tái hiện lỗi; tiến trình starter ban đầu trả exit 2.

## Những gì cần quan sát
Ghi `MAX_ACTIVE`, `POSTED`, `CYCLES`, exit code và timeline trước/sau khi gateway được giải phóng. Đối chiếu kỳ vọng mỗi batch chỉ có một chứng từ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md), [2](hints/hint-02.md), [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
[Before](expected-results/before.md), [After](expected-results/after.md)

## Estimated Time
45 phút.
