# UNIT-EF-014 — Filtered Include trả về enrollment không thuộc bộ lọc

## Mục tiêu

Điều tra một sai lệch dữ liệu chỉ xuất hiện khi hai luồng đọc dùng chung một request scope; thu thập bằng chứng, đề xuất giả thuyết, sửa đúng hành vi và chứng minh không hồi quy.

## Bối cảnh thực tế

Nền tảng đào tạo doanh nghiệp cung cấp báo cáo học viên đang hoạt động của từng cohort. Endpoint độc lập trả đúng hai học viên, nhưng sau khi dashboard trong cùng request lấy danh sách đăng ký, báo cáo đôi khi chứa thêm học viên đã ngừng học. Dữ liệu trong SQLite không đổi và không có ghi đồng thời.

## Bạn cần làm gì

1. Chạy hai đường đọc **cold** và **warmed** trên cùng một tập dữ liệu.
2. So sánh ID đầu ra, SQL và trạng thái runtime; ghi ít nhất hai giả thuyết.
3. Xác định vì sao lịch sử truy vấn ảnh hưởng tới kết quả, rồi sửa `starter/CohortReader.cs`.
4. Chạy kiểm chứng để cả hai đường đọc đều chỉ trả về enrollment active, không làm mất hành vi báo lỗi khi cohort không tồn tại.

## Yêu cầu môi trường

- Windows PowerShell 7 hoặc Windows PowerShell 5.1.
- .NET 8 SDK và runtime; NuGet để restore `Microsoft.EntityFrameworkCore.Sqlite` 8.0.20.
- Không cần SQL Server, Docker, tài khoản cloud hoặc dịch vụ trả phí.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chỉ thành công khi **starter ban đầu** biểu hiện đúng sự khác biệt cold/warmed. Sau khi sửa code, chạy `./verify.ps1` thay vì dùng reproduce để đánh giá bản sửa.

## Những gì cần quan sát

- Danh sách ID và số lượng enrollment ở từng đường đọc.
- Có hay không thay đổi dữ liệu giữa hai lần đọc.
- Một query trước đó trong cùng scope có ảnh hưởng tới output hay không.
- Chạy lại với context mới có tái hiện hiện tượng hay không.
- Đừng suy luận chỉ từ SQL: so sánh dữ liệu SQL với object graph trả về.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ xem sau khi đã tự điều tra và thử sửa: [Reference Solution](solution/README.md).

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- Ghi nhận kết quả tại [My Investigation](workspace/my-investigation.md).

## Estimated Time

55 phút (L2 / D4).
