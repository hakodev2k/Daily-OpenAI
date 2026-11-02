# UNIT-UPSIG-001 — Hồ sơ cấp phép xây dựng được nhận nhưng không đọc được bản vẽ

## Mục tiêu

Điều tra một sự cố tiếp nhận tài liệu ở ranh giới API, xác định bằng chứng đáng tin cậy, sửa logic admission và bổ sung regression tests cho đường đi người dùng tự chỉnh sửa.

## Bối cảnh thực tế

Portal cấp phép xây dựng nhận bản vẽ PDF của công dân. API báo `ACCEPTED`, nhưng dịch vụ xử lý tài liệu phía sau liên tục trả `DOCUMENT_PARSE_FAILED`. Một số hồ sơ vẫn xử lý bình thường. Nhóm vận hành nghi ngờ parser mới triển khai, dữ liệu bị hỏng khi truyền hoặc lỗi kiểm tra đầu vào.

## Bạn cần làm gì

1. Chạy baseline để tái hiện tình huống một tệp được nhận ở cửa vào nhưng không phù hợp với bước xử lý sau.
2. Đọc log và đối chiếu các giả thuyết. Ghi lại bằng chứng trước khi sửa.
3. Chỉnh `starter/UploadGate.cs` để chặn đầu vào không đáp ứng hợp đồng của pipeline, đồng thời giữ các trường hợp hợp lệ.
4. Chạy `verify.ps1` trên chính mã `starter/` đã sửa. Không chép lời giải trước khi điều tra.

## Yêu cầu môi trường

- .NET SDK 8.0.x, PowerShell 7+ (Windows/Linux/macOS). Không cần database, Docker, cloud hay NuGet ngoài SDK.
- Toàn bộ dữ liệu là synthetic; không tải tệp thực tế của công dân.

## Chạy nhanh

Chạy từ thư mục unit:

~~~powershell
pwsh ./scripts/setup.ps1
pwsh ./scripts/reproduce.ps1
~~~

## Cách reproduce vấn đề

`reproduce.ps1` biên dịch baseline cố định, thực thi một hồ sơ mẫu và xác nhận triệu chứng đã được tái hiện. Script không phụ thuộc vào việc bạn đã sửa `starter/` hay chưa. Xem `evidence/incident-log.txt` và quan sát kết quả `ACCEPTED` ở admission đối chiếu với bước xử lý phía sau.

## Những gì cần quan sát

- Sự khác nhau giữa kết quả kiểm tra ở cửa vào và kết quả xử lý tài liệu.
- Những thuộc tính nào đến từ người gửi và những thuộc tính nào có thể kiểm chứng độc lập.
- Những trường hợp âm tính phải bị chặn, và những trường hợp dương tính không được hồi quy.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix ở `starter/UploadGate.cs`.
4. Verify bằng `pwsh ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi tự điều tra và verify.

## Expected Results

- Baseline: tái hiện hồ sơ không phù hợp nhưng được admission chấp nhận.
- Sau fix: `verify.ps1` pass các positive/negative contracts, không cần dựa vào reference solution.
- Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time

55–70 phút (L3 / D4).
