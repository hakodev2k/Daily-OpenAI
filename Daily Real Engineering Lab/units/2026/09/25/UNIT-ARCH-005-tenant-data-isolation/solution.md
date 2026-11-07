# One defensible solution under the stated constraints

Một hybrid model là phương án có thể bảo vệ tốt các constraints: tenant nhỏ/lớn thông thường dùng shared PostgreSQL boundary với tenant key bắt buộc trong data-access contract; enterprise tier có thể được placement sang dedicated database khi cần restore/encryption/isolation boundary riêng.

Điểm quan trọng không phải “hybrid” tự nó tốt hơn, mà là control plane phải explicit: tenant ID → placement mapping là source of truth; request resolve placement trước data access; connection pools được bounded theo placement; migration tooling chạy theo cohort; backup/restore procedure được test theo tier.

Shared tier cần defense-in-depth cho tenant isolation: application authorization, mandatory tenant scoping, database policy như row-level security nếu phù hợp, integration tests chống cross-tenant access và audit telemetry. Dedicated tier giảm blast radius nhưng tăng fleet operations, migrations, credentials, monitoring và connection management.

Rejected default: database-per-tenant cho toàn bộ 600 tenant vì operational/cost fan-out không được chứng minh là cần thiết. Rejected default: một shared database duy nhất cho mọi tier nếu enterprise restore/encryption boundary không thể đáp ứng.

Revisit triggers: enterprise ratio tăng mạnh, compliance yêu cầu physical isolation cho mọi tenant, shared-tier noisy-neighbor vượt SLO, migration window không còn đạt, hoặc automation đủ trưởng thành để vận hành database fleet với chi phí chấp nhận được.
