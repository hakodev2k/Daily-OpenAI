# UNIT-KEY-001 — Bridge inspection briefings: tenant cache returns the wrong report

## Mục tiêu

Điều tra một lỗi cô lập dữ liệu giữa các tenant trong một lớp cache của backend .NET. Dùng evidence và regression fixtures để phân biệt lỗi dữ liệu gốc với lỗi xảy ra trong đường đọc đã cache.

## Bối cảnh thực tế

Nền tảng quản lý kiểm định cầu phục vụ nhiều cơ quan địa phương. Dữ liệu nguồn đã phân biệt từng cặp tenant/report. Một số yêu cầu đọc báo cáo lại trả về nội dung của cơ quan khác, đặc biệt sau khi hệ thống vừa phục vụ một yêu cầu trước đó. Lab chỉ sử dụng dữ liệu synthetic và không kết nối mạng.

## Bạn cần làm gì

1. Chạy fixture và ghi lại test nào thất bại, thứ tự truy cập và các chỉ số cache.
2. Đưa ra ít nhất hai hypothesis, sau đó xác định bằng chứng bác bỏ hypothesis không phù hợp.
3. Chỉ chỉnh sửa mã trong thư mục starter; giữ nguyên hợp đồng dữ liệu nguồn và test harness.
4. Bảo đảm mỗi tenant chỉ nhận báo cáo thuộc đúng cặp tenant/report, kể cả khi origin không có bản ghi.
5. Chạy verify, giải thích cách bảo đảm tính duy nhất và đánh giá chi phí vận hành.

## Yêu cầu môi trường

- .NET 8 SDK (global.json pin 8.0.100, cho phép roll-forward tới bản feature SDK 8.0 mới hơn).
- PowerShell 7 hoặc Windows PowerShell 5.1; không cần Docker, NuGet ngoài SDK hay cloud account.

## Chạy nhanh

Từ thư mục unit này:

~~~powershell
dotnet --version
./reproduce.ps1
~~~

Nếu không dùng PowerShell, chạy: dotnet run --project starter/BridgeBriefingLab.csproj (exit code 12 là symptom dự kiến của starter).

## Cách reproduce vấn đề

Chạy ./reproduce.ps1. Harness kiểm tra lượt đọc thông thường, hai thứ tự truy cập chéo và một trường hợp không có bản ghi ở origin. Script chỉ thành công nếu nhận đúng failure signature của starter.

## Những gì cần quan sát

- Kết quả PASS/FAIL của bốn fixture, đặc biệt sự khác biệt giữa hai thứ tự đọc.
- OriginReads, CacheHits và CacheEntries khi dữ liệu đọc khác kỳ vọng.
- Tình huống origin không có bản ghi nhưng API vẫn trả về nội dung.
- Không có database, mạng hoặc cache service thật tham gia reproduction.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trong starter/BriefingCache.cs.
4. Verify bằng ./verify.ps1, bao gồm regression và cache-hit contract.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và verify.

## Expected Results

- Starter: NORMAL_REPEAT PASS; ba fixture còn lại FAIL; process exit 12.
- Sau fix: bốn fixture PASS, process exit 0; cache vẫn có hit với cùng một cặp tenant/report.
- Chi tiết: [Before](expected-results/before.md) và [After](expected-results/after.md).

## Estimated Time

55 phút (D4 / L2).
