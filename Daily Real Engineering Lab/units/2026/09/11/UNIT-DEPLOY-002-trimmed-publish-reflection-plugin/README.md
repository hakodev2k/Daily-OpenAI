# UNIT-DEPLOY-002 — Trimmed Publish Reflection Plugin

## Mục tiêu

Điều tra một lỗi deployment chỉ xuất hiện sau khi publish ứng dụng .NET với trimming, sau đó hiện đại hóa cơ chế chọn formatter để hành vi production không phụ thuộc vào metadata mà build pipeline có thể loại bỏ.

## Bối cảnh thực tế

Một worker tạo báo cáo chạy bình thường bằng `dotnet run` trên máy developer. Pipeline mới bật `PublishTrimmed=true` để giảm kích thước artifact. Artifact vẫn build và start được, nhưng job đầu tiên không thể khởi tạo formatter đã cấu hình và dừng xử lý.

## Bạn cần làm gì

1. Chạy trạng thái starter chưa trim và artifact đã trim.
2. Ghi lại khác biệt quan sát được và ít nhất hai hypothesis trong `workspace/my-investigation.md`.
3. Sửa code trong `starter/` để artifact trimmed vẫn chọn đúng formatter.
4. Không được giải quyết bằng cách tắt trimming trong `verify.ps1`.
5. Chạy `verify.ps1`, sau đó mới xem reference solution.

## Yêu cầu môi trường

- Windows x64
- .NET 8 SDK
- PowerShell
- Không cần Docker hay dịch vụ cloud

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-DEPLOY-002-trimmed-publish-reflection-plugin"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy cùng business scenario theo hai cách:

1. `dotnet run` từ project starter.
2. `dotnet publish` self-contained cho `win-x64` với trimming, rồi chạy executable trong artifact.

Script chỉ xác nhận reproduction khi local run xử lý được báo cáo nhưng artifact trimmed không tạo được formatter đã cấu hình.

## Những gì cần quan sát

- Cấu hình formatter giống nhau giữa hai execution path.
- Local run in ra kết quả business mong đợi.
- Artifact trimmed start được nhưng không hoàn tất cùng operation.
- So sánh output build/publish và thành phần thực sự có mặt trong artifact trước khi kết luận.

README cố ý không chỉ ra API hoặc dòng code cần sửa. Hãy dùng evidence từ hai execution path để tìm boundary gây khác biệt.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã tự sửa và chạy verify.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- untrimmed local run: `FORMATTED:ACME`
- trimmed artifact: không tìm/khởi tạo được formatter đã cấu hình và trả exit code khác `0`

### Sau khi sửa

- trimmed artifact vẫn in `FORMATTED:ACME`
- exit code `0`
- trimming vẫn được bật
- cơ chế chọn formatter không phụ thuộc vào việc một type chỉ được nhắc đến từ cấu hình runtime

Nếu không reproduce đúng, chạy `dotnet --info`, xóa thư mục `.artifacts`, rồi chạy lại script.

## Estimated Time

45–70 phút.