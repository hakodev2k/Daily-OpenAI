# After — sau khi sửa

- `./verify.ps1` exit 0.
- `Sequential contract: PASS` và `Overlapping contract: PASS`.
- Nhãn A đúng duy nhất: `Label|LOT-A|North-Vault|R-01` và `Fee|17.00`.
- Nhãn B đúng duy nhất: `Label|LOT-B|South-Vault|R-02` và `Fee|42.00`.
- Không có thông tin LOT-B trong nhãn A và ngược lại.
- Nếu chạy `./reproduce.ps1` trên starter đã sửa, script sẽ exit 1 vì triệu chứng cũ không còn; điều này là dự kiến.

Giữ cả hợp đồng tuần tự, vì một bản sửa chỉ hoạt động dưới concurrency nhưng phá đường đi bình thường cũng không đạt.
