# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms

Deploy cấu hình ngưỡng cảnh báo 5 phút và queue priority nhưng worker tiếp tục sử dụng 30 phút và queue standard. JSON vẫn parse bình thường, không có exception.

## 2. Evidence

reproduce.ps1 chạy process với BAGGAGE_Baggage__AlertAfterMinutes=5 và BAGGAGE_Baggage__EscalationQueue=priority; output cho thấy EFFECTIVE_ALERT_MINUTES=30, EFFECTIVE_ESCALATION_QUEUE=standard. Hai giá trị này trùng appsettings.json.

## 3. Root cause

Trong starter/Program.cs, AddEnvironmentVariables(prefix: "BAGGAGE_") được gọi trước AddJsonFile. ConfigurationBuilder xử lý trùng key theo thứ tự provider; provider được thêm sau có quyền ghi đè. JSON thắng environment variables nên deployment override bị vô hiệu hóa. Dấu gạch dưới kép __ được normalize thành dấu hai chấm trong key cấu hình; prefix BAGGAGE_ được loại bỏ.

## 4. Why the fix works

Trong solution/Program.cs, đăng ký AddJsonFile trước rồi AddEnvironmentVariables sau. Với các key trùng nhau, biến môi trường process-local thắng JSON; khi không có biến tương ứng, JSON vẫn cung cấp fallback. Giữ nguyên hai key để không làm hỏng contract của ứng dụng.

## 5. How to verify

Chỉnh sửa starter/Program.cs rồi chạy ./verify.ps1. Script build starter, kiểm tra override 5, override 11, sau đó xóa override trong phạm vi process để kiểm tra fallback 30/standard. Cần có ba CONTRACT_PASS và exit code 0. Có thể đối chiếu source trong solution/ nhưng không dùng solution/ để thay cho learner verification.

## 6. Alternative fixes

- Sử dụng WebApplication.CreateBuilder(args) với default providers theo thứ tự được framework thiết lập, nếu toàn bộ ứng dụng phù hợp với host model đó; vẫn cần kiểm thử prefix và override policy.
- Dùng cấu hình explicit theo environment để dễ audit, nhưng phải định nghĩa rõ precedence khi nhiều nguồn cùng tồn tại.

## 7. Wrong / tempting fixes

- Hardcode 5 phút: chỉ giải quyết một deployment, phá fallback và thay đổi tương lai.
- Đổi tên biến mà không đo effective configuration: không giải quyết được source precedence.
- Tăng logging hay restart liên tục: không thay đổi quy tắc chọn provider.
- Bỏ appsettings.json hoàn toàn: có thể làm mất giá trị mặc định cần thiết ở local/test.

## 8. Production implications

Configuration precedence là một phần của deployment contract. Nếu staging và production có thứ tự provider khác nhau, cấu hình quan trọng có thể không có hiệu lực mà không sinh exception. Ghi log nguồn cấu hình ở mức an toàn (không log secret values), validate các ngưỡng vận hành và kiểm thử deployment configuration trước khi nhận traffic.

## 9. Trade-offs

Explicit provider order dễ hiểu nhưng cần maintain khi thêm Azure App Configuration, Key Vault hoặc command-line arguments. Một nguồn ưu tiên cao không có nghĩa luôn đúng: security-sensitive settings có thể cần policy riêng. Dùng validation và observability để phát hiện cấu hình ngoài phạm vi hợp lệ.

## 10. What a Senior engineer should notice

Phân biệt config key mapping, source presence, source precedence và config reload. Không coi deployment success là bằng chứng effective runtime policy đúng. Đặt regression test cho override + fallback và xác nhận test chạy trên code thực sự sẽ được deploy.

## Reference source

Xem Program.cs trong cùng thư mục; chỉ khác vị trí hai provider registration. Không có package ngoài và không cần dịch vụ cloud.
