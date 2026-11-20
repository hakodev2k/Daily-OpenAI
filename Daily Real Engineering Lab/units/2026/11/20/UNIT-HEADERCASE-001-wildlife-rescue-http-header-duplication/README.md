# UNIT-HEADERCASE-001 — Gateway cứu hộ động vật route sai khi header bất thường

## Mục tiêu
Tái hiện, thu thập evidence và sửa sai lệch giữa HTTP input và quyết định routing trong .NET.

## Bối cảnh thực tế
Một gateway nhận yêu cầu chuyển động vật hoang dã tới vùng cứu hộ north/south. Proxy và ứng dụng báo khác nhau khi đối tác gửi header X-Rescue-Zone nhiều lần. Hệ thống có thể chọn vùng ngoài ý định của đối tác.

## Bạn cần làm gì
Chạy replay, ghi lại từng trường hợp, đề xuất hợp đồng validation và sửa starter để không route yêu cầu ambiguous.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell, không cần database hay Azure.

## Chạy nhanh
```powershell
dotnet run --project starter/RescueGateway.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit, kiểm tra case conflict.

## Những gì cần quan sát
So sánh single, duplicate-identical, duplicate-conflicting, missing và empty. Ghi lại input và output trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[1](hints/hint-01.md), [2](hints/hint-02.md), [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md)

## Expected Results
Chỉ đúng một header hợp lệ được route; request mơ hồ phải bị reject.

## Estimated Time
50 phút.
