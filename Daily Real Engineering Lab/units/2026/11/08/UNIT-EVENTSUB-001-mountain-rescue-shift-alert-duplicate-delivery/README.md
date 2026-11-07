# UNIT-EVENTSUB-001 — Cảnh báo cứu hộ bị gửi trùng sau bàn giao ca

## Mục tiêu
Điều tra sự cố notification trong tiến trình .NET chạy lâu, thu thập bằng chứng theo từng lần bàn giao ca và chứng minh tính đúng đắn sau sửa.

## Bối cảnh thực tế
Trạm cứu hộ vùng núi có một tiến trình phát cảnh báo thời tiết. Mỗi ca trực chỉ một nhóm trực hiện tại được nhận một bản tin. Sau nhiều lần bàn giao, log delivery tăng bất thường dù nguồn chỉ phát một sự kiện mỗi lần.

## Bạn cần làm gì
Chạy starter, ghi nhận số delivery, nêu ít nhất hai giả thuyết (ví dụ nguồn phát trùng hoặc vòng đời consumer), tìm bằng chứng phân biệt, sửa code trong `starter/Program.cs`, chạy verify và đối chiếu reference.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell, không cần dịch vụ bên ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy ba lần bàn giao ca A → B → C và một bản tin mỗi ca. Script thành công khi quan sát được sai lệch ở starter ban đầu.

## Những gì cần quan sát
So sánh chuỗi Expected/Actual, số delivery và danh tính ca trực nhận tin. Tách số lần nguồn Publish với số callback thực tế. Ghi nhận hiện tượng trước khi đọc hints.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ đọc sau khi tự sửa.

## Expected Results
Starter: phát hiện sai lệch; sau sửa: đúng ba delivery A/B/C, không còn callback từ ca cũ. `./scripts/verify.ps1` phải PASS trên code bạn sửa, không phải trên reference.

## Estimated Time
40 phút.
