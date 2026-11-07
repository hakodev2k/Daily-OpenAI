# UNIT-BOX-001 — Khách sạn: số túi đồ giặt đã quét nhưng báo cáo cuối ca bằng 0

## Mục tiêu

Điều tra sai lệch giữa kết quả xử lý của worker và số liệu đối soát cuối ca trong ứng dụng .NET; dùng bằng chứng để xác định nguyên nhân và kiểm chứng tính nhất quán của dữ liệu.

## Bối cảnh thực tế

Bộ phận giặt là của một khách sạn lớn quét mã túi đồ để lập phiếu bàn giao. Hệ thống loại bỏ mã trùng trong cùng ca và bỏ qua mã không đọc được. Dashboard của worker ghi nhận các túi đã tiếp nhận, nhưng báo cáo tổng hợp cuối ca lại ghi số lượng thấp bất thường. Điều này gây chênh lệch giữa bàn giao thực tế và hóa đơn đối tác.

## Bạn cần làm gì

1. Chạy fixture gốc và ghi số đếm tại ba điểm quan sát.
2. Viết ít nhất hai giả thuyết, phân biệt lỗi dữ liệu đầu vào với lỗi trạng thái ứng dụng.
3. Chỉ sửa code trong starter/LaundryAudit/; giữ nguyên contracts/.
4. Chạy verify, chứng minh ca sáng, ca chiều và ca rỗng không bị ảnh hưởng lẫn nhau.
5. Sau khi hoàn thành, so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x và .NET 8 runtime; không cần NuGet ngoài, Docker hoặc cloud.
- Windows PowerShell 5.1+ hoặc PowerShell 7.
- global.json yêu cầu SDK tối thiểu 8.0.100; có thể dùng SDK mới hơn nếu máy hỗ trợ.

## Chạy nhanh

Tại thư mục unit:

~~~powershell
dotnet --version
./run.ps1
./reproduce.ps1
~~~

Có thể chạy trực tiếp:

~~~powershell
dotnet run --project ./contracts/LaundryAudit.Contract.csproj -c Release -- --reproduce
~~~

## Cách reproduce vấn đề

reproduce.ps1 chạy contract độc lập dựa trên starter. Fixture có ba túi hợp lệ, một lần quét trùng, một mã không đọc được và một ca khác để đối chiếu. Script thoát mã 0 chỉ khi triệu chứng gốc thực sự xuất hiện. Ghi lại số liệu trước khi chỉnh sửa.

## Những gì cần quan sát

- Số túi được worker chấp nhận có khớp với kết quả xử lý?
- Số báo cáo cuối ca có khớp với số worker không?
- Lỗi có phụ thuộc vào túi bị trùng, mã không đọc được hoặc việc chuyển ca?
- Có sự khác biệt giữa số liệu được quan sát ở các thành phần?

Xem [bằng chứng](evidence/shift-reconciliation.md) và ghi [investigation](workspace/my-investigation.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter/.
4. Verify bằng ./verify.ps1, chạy chính starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference solution](solution/README.md) chỉ dành cho sau khi tự điều tra và verify.

## Expected Results

Trước sửa: ca sáng worker và số túi được xử lý đều là 3 nhưng báo cáo cuối ca là 0; ca chiều tương tự là 1 so với 0. reproduce.ps1 thoát 0.

Sau sửa: ca sáng 3/3/3, ca chiều 1/1/1 và ca rỗng 0/0/0. verify.ps1 thoát 0, không cần sửa contracts/. Nếu không reproduce được, kiểm tra .NET SDK/runtime, chạy lại từ thư mục unit và xác nhận chưa sửa starter.

## Estimated Time

35–45 phút. Phần [concepts after lab](docs/concepts-after-lab.md) chỉ đọc sau khi thử nghiệm.
