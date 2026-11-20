# Investigation guide
1. Chạy `reproduce.ps1`, lưu stdout và exit code.
2. Vẽ timeline T1 giữ permit, T2 bị cancel, T3 cố vào.
3. Ghi rõ mỗi `WaitAsync` hoàn thành hay ném exception.
4. Kiểm tra invariant: số lần release không thể lớn hơn số lần acquire thành công.
5. Thử exception trong action và kiểm tra permit được trả lại.
6. Sau khi sửa, chạy `verify.ps1`; giải thích tại sao kết quả không phụ thuộc thời gian chờ ngẫu nhiên.
