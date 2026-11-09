# UNIT-HTTPRETRY-001 — Upload ảnh di sản thất bại khi thử lại HTTP

## Mục tiêu
Điều tra một lỗi xảy ra sau phản hồi HTTP tạm thời, bảo toàn payload khi retry và chứng minh hành vi bằng kiểm thử có thể lặp lại.

## Bối cảnh thực tế
Dịch vụ số hóa ảnh hiện vật gửi ảnh tới kho lưu trữ qua HTTP. Gateway đôi khi trả 503 ở lần đầu. Người vận hành thấy một số ảnh không được chuyển dù hệ thống đã bật retry.

## Bạn cần làm gì
Chạy hệ thống, ghi nhận số request thực sự tới gateway, lập ít nhất hai giả thuyết, sửa đường gửi ảnh và viết kết luận về retry an toàn cho POST.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell 7+; không cần Internet hoặc dịch vụ ngoài.

## Chạy nhanh
```powershell
dotnet run --project starter/HeritageUpload.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Kỳ vọng script xác nhận lỗi đã tái hiện (exit 0); đây không phải kiểm thử bản sửa.

## Những gì cần quan sát
Ghi exception, status ở lần đầu, số lần gateway thực nhận và ảnh hưởng khi gateway tạm lỗi. So sánh với tình huống gateway trả 200 ngay.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45–55 phút.

