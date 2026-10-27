# Reference Solution — SPOILER

Chỉ đọc sau khi đã chạy reproduce, ghi hypothesis và thử sửa starter.

## 1. Symptoms
Hai bản ghi encrypted trong cùng batch vẫn giải mã được nhưng người quan sát không có key vẫn suy ra quan hệ byte giữa hai plaintext. Rủi ro cao hơn nếu một plaintext hoặc một phần nội dung đã biết.

## 2. Evidence
Harness so sánh trường Nonce, kiểm tra round-trip và xác minh rằng ciphertextA[i] XOR ciphertextB[i] bằng plaintextA[i] XOR plaintextB[i] với hai bản ghi cùng chiều dài. Đây là bằng chứng hai phép mã hóa sử dụng cùng keystream, không phải bằng chứng key đã lộ.

## 3. Root cause
Starter lấy 12 byte đầu của SHA-256(batchId) làm AES-GCM nonce. Mọi record trong cùng batch sử dụng cùng key và nonce. AES-GCM dùng CTR mode để mã hóa; tái sử dụng cặp (key, nonce) lặp keystream và làm lộ XOR của plaintext. Nonce reuse còn làm mất bảo đảm integrity/authenticity trong threat model phù hợp.

## 4. Why the fix works
RandomNumberGenerator.GetBytes(12) tạo nonce 96-bit mới cho từng lần Encrypt. Nonce được lưu trong envelope, không cần giữ bí mật. AAD vẫn là batchId nên giải mã sai batch thất bại. Xác suất collision tăng theo birthday bound; phải giới hạn số bản ghi trên mỗi key và có chính sách key rotation.

## 5. How to verify
Chạy ./verify.ps1 trên starter đã chỉnh sửa. Chỉ để đối chiếu sau khi tự làm:

~~~powershell
Copy-Item ./starter/EnvelopeCrypto.cs ./starter/EnvelopeCrypto.mine.cs.bak
Copy-Item ./solution/EnvelopeCrypto.cs ./starter/EnvelopeCrypto.cs -Force
./verify.ps1
~~~

Harness kiểm tra 48 record cùng batch có nonce khác nhau, decrypt round-trip, reject tag/ciphertext/key/batch bị chỉnh sửa. Đây là regression test, không phải formal proof về cryptographic safety.

## 6. Alternative fixes
- Random 96-bit nonce và key rotation là phương án đơn giản cho exporter quy mô vừa.
- Counter nonce có thể bảo đảm uniqueness mạnh hơn nếu phân bổ, persist và phối hợp qua replica/restart đúng; counter chỉ nằm trong RAM có thể reuse sau crash.
- Key riêng cho mỗi batch giới hạn blast radius nhưng KHÔNG sửa reuse nếu nhiều record cùng batch vẫn trùng nonce.

## 7. Wrong / tempting fixes
- Hash plaintext thành nonce: input trùng sẽ cho nonce trùng; không đáp ứng uniqueness.
- Tăng tag length: không sửa lỗi nonce reuse.
- Chỉ kiểm tra decrypt đúng: không kiểm tra confidentiality hoặc uniqueness.
- Đổi batchId ở client: không bảo đảm uniqueness qua replica/retry.
- Chỉ đổi nonce trong envelope sau Encrypt: ciphertext và tag vẫn gắn với nonce cũ.

## 8. Production implications
Audit cách tạo nonce ở mọi replica và version; đánh giá số message/key và rotation. Vá code không khôi phục confidentiality của ciphertext đã phát hành với nonce trùng. Cần đánh giá exposure và phản ứng sự cố theo threat model. Không log key hay plaintext thật.

## 9. Trade-offs
Random nonce không cần shared counter nhưng phải quản trị xác suất collision và key lifetime. Counter nonce có invariant rõ hơn khi quản trị đúng nhưng phát sinh persistence, crash recovery và coordination. Không tự chế protocol khi có định dạng đã được review.

## 10. What a Senior engineer should notice
Encryption success không đồng nghĩa security. Invariant AEAD phải được kiểm tra ở cấp protocol, negative paths, key lifecycle và envelope versioning; không chỉ ở một round-trip test.
