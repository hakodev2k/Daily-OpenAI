# Senior Challenge — identity across service boundaries

Một API lưu permit ID trong PostgreSQL, worker .NET dùng dictionary và cache Redis dùng string keys. Thiết kế contract để ba nơi cùng coi `INSP-204` và `insp-204` là một identity nhưng vẫn giữ `INSP-205` độc lập. Nêu cách kiểm tra unique constraint/collation, migration dữ liệu lịch sử, key versioning, collision policy, observability và rollback. Không mặc định rằng một DB collation bất kỳ sẽ tương đương ordinal-ignore-case.
