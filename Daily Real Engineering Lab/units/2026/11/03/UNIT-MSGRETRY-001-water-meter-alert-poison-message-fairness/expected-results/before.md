# Before
`./reproduce.ps1` trả exit code 0 khi triệu chứng starter được xác nhận: 12 attempts dành cho P1, không có delivery, không có dead-letter. Đây là PASS của bước **reproduce**, không phải PASS của hệ thống.

`./verify.ps1` trên starter phải trả exit code 1, với ít nhất primary và exhaustion/fairness không đạt.
