# UNIT-PDETAIL-001 — Đồng bộ kiểm kê cây đô thị: phản hồi lỗi mất thông tin điều tra

## Mục tiêu

Tái hiện một lỗi tích hợp HTTP khi đối tác từ chối dữ liệu hoặc tạm ngừng phục vụ; phân biệt triệu chứng, hợp đồng lỗi, khả năng retry và bằng chứng phục vụ vận hành. Chứng minh bản sửa giữ nguyên đường đi thành công.

## Bối cảnh thực tế

Một hệ thống quản lý cây đô thị gửi batch kiểm kê sang cổng tiếp nhận của đơn vị đối tác. Dashboard báo cùng một loại lỗi cho batch sai dữ liệu, đối tác bảo trì và lỗi server cũ. Nhân viên hỗ trợ không tìm được mã lỗi nghiệp vụ hoặc mã trace để trao đổi với đối tác; retry thủ công có thể lặp lại vô ích.

## Bạn cần làm gì

1. Chạy chương trình và reproduce ba tình huống đối tác từ chối.
2. Thu thập status, headers, content-type và response body từ fake partner; viết ít nhất ba hypothesis.
3. Chỉ sửa starter/PartnerClient.cs. Không thay đổi contract runner trong starter/Program.cs.
4. Giữ đường thành công hoạt động; phân biệt lỗi dữ liệu và lỗi tạm thời, không đưa thông tin chi tiết không tin cậy vào log.
5. Chạy verify.ps1 trên chính starter đã sửa; ghi lại trade-off trước khi xem lời giải.

## Yêu cầu môi trường

.NET SDK 8.x, PowerShell 5.1+ hoặc PowerShell 7. Không cần network, Docker, NuGet ngoài SDK hay tài khoản Azure. Fake HTTP handler hoạt động trong process.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
dotnet --version
./run.ps1
./reproduce.ps1
~~~

Có thể chạy trực tiếp:

~~~powershell
dotnet run --project ./starter/TreeSurveyLab.csproj -c Release -- --reproduce
~~~

## Cách reproduce vấn đề

reproduce.ps1 chạy code gốc và trả exit code 0 **chỉ khi** ba triệu chứng lỗi đã được quan sát đúng. Chạy trước khi chỉnh starter. Các response cố định, không phụ thuộc Internet hay đồng hồ máy.

## Những gì cần quan sát

- Status HTTP có giống nhau giữa các tình huống không?
- Dữ liệu chẩn đoán nào tồn tại ở response nhưng không đến được tầng nghiệp vụ?
- Trường hợp nào nên cho phép retry và trường hợp nào không?
- Có thể sửa mà không làm lộ nội dung chi tiết của phản hồi không?

Xem [incident evidence](evidence/incident.md) và ghi [investigation](workspace/my-investigation.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis và evidence.
3. Tự sửa trong starter/PartnerClient.cs.
4. Verify trên starter đã sửa.
5. Chỉ sau đó mới mở reference solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference solution](solution/README.md) và [source](solution/PartnerClient.cs). Không mở trước khi tự điều tra.

## Expected Results

Trước sửa: reproduce.ps1 báo ba lỗi HTTP tổng quát, exit 0. Sau sửa: verify.ps1 PASS bốn contract case (422, 503, 500 không có JSON và 200). verify.ps1 phải FAIL với starter ban đầu. Nếu reproduce không chạy, kiểm tra SDK 8 và vị trí thư mục unit.

## Estimated Time

45–60 phút, L2 / D4.
