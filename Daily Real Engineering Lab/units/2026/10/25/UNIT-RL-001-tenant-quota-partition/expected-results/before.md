# Before — dự kiến khi chạy starter chưa sửa

`reproduce.ps1` phải in ra `EXPECTED_INCIDENT_REPRODUCED` và exit 0.

- A: Beta request đầu tiên bị `DENY` thay vì `ALLOW`.
- B: Alpha request thứ ba trên endpoint khác được `ALLOW` thay vì `DENY`.
- C: Anonymous request thứ ba trên endpoint khác được `ALLOW` thay vì `DENY`.
- D: cùng tenant và cùng endpoint: hai `ALLOW`, một `DENY`; đây là positive control.

`verify.ps1` phải in `VERIFICATION_FAILED` và trả exit code 1.

Đây là **kỳ vọng từ static review**, chưa phải kết quả chạy trong môi trường thực thi.
