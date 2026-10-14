# Before — starter
`./reproduce.ps1` trả exit 0 và in `EXPECTED SYMPTOM REPRODUCED`. Sự kiện sensor-A không còn `temperature:21.4`; sensor-B vẫn là `temperature:39.1`. `./verify.ps1` trả exit 1, nhiều case FAIL. Đây là **test chứng minh lỗi**, không phải validation thành công.
