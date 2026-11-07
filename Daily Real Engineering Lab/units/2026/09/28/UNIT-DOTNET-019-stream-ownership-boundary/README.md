# UNIT-DOTNET-019 — Stream hợp lệ trong service nhưng hỏng khi caller đọc

## Mục tiêu
Điều tra ownership và lifetime của Stream khi một service trả dữ liệu cho caller sau khi scope tạo resource đã kết thúc.

## Bối cảnh thực tế
Một service tạo file export trong memory và trả Stream cho tầng gọi phía trên. Log cho thấy export đã được ghi đủ byte trước khi service return, nhưng caller thất bại ngay khi bắt đầu đọc kết quả.

## Bạn cần làm gì
Chạy starter, ghi lại evidence tại ranh giới producer/consumer, nêu ít nhất hai hypothesis, sửa code trong starter, rồi chạy verification.

## Yêu cầu môi trường
.NET 8 SDK.

## Chạy nhanh
Chạy scripts/run.ps1.

## Cách reproduce vấn đề
Chạy scripts/reproduce.ps1, quan sát số byte service báo đã tạo và lỗi khi caller consume stream. Ghi hypothesis trước khi mở hints.

## Những gì cần quan sát
Resource còn usable ở thời điểm nào; component nào tạo và consume resource; thứ tự giữa method return và cleanup.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-1.md) · [Hint 2](hints/hint-2.md) · [Hint 3](hints/hint-3.md)

## Reference Solution
⚠️ Spoiler: [solution](solution/README.md)

## Expected Results
Caller đọc được đúng payload invoice-42|total=125000 và bạn giải thích được ownership/lifetime contract của returned disposable resource.

## Estimated Time
30–45 phút.
