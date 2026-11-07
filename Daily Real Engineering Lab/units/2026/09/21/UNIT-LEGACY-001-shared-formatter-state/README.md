# UNIT-LEGACY-001 — Shared Formatter State During Legacy Modernization

## Mục tiêu

Refactor một helper C# cũ đang được dùng bởi billing service mà vẫn giữ nguyên output contract và loại bỏ lỗi chỉ xuất hiện khi nhiều statement được render đồng thời.

## Bối cảnh thực tế

Một billing service đã chạy ổn định nhiều năm. Sau khi workload được chuyển sang xử lý nhiều statement song song, smoke test tuần tự vẫn pass nhưng regression test song song đôi khi phát hiện nội dung của statement A xuất hiện trong statement B. Team muốn hiện đại hóa helper mà không đổi public formatting contract.

## Bạn cần làm gì

1. Chạy starter ở chế độ tuần tự và song song.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Xác định state nào thuộc về toàn ứng dụng và state nào chỉ thuộc về một lần render.
4. Refactor `starter/` nhưng giữ nguyên public method và format output.
5. Chạy `verify.ps1`.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Sequential rendering có ổn định không.
- Parallel rendering có giữ đúng `StatementId`, customer và line items của từng request không.
- Public output contract có bị thay đổi sau refactor không.
- Lỗi có phụ thuộc vào timing giữa hai operation hay không.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

Before: sequential check pass; parallel regression check phát hiện ít nhất một statement không còn độc lập.

After: cả sequential và parallel checks pass, đồng thời output format vẫn giữ nguyên.

Nếu không reproduce được, chạy trực tiếp `dotnet run --project starter -- --reproduce`; reproduction dùng synchronization points để tránh phụ thuộc vào tốc độ máy.

## Estimated Time

Khoảng 55 phút.