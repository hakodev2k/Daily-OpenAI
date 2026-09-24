# UNIT-IIS-001 — HTTPS Redirect Loop Behind Reverse Proxy

## Mục tiêu
Điều tra một production incident chỉ xuất hiện khi ASP.NET Core chạy sau reverse proxy, dựa trên request evidence thay vì đoán cấu hình.

## Bối cảnh thực tế
Partner Portal vừa được chuyển sang topology public HTTPS → reverse proxy → ASP.NET Core backend. Health endpoint vẫn ổn nhưng browser báo quá nhiều redirects ở các route ứng dụng. Chạy trực tiếp backend ở local không thấy lỗi.

## Bạn cần làm gì
1. Reproduce topology mô phỏng bằng starter.
2. Ghi lại scheme và forwarding evidence của từng hop.
3. Đưa ra ít nhất ba hypothesis trước khi sửa.
4. Sửa learner-editable starter để public request kết thúc bằng 200 mà vẫn giữ HTTPS policy.
5. Chạy verify và so sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell
- Không cần IIS/Azure thật; starter mô phỏng proxy boundary local.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script khởi chạy backend và proxy simulator, gửi một request theo public path và theo redirect tối đa 5 lần. Exit code khác 0 là expected ở starter khi symptom được reproduce.

## Những gì cần quan sát
- Chuỗi HTTP status/Location qua từng hop.
- Scheme mà backend ghi nhận.
- Header mà proxy gửi tới backend.
- CPU/process vẫn bình thường trong khi client không tới được response cuối.

Evidence được in trực tiếp bởi chương trình; ghi hypothesis vào `workspace/my-investigation.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Starter: request public không đạt 200 trong redirect budget. Sau fix: request public đạt 200, backend vẫn áp dụng HTTPS policy theo public request semantics.

Nếu không reproduce được, chạy `dotnet --info`, xác nhận port 5181/5182 chưa bị dùng và chạy lại `./reproduce.ps1`.

## Estimated Time
60 phút.