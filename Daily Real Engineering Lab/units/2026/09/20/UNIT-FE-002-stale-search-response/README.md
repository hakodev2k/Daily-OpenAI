# UNIT-FE-002 — Stale Search Response

## Mục tiêu
Reproduce và điều tra một UI race condition khi nhiều search request hoàn thành không theo thứ tự người dùng nhập.

## Bối cảnh thực tế
Customer directory có live search. Khi network ổn định, UI luôn có vẻ đúng. Khi người dùng gõ nhanh và một request cũ chậm hơn request mới, đôi lúc danh sách cuối cùng không khớp text đang hiển thị trong ô search.

## Bạn cần làm gì
Chạy deterministic simulator, thu timeline evidence, ghi hypothesis, sửa `starter/search.js`, rồi verify cả kết quả lẫn ordering property.

## Yêu cầu môi trường
- Node.js 20+ hoặc 22+
- PowerShell
- Không cần package ngoài

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script mô phỏng hai user intents liên tiếp với latency cố định khác nhau. Không có randomness.

## Những gì cần quan sát
- Thứ tự user intent.
- Thứ tự request hoàn thành.
- Query hiện tại khi mỗi response được xử lý.
- Query mà final rendered result thực sự đại diện.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/search.js`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
**Before:** input cuối cùng là query mới nhưng rendered result có thể thuộc query cũ.

**After:** completion của request cũ không thể thay thế state của intent mới hơn; request mới nhất vẫn render bình thường.

Nếu không reproduce được, chạy `node starter/search.js reproduce` và kiểm tra Node version.

## Estimated Time
35 phút