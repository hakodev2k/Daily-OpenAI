# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Protected authentication state hoạt động trên instance tạo ra nó nhưng thất bại khi traffic chuyển sang instance khác.

## Evidence

Self-read trên A thành công; cross-instance read trên B ném cryptographic failure. Business payload không thay đổi, nên boundary cần điều tra là protection/key state giữa các instances.

## Root cause

Hai application instances dùng hai Data Protection key rings độc lập. Cùng `ApplicationName` chưa đủ nếu cryptographic keys không được chia sẻ/persist theo deployment topology.

## Why the fix works

Cho cả hai providers dùng cùng một durable key-ring location và cùng application discriminator. Trong lab local, thay `instanceAKeys` và `instanceBKeys` bằng một `sharedKeys` directory cho cả A và B. Production có thể persist key ring vào storage phù hợp với topology và bảo vệ keys at rest theo threat model.

## How to verify

Chạy `./verify.ps1`. `A_SELF_READ` và `B_CROSS_INSTANCE_READ` đều phải PASS.

## Alternative fixes

Sticky sessions có thể giảm tần suất symptom nhưng không giải quyết restart/failover và không tạo durable shared protection boundary. Một centralized authentication architecture có thể thay đổi nơi cookie/token được phát hành, nhưng đó là quyết định lớn hơn.

## Wrong or misleading fixes

- Catch cryptographic exception rồi coi user vẫn authenticated: phá security boundary.
- Hard-code encryption key trong source: tạo secret-management risk.
- Chỉ bật sticky session: che symptom, không bảo đảm failover/restart correctness.
- Vô hiệu hóa Data Protection: loại bỏ cơ chế bảo vệ thay vì sửa deployment state.

## Production implications

Key persistence, key protection at rest, application isolation, rotation và deployment permissions phải được thiết kế cùng nhau. Multi-instance scaling biến local process state thành distributed deployment concern.

## Trade-offs

Shared key storage tăng dependency vận hành và cần access control tốt; đổi lại các equivalent instances có thể đọc protected state nhất quán. Application discriminator phải ngăn applications không liên quan vô tình chia sẻ protection scope.

## What a Senior engineer should notice

Một lỗi chỉ xuất hiện sau scale-out thường là dấu hiệu state từng được giả định là process-local nhưng thực tế thuộc deployment contract. Phải xác định state nào cần shared, state nào cần isolated, ai được đọc keys, cách rotate và cách rollback mà không logout toàn bộ người dùng ngoài ý muốn.
