# UNIT-HBODY-001 — Tải hồ sơ học bổng vượt deadline dù HTTP đã trả headers

## Mục tiêu
Điều tra ranh giới thời gian của một HTTP download streaming trong .NET, phân biệt việc nhận response headers với việc tiêu thụ toàn bộ body, và chứng minh deadline thực sự bảo vệ một công việc I/O từ đầu đến cuối.

## Bối cảnh thực tế
Dịch vụ đối soát học bổng của trường đại học tải tài liệu xác minh từ đối tác. Partner trả HTTP 200 và headers rất nhanh, nhưng một số tệp vẫn mất quá lâu để hoàn tất. Dashboard ghi nhận request “thành công” dù worker đã vượt SLA 250 ms dành cho từng lượt tải. Các đợt hủy công việc cũng không luôn làm việc tải dừng kịp thời.

## Bạn cần làm gì
1. Chạy lab và quan sát ba trường hợp: body nhanh, body chậm, caller hủy.
2. Ghi ít nhất ba giả thuyết dựa trên evidence, không suy đoán từ tên API.
3. Chỉnh **chỉ** `starter/PartnerAttachmentClient.cs` để áp dụng deadline xuyên suốt operation.
4. Chạy `verify.ps1`, đảm bảo body nhanh vẫn thành công và cả hai tình huống chậm đều dừng đúng hạn.
5. So sánh reference solution sau khi đã tự kiểm chứng.

## Yêu cầu môi trường
- .NET 8 SDK và Windows PowerShell 5.1+ hoặc PowerShell 7+.
- Không cần mạng, Azure, Docker, database hay NuGet ngoài .NET SDK: fake partner được mô phỏng trong process.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 có nghĩa là **lỗi gốc đã được tái hiện**, không có nghĩa là đã sửa. Hãy chạy bước này trước khi chỉnh starter; `verify.ps1` dự kiến không đạt ở starter ban đầu.

## Những gì cần quan sát
- Thời điểm headers đã sẵn sàng so với thời điểm body hoàn tất.
- Elapsed time của request body nhanh và body chậm.
- Kết quả của caller cancellation sau khi headers đã nhận.
- Sự khác nhau giữa HTTP 200, thành công nghiệp vụ và hoàn thành trong deadline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify với `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [solution/README.md](solution/README.md)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
50–70 phút (L3 / D4).
