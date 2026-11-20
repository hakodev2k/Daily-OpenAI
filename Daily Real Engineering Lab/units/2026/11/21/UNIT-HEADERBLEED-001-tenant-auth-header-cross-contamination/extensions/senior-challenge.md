# Senior challenge
Thiết kế API `PartnerDispatcher` nhận tenant context + token provider; không để caller ghi shared HTTP client state. Thêm cancellation token, retry chỉ cho operation idempotent và telemetry không lộ credential. Mô tả test khi 100 tenant gửi đồng thời và cách xác minh không cross-tenant leak.
