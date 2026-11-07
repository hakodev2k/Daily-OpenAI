# Incident brief
- 24 hồ sơ độc lập; mỗi OCR mô phỏng có latency khoảng 120 ms.
- Downstream chấp nhận tối đa 8 request đồng thời; request thứ 9 bị từ chối.
- Operator truyền --max-concurrency N (1..64).
- Yêu cầu: không mất/trùng hồ sơ, không vượt capacity, batch nhanh hơn baseline.
- Evidence: peak concurrency, successful count, rejected, elapsedMs.
- Chỉ starter/Program.cs được sửa. Không sửa harness hoặc solution.
