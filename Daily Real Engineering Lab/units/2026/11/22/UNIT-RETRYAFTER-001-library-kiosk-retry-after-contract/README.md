# UNIT-RETRYAFTER-001 — Retry-After của kiosk thư viện

## Mục tiêu
Điều tra sai lệch thời gian retry khi gateway trả HTTP 429, thu thập evidence và sửa hợp đồng client.

## Bối cảnh thực tế
Kiosk tự phục vụ của thư viện đồng bộ dữ liệu qua gateway giới hạn lưu lượng. Một số kiosk retry sớm hơn thời điểm server yêu cầu. Harness chạy offline với đồng hồ cố định.

## Bạn cần làm gì
Reproduce, ghi hai hypothesis, kiểm tra từng mẫu header, sửa `starter/Program.cs`, verify và giải thích trade-offs.

## Yêu cầu môi trường
.NET SDK 8, PowerShell. Không cần internet, database hoặc credential.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter chưa sửa. Exit code 0 nghĩa là đã quan sát sai lệch hợp đồng.

## Những gì cần quan sát
So sánh expected và actual delay, nhóm các case PASS/FAIL và ghi `CONTRACT_FAILURES`. Không suy diễn root cause trước khi kiểm tra evidence.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45 phút.
