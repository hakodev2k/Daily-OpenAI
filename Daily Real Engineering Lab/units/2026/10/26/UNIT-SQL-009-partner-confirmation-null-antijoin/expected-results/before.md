# Before
- Seed có bốn hợp đồng ACTIVE của tenant A.
- Batch 42 chứa xác nhận hợp lệ và một dòng thiếu mã hợp đồng.
- Baseline trả về **0 dòng**, dù có hai hợp đồng ACTIVE chưa được xác nhận.
- `reproduce.ps1` phải thành công khi chứng minh được triệu chứng; đây **không** phải dấu hiệu nghiệp vụ đúng.
