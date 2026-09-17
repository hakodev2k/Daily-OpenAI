# UNIT-MONGO-001 — Stable Feed Pagination Under Concurrent Inserts

## Mục tiêu
Điều tra một API phân trang MongoDB có kết quả không ổn định khi dữ liệu thay đổi giữa các lần lấy trang.

## Bối cảnh thực tế
Customer activity feed hiển thị event mới nhất. Khi người dùng chọn Load more, đôi lúc event bị lặp hoặc bị bỏ sót dù từng response riêng lẻ vẫn hợp lệ.

## Bạn cần làm gì
Chạy starter, reproduce hiện tượng, ghi hypotheses, sửa implementation trong starter, chạy verify, rồi mới so sánh reference solution.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell. Không cần MongoDB server vì lab dùng deterministic simulator cho query semantics.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
Ghi ID và timestamp của hai page, thay đổi dataset giữa hai request, và kiểm tra tính liên tục của traversal.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Dùng comments trong starter làm investigation notes; chúng không chỉ ra exact fix.

## Reference Solution
Xem `solution/README.md` sau khi đã tự thử.

## Expected Results
Before: reproduction phát hiện traversal không ổn định sau khi có event mới. After: verification xác nhận các page không overlap và giữ đúng thứ tự traversal.

## Estimated Time
45 phút.