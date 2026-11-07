# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

compact-valid được chấp nhận, nhưng pretty-valid và spaced-valid bị từ chối dù chữ ký được tạo từ đúng payload tương ứng. tampered-body, malformed-signature và truncated-signature bị từ chối.

## Evidence

Trong LabCases.Build, chữ ký được tính trên từng mảng byte của payload. Starter lại phân tích JSON rồi serialize thành một mảng byte mới trước khi tính HMAC. Khi JSON chứa whitespace hợp lệ, mảng byte mới không còn giống mảng byte gốc.

## Root cause

HMAC xác thực **dãy byte**, không xác thực ý nghĩa của đối tượng JSON. Hai tài liệu JSON có cùng giá trị nghiệp vụ nhưng cách biểu diễn byte khác nhau sẽ có HMAC khác nhau. Việc parse rồi serialize lại ở phía receiver làm thay đổi dữ liệu được xác thực; đó là lỗi contract giữa sender và receiver.

## Why the fix works

Đọc [WebhookVerifier.cs](WebhookVerifier.cs): tính HMACSHA256 trực tiếp trên body byte[] nhận được, sau đó so sánh chữ ký nhị phân với CryptographicOperations.FixedTimeEquals. Signature sai định dạng hoặc sai độ dài vẫn bị từ chối. Parsing và business validation phải diễn ra **sau** bước xác thực; không thay đổi body trước bước này.

Để tự so sánh, copy nội dung solution/WebhookVerifier.cs vào starter/WebhookVerifier.cs, sau đó chạy verify.ps1. Không sửa Program.cs hoặc fixtures.

## How to verify

~~~powershell
.\verify.ps1
~~~

Kết quả mong đợi: cả sáu fixture đều match=true, VERIFY PASS, exit code 0. Kiểm tra thêm: thay đổi một byte của payload nhưng giữ nguyên signature phải bị từ chối; JSON có whitespace khác nhưng được ký đúng phải được chấp nhận.

## Alternative fixes

- Nếu hợp đồng tích hợp **quy định canonical JSON chính thức**, cả sender và receiver có thể canonicalize theo **cùng một đặc tả chính xác** trước khi ký. Đây là một contract khác, không phải hành vi được phép tự suy đoán.
- Nếu nhà cung cấp dùng định dạng signature khác (Base64, timestamp+body, prefix version), phải tuân theo đúng thông số nhà cung cấp; không áp dụng ví dụ hex của lab một cách máy móc.

## Wrong / tempting fixes

- Bỏ xác thực signature để giảm false negative: làm mất kiểm tra integrity và authenticity.
- Parse JSON rồi serialize với một tùy chọn khác: chỉ chữa vài fixture, vẫn không bảo đảm byte-for-byte tương thích.
- So sánh chuỗi signature bằng toán tử ==: không cung cấp đặc tính so sánh constant-time của primitive chuyên dụng.
- Bỏ các test negative để pipeline xanh: che giấu lỗi integrity.

## Production implications

- Đọc raw request body có giới hạn kích thước; bảo đảm middleware không tiêu thụ body trước verifier.
- Quản lý secret qua secret store; hỗ trợ rotation và key identifier nếu provider có quy định.
- Xác minh timestamp/replay window và idempotency của event theo hợp đồng provider; HMAC đúng **không** tự ngăn replay.
- Không log secret, full signature hoặc payload chứa dữ liệu lương. Log event ID an toàn, kết quả xác thực, correlation ID.
- Dùng signature header/encoding chính xác; không tự đặt chuẩn canonicalization.

## Trade-offs

Raw-body HMAC đơn giản, đúng với provider ký raw bytes và không phụ thuộc serializer; đổi lại cần giữ nguyên body byte[] trong request pipeline và kiểm soát kích thước. Canonical JSON chỉ phù hợp khi hai bên có chuẩn canonical chung và được kiểm thử liên thông. Không cần thêm broker, cache hay service chỉ để giải quyết lỗi này.

## What a Senior engineer should notice

Tách integrity check khỏi deserialization và business validation; xác định rõ trust boundary; bảo vệ cả positive lẫn negative paths bằng contract tests. Khi có sự cố, đối chiếu byte payload ở sender/receiver bằng digest an toàn thay vì in dữ liệu nhạy cảm. Đánh giá replay, key rotation và operational telemetry trước rollout.
