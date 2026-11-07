# After — query đã sửa

- `./verify.ps1` pass baseline: `["WT-042","WT-044","WT-045"]`.
- Regression thêm `WT-047` (không phải brake failure) và `WT-048` (brake failure); kết quả phải là `["WT-042","WT-044","WT-045","WT-048"]`.
- `VERIFY_PASSED learner_query` và exit code 0.
- `./reproduce.ps1` không còn xác nhận triệu chứng baseline sau khi sửa, vì script đó chỉ kiểm tra lỗi gốc.
