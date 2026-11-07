# After — learner fix
`./verify.ps1` trả exit 0 với cả 4 case PASS: same-size-overwrite, different-size-overwrite, multiple-pending-events, utf8-boundary. Thứ tự, sensorId và payload nguyên vẹn. `./reproduce.ps1` không còn tìm thấy triệu chứng nên trả exit 1: đó là hành vi đúng **sau khi sửa**, vì script này chỉ chứng minh lỗi gốc.
