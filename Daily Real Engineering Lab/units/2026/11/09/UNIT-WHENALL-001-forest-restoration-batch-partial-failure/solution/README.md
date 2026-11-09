# Reference Solution — spoiler

## Symptoms & evidence
Ba input nhưng một lỗi làm output rỗng. Các task hợp lệ đã chạy xong; console báo batch rejected.

## Root cause
`Task.WhenAll` fault khi có ít nhất một task fault. Code chỉ đọc mảng thành công trong nhánh `try`, còn nhánh `catch` xóa toàn bộ kết quả. Task sibling không bị tự động cancel, nhưng kết quả của chúng bị bỏ qua.

## Why the fix works
Bọc mỗi đơn vị bằng `ImportOneAsync`, chuyển lỗi dữ liệu được dự kiến thành `ImportResult`. `WhenAll` lúc này hoàn thành với đủ ba outcome. Việc phân loại lỗi phải nằm ở boundary của mỗi đơn vị.

## Verify
Chạy `./verify.ps1` sau khi sửa starter; hoặc chạy riêng `dotnet run --project solution/SurveyImport.csproj` để đối chiếu.

## Alternatives
Dùng `Task.WhenAll` và sau khi catch đọc `Task.Status`/result từng task, nhưng cần xử lý fault/cancellation cẩn thận. Dùng bounded concurrency khi nguồn dữ liệu lớn; không cần thiết cho ba input.

## Wrong / tempting fixes
Retry cả batch có thể nhập trùng những báo cáo đã xử lý; `catch` rồi return empty che mất dữ liệu; bỏ hoàn toàn exception có thể làm batch crash. Không nuốt lỗi hạ tầng ngoài danh mục dự kiến.

## Production implications & trade-offs
Cần idempotent persistence, correlation ID theo site, metric success/failure, dead-letter/quarantine, policy partial success rõ ràng. Nếu yêu cầu atomic all-or-nothing thì giữ batch failure và transaction mới đúng; ở đây nghiệp vụ yêu cầu partial acceptance.

## Senior engineer checklist
Phân biệt task completion với business commit, kiểm soát fan-out, phân loại lỗi, đo missing outcomes, đảm bảo retry chỉ cho phần thất bại.
