# UNIT-REQBODY-001 — Webhook điều phối tình nguyện viên trả 400 sau khi bật audit

## Mục tiêu
Điều tra một thay đổi trong ASP.NET Core request pipeline khiến webhook JSON hợp lệ bị từ chối. Thu thập bằng chứng tại ranh giới middleware/endpoint và kiểm tra cả đường đi lỗi.

## Bối cảnh thực tế
Hệ thống điều phối vật tư cứu trợ nhận ticket từ các nhóm tình nguyện viên. Sau khi bật middleware ghi dấu vết audit (chỉ lưu fingerprint SHA-256 và độ dài, không lưu nội dung nhạy cảm), đối tác báo HTTP 400 dù payload hợp lệ. Một số payload sai vẫn phải trả HTTP 400.

## Bạn cần làm gì
1. Reproduce tình huống và ghi hai giả thuyết có thể kiểm chứng.
2. Xác định request đã đi qua các thành phần nào và dữ liệu ở từng ranh giới.
3. Sửa mã trong \`starter/Program.cs\`, không sửa test contract để làm xanh giả.
4. Verify webhook hợp lệ được nhận, payload sai vẫn bị từ chối, audit vẫn hoạt động.

## Yêu cầu môi trường
- Windows PowerShell 7+ hoặc Windows PowerShell 5.1.
- .NET SDK 8 (đề xuất 8.0.4xx); chỉ dùng ASP.NET Core shared framework, không cần Azure, Docker hay NuGet bên ngoài.
- Chạy PowerShell tại thư mục unit này.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Script trả exit code 0 khi triệu chứng ban đầu đã xuất hiện, không có nghĩa ứng dụng đúng. Chạy \`./verify.ps1\` để kiểm tra yêu cầu sau khi tự sửa.

## Những gì cần quan sát
- HTTP status và response của JSON hợp lệ so với JSON sai.
- Số bản ghi audit và độ dài dữ liệu audit quan sát.
- Khả năng đọc request body ở endpoint sau khi middleware xử lý.
- Các khác biệt giữa luồng HTTP thực và một stream chỉ chạy tiến.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong \`starter/Program.cs\`.
4. Verify bằng \`./verify.ps1\`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ đọc sau khi tự thử.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
40 phút. Ghi nhận điều tra tại [workspace](workspace/my-investigation.md).
