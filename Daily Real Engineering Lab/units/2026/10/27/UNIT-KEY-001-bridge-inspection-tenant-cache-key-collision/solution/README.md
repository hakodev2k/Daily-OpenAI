# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Hai cặp tenant/report khác nhau trả cùng một briefing nếu cặp đầu đã được cache. Đảo thứ tự thì dữ liệu sai cũng đảo chiều. Một báo cáo không tồn tại tại origin lại có thể trả về dữ liệu từ cache.

## 2. Evidence

NORMAL_REPEAT vẫn đúng và có cache hit. Ở hai fixture chéo, chỉ có một origin read dù có hai tuple khác nhau. Dữ liệu origin được seed riêng theo record struct BriefingId và không đổi.

## 3. Root cause

Starter tạo cache key bằng cách nối tenantId, ký tự ':' và reportId. Hai tuple ("agency:west", "deck-9") và ("agency", "west:deck-9") cùng tạo ra chuỗi "agency:west:deck-9". Cache không thể phân biệt hai danh tính dù origin có thể.

## 4. Why the fix works

[BriefingCache.cs](BriefingCache.cs) mã hóa tuple có cấu trúc bằng JSON array với thứ tự trường ổn định, kèm prefix version "bridge-briefing:v2:". JSON escaping phân biệt rõ ranh giới các thành phần, kể cả khi dữ liệu chứa ':' hoặc ký tự escape. Với input là hai string và serializer cố định, hai tuple khác nhau không thể cùng biểu diễn một JSON array.

## 5. How to verify

Sao chép nội dung solution/BriefingCache.cs vào starter/BriefingCache.cs, rồi chạy ./verify.ps1 từ thư mục unit. Script chỉ kiểm tra learner-editable starter; solution không được dùng trực tiếp để che giấu lỗi trong starter. Kết quả kỳ vọng: RESULT 4/4, exit 0.

## 6. Alternative fixes

- Length-prefix encoding của từng thành phần trên một byte representation chuẩn: hiệu quả, nhưng phải thống nhất encoding và parser giữa các service.
- Escape từng thành phần theo một chuẩn bất biến: ngắn hơn JSON trong một số trường hợp nhưng phải escape cả escape character và kiểm tra tính injective.
- Structured key object cho in-memory cache: có thể dùng tuple (tenantId, reportId) làm dictionary key; với Redis hay distributed cache vẫn cần wire encoding.

## 7. Wrong / tempting fixes

- Bỏ cache hoàn toàn: làm fixture về đúng nhưng mất performance contract và không giải quyết thiết kế identity.
- Thay ':' bằng '_' hoặc dùng dấu khác: chỉ chuyển collision sang một ký tự khác có thể có trong input.
- Chỉ thêm tenantId ở đầu key: tenantId đã có, vấn đề là ranh giới giữa các thành phần.
- Dùng GetHashCode() để tạo key: không bảo đảm duy nhất, không ổn định giữa tiến trình, có nguy cơ collision.
- Bỏ test MISSING_ORIGIN: che giấu nguy cơ dữ liệu chéo tenant.

## 8. Production implications

Đây là lỗi cô lập dữ liệu đa tenant. Cần rà soát log truy cập, phân quyền, cache namespace, invalidation, TTL và cả đường ghi cache. Dùng dữ liệu synthetic trong lab; khi xử lý incident thật phải tuân thủ quy trình bảo mật, không ghi payload nhạy cảm vào logs.

## 9. Trade-offs

JSON array encoding rõ ràng, dễ audit, nhưng dài hơn key thô và có chi phí serialize. Prefix version giúp chuyển đổi không đọc nhầm key cũ; có thể cần xóa key v1 hoặc để TTL hết hạn. Với hệ thống phân tán, hợp đồng canonical encoding phải thống nhất giữa producer và consumer.

## 10. Senior engineering observations

Đúng dữ liệu origin không đủ nếu cache identity không injective. Kiểm thử phải bao gồm thứ tự truy cập đảo ngược, negative lookup, và thuộc tính cache-hit. Bất kỳ tối ưu hóa nào vượt qua authorization boundary đều phải giữ nguyên identity và tenant isolation.
