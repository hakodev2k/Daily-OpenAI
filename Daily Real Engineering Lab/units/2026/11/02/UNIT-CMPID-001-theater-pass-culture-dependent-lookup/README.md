# UNIT-CMPID-001 — Vé nhà hát: cùng mã QR nhưng hai kiosk xác thực khác nhau

## Mục tiêu
Điều tra một lỗi xác thực mã vé phụ thuộc môi trường; xác định hợp đồng so sánh identifier, thu thập bằng chứng và chứng minh bản sửa ổn định trên hai cấu hình locale.

## Bối cảnh thực tế
Một nhà hát phát mã QR chứa pass code dạng ASCII. Hợp đồng nghiệp vụ cho phép người dùng nhập chữ hoa hoặc chữ thường ASCII tương đương, nhưng không chấp nhận ký tự Unicode trông gần giống. Hai kiosk chạy cùng phiên bản ứng dụng cho kết quả khác nhau với mã `INV-42`.

## Bạn cần làm gì
1. Chạy starter trên máy cá nhân, không kết nối hệ thống vé thật.
2. Reproduce và ghi lại ba tín hiệu: lookup chữ thường ASCII, lookup ký tự trông giống, và đăng ký trùng.
3. Nêu ít nhất hai giả thuyết về nguyên nhân khác biệt giữa hai kiosk.
4. Chỉ sửa `starter/AccessCodeRegistry.cs`; không sửa harness `starter/Program.cs`.
5. Chạy verify và chứng minh hợp đồng ổn định trên `en-US` và `tr-TR`.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell 5.1+ hoặc 7; không cần database, NuGet package ngoài, tài khoản cloud hoặc dữ liệu thật. Mô phỏng có tính quyết định trong runtime .NET với globalization data thông thường (không dùng globalization-invariant mode).

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Trên starter nguyên bản, `./reproduce.ps1` trả exit code 0 khi **triệu chứng được tái hiện**, không có nghĩa là đã sửa. `./verify.ps1` phải thất bại trước khi sửa và thành công sau khi sửa.

## Những gì cần quan sát
So sánh `asciiLookup`, `lookalikeLookup`, `duplicateAccepted` giữa `en-US` và `tr-TR`. Giải thích vì sao cùng dữ liệu mà kết quả thay đổi và ảnh hưởng tới vận hành tại cổng soát vé.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify hành vi, gồm cả case âm và case dương.
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md) · [Reference code](solution/AccessCodeRegistry.cs). Chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
40 phút · L1 Deterministic Bug Lab / D3.
