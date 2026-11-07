# UNIT-MONGO-003 — Partial model, full document replacement

## Mục tiêu
Điều tra một data-loss bug tại persistence boundary khi application model chỉ biết một phần document nhưng write operation thay thế toàn bộ persisted representation.

## Bối cảnh thực tế
Một legacy service cập nhật trạng thái của customer document. Sau deployment, field do service khác sở hữu đôi lúc biến mất dù request chỉ thay đổi status. Log không có exception và write được database chấp nhận.

## Bạn cần làm gì
Phân tích before/after snapshots, xác định write contract nào đang quá rộng, sửa starter model để chỉ mutation được yêu cầu tác động tới persisted document, rồi verify các field không thuộc ownership của service vẫn được giữ nguyên.

## Yêu cầu môi trường
.NET SDK 8.x. Lab mô phỏng document store local, không cần MongoDB server.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy update trên document có thêm field mà application DTO không biểu diễn, rồi so sánh snapshot trước và sau.

## Những gì cần quan sát
- Field nào request thực sự muốn thay đổi.
- Field nào biến mất ngoài ý muốn.
- Application model có đại diện đầy đủ persisted document hay không.

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
Starter làm mất field ngoài ownership của updater. Fixed state chỉ thay đổi status và giữ nguyên các field còn lại.

## Estimated Time
35–50 phút.