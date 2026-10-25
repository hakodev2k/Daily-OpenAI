# Before — starter gốc

- **./reproduce.ps1** trả exit code 0 và in **REPRODUCED**.
- **./run.ps1** báo **succeeded=0, failed=1**; mục lỗi mang tên **batch**.
- Các nguồn giả lập thực tế gồm 1 thành công và 2 lỗi riêng biệt.
- **./verify.ps1** trả exit code khác 0; kỳ vọng **2/6 PASS** (trường hợp toàn thành công và batch rỗng). Đây là dấu hiệu lỗi dự kiến, không phải thành công của verification.
- Không yêu cầu số mili-giây cố định; các operation được mô phỏng hoàn toàn trong bộ nhớ.
