## Hint 3

Tính riêng `retained = errors * errorRetention + successes * successSampling`. Nếu muốn giữ tất cả errors, cần tail-aware policy; kiểm tra queue memory, late spans và fail-open/fail-safe cho nghiệp vụ.
