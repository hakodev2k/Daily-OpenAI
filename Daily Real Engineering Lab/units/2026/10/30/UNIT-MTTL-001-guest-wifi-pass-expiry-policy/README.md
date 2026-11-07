# UNIT-MTTL-001 — Thẻ Wi-Fi khách hết hạn nhưng cổng truy cập vẫn cho phép

## Mục tiêu

Đánh giá một quyết định thiết kế có ảnh hưởng đến quyền truy cập: xác định ranh giới giữa lưu trữ, dọn dữ liệu và quyết định cho phép truy cập. Tự tái hiện sự cố, sửa policy trong ứng dụng .NET và viết một ADR có trade-off phù hợp với SLA.

## Bối cảnh thực tế

Hệ thống Wi-Fi khách của chuỗi không gian làm việc cấp thẻ truy cập có thời điểm hết hạn và có thể bị thu hồi sớm. Sau một đợt thay đổi quy trình lưu trữ, một số lượt kiểm tra vẫn được chấp nhận sau thời điểm hết hạn hoặc sau khi nhân viên thu hồi thẻ. Sự cố chỉ kéo dài một khoảng thời gian và biến mất mà không có deploy.

## Bạn cần làm gì

1. Chạy fixture, đối chiếu các quyết định ALLOW/DENY với hợp đồng nghiệp vụ.
2. Viết ít nhất ba giả thuyết; dùng timeline và output để loại trừ.
3. Chỉ chỉnh sửa starter/AccessPolicy.cs để sửa hành vi; không chỉnh các contract case trong Program.cs.
4. Chạy verify.ps1, chứng minh cả expiry boundary, early revocation và thẻ hợp lệ.
5. Viết ADR trong workspace/decision.md: chọn một kiến trúc triển khai, nêu cách đáp ứng SLA và phương án fallback khi kho dữ liệu lỗi.
6. Sau đó mới xem reference solution và so sánh các trade-off.

## Yêu cầu môi trường

- .NET SDK 8.x và .NET 8 runtime, PowerShell 5.1+ hoặc 7.
- Không cần MongoDB, Docker, tài khoản cloud hoặc NuGet bên ngoài. In-memory fixture mô phỏng độ trễ dọn dữ liệu; không giả vờ kiểm chứng hành vi của một MongoDB server thật.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
dotnet --version
./run.ps1
./reproduce.ps1
~~~

Hoặc:

~~~powershell
dotnet run --project ./starter/VisitorPassLab.csproj -c Release -- --reproduce
~~~

## Cách reproduce vấn đề

reproduce.ps1 chạy starter và chỉ thoát mã 0 khi các triệu chứng kỳ vọng của bản lỗi thực sự xuất hiện. Không chỉnh sửa starter trước lần chạy đầu. Các tình huống dùng đồng hồ UTC cố định, không phụ thuộc thời gian máy.

## Những gì cần quan sát

- Thẻ nào được cho phép ngay trước thời điểm hết hạn, đúng thời điểm hết hạn và sau thời điểm đó?
- Điều gì thay đổi khi một bản ghi được xóa muộn hơn?
- Thẻ đã thu hồi có cùng hành vi với thẻ chưa tồn tại hay không?
- Một thay đổi policy có thể bảo vệ hợp đồng nghiệp vụ mà không cần thêm hạ tầng không?

Đọc [timeline](evidence/incident-timeline.md), [ràng buộc](docs/decision-constraints.md) và ghi [investigation/ADR](workspace/decision.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong starter/AccessPolicy.cs.
4. Verify bằng ./verify.ps1 trên chính starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference solution](solution/README.md) và [ADR tham khảo](solution/decision-record.md) chỉ nên đọc sau khi tự điều tra.

## Expected Results

Trước sửa: reproduce.ps1 thoát 0 và hiển thị ít nhất ba tình huống quyết định sai. Sau sửa: verify.ps1 thoát 0 với toàn bộ case PASS. Nếu không tái hiện được, kiểm tra .NET 8, chạy từ thư mục unit và đảm bảo chưa thay đổi starter.

## Estimated Time

60–75 phút (L5 design decision, D4 production constraints).
