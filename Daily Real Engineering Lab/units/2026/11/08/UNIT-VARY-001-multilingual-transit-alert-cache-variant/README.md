# UNIT-VARY-001 — Bản tin giao thông hiển thị sai ngôn ngữ sau khi bật cache

## Mục tiêu
Refactor một cache key contract của hệ thống bản tin công cộng, giữ đúng tính độc lập của các biến thể nội dung và kiểm chứng bằng regression tests.

## Bối cảnh thực tế
Cổng giao thông đô thị phục vụ bản tin cùng một URL cho tiếng Việt và tiếng Anh. Sau khi tối ưu bộ nhớ đệm, một số hành khách nhận bản tin khác ngôn ngữ dù request hợp lệ. Hiện tượng phụ thuộc thứ tự request.

## Bạn cần làm gì
Chạy harness, ghi ít nhất hai giả thuyết, thu thập kết quả của hai thứ tự request, sửa duy nhất code trong `starter/CacheKeyBuilder.cs` mà không đổi public API, rồi chạy verify.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+ hoặc Windows PowerShell; không cần Redis hay network.

## Chạy nhanh
`./run.ps1` hoặc `dotnet run --project starter/TransitLab.csproj -- inspect`.

## Cách reproduce vấn đề
`./reproduce.ps1`. Lưu output cho cả hai thứ tự ngôn ngữ và phân biệt observed với expected.

## Những gì cần quan sát
Kiểm tra ngôn ngữ nội dung trả về, cache hit/miss, số mục trong cache, và tác động của thứ tự request. Không suy luận nguyên nhân chỉ từ một request.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) chỉ đọc sau khi tự sửa.

## Expected Results
Starter: ít nhất một request nhận sai ngôn ngữ. Sau sửa: hai thứ tự request đều trả đúng ngôn ngữ, cache có hai biến thể, cùng biến thể được tái sử dụng.

## Estimated Time
65 phút.
