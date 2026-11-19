# UNIT-URIPATH-001 — Regional Dispatch API: request đi sai route

## Mục tiêu
Điều tra HTTP request-target bằng bằng chứng, phân biệt routing, credentials và cấu hình.

## Bối cảnh thực tế
Gateway của đối tác điều phối cứu hộ trả 404 ở hai khu vực dù API key vẫn hợp lệ. Một số môi trường trước đó hoạt động. Hãy tìm nguyên nhân thay vì retry mù quáng.

## Bạn cần làm gì
Reproduce, ghi ít nhất ba hypotheses trong workspace, quan sát URI/status/auth, sửa `starter/Program.cs`, verify, rồi mới mở reference.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell, không cần Azure hay package ngoài.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy starter gốc và trả exit code 0 khi phát hiện triệu chứng; nó không chứng minh bản sửa đúng.

## Những gì cần quan sát
So sánh `uri`, `expected`, `status`, `auth` trong JSON của từng khu vực. HTTP 404 chưa đủ để kết luận nguyên nhân.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md).

## Expected Results
Trước: request sai contract, status 404. Sau: cả hai tenant status 202, đúng path và query, giữ auth.

## Estimated Time
45 phút.
