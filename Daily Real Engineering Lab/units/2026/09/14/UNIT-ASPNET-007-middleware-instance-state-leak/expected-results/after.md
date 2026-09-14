# After

- `request-a -> tenant-a`
- `request-b -> tenant-b`
- Không có cross-request tenant contamination.
- Fix không dựa vào serialize toàn bộ request hoặc global lock.
