# UNIT-AZURE-006 — Concurrent configuration publication

## Mục tiêu
Phân tích lost update khi nhiều worker cùng publish một document lên object storage và chọn concurrency contract phù hợp.

## Bối cảnh thực tế
Hai deployment worker có thể đọc cùng một configuration snapshot, chỉnh các phần độc lập rồi publish gần như đồng thời. Hệ thống hiện báo cả hai lần publish thành công nhưng thay đổi của một worker đôi lúc biến mất.

## Bạn cần làm gì
Dựa trên timeline và starter model, xác định điều kiện tạo lost update, đề xuất và triển khai optimistic concurrency cho write boundary, sau đó kiểm tra conflict được phát hiện thay vì silently overwrite.

## Yêu cầu môi trường
.NET SDK 8.x. Lab dùng local in-memory storage model, không cần Azure subscription.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy hai publisher trên cùng initial version và quan sát final document.

## Những gì cần quan sát
- Cả hai writer có bắt đầu từ cùng version hay không.
- Write thứ hai có biết object đã thay đổi sau lần read hay không.
- Success response có thực sự đồng nghĩa không mất update hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Sau đó mới xem solution.

## Hints
Xem `hints.md`.

## Reference Solution
Xem `solution/README.md` sau khi tự thử.

## Expected Results
Starter cho phép last writer silently replace thay đổi trước. Fixed state phải phát hiện stale writer và buộc caller xử lý conflict.

## Estimated Time
40–55 phút.