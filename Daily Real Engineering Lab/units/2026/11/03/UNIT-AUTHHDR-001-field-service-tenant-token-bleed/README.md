# UNIT-AUTHHDR-001 — Token của tenant xuất hiện trong request khác

## Mục tiêu
Điều tra lỗi cách ly danh tính khi .NET worker gọi partner API cho nhiều tenant đồng thời; thu thập evidence, kiểm chứng hypothesis và sửa bằng test interleaving.

## Bối cảnh thực tế
Hai đội bảo trì dùng bearer token riêng. Khi tải tăng, gateway đôi lúc thấy tenant A mang token của tenant B; ở tải tuần tự mọi thứ đều đúng.

## Bạn cần làm gì
Reproduce, ghi hai hypothesis, tìm thời điểm header được tạo và quan sát, sửa **starter/Program.cs**, verify nhiều lần. Không thay đổi assertion hoặc gateway.

## Yêu cầu môi trường
.NET SDK 8.0 và PowerShell, không cần internet hay secrets.

## Chạy nhanh
```powershell
dotnet restore starter/FieldDispatch.csproj
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script thành công khi starter **tái hiện lỗi**, không phải khi đã sửa.

## Những gì cần quan sát
So sánh tenant, token mong đợi và token gateway thực nhận. Phân biệt thứ tự gọi với thời điểm request được xử lý. Kiểm tra khác biệt giữa tuần tự và đồng thời.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
Starter tuần tự PASS, concurrent FAILED; sau sửa cả hai PASS, token đúng tenant.

## Estimated Time
45 phút.
