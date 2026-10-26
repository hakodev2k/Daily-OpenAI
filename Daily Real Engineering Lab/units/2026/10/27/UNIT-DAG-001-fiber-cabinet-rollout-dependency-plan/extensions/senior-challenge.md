# Senior challenge — rollout at scale

Bổ sung 1.000 cabinet theo từng ring, concurrency limit 8, bước bắt buộc rollback khi health check thất bại. Phân biệt **planning** với **execution**: planner chỉ phát hành immutable validated plan, executor mới thay đổi thiết bị.

Thiết kế checkpoint, idempotency key, operator approval và recovery sau crash; nêu rõ vì sao cycle validation không thay thế authorization, health check hay transactional rollout. Đo độ phức tạp của planner và giải thích tác động của deterministic tie-break đối với reproducibility.
