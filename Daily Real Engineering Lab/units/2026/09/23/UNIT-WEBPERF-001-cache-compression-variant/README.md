# UNIT-WEBPERF-001 — Compression Cache Variant

## Mục tiêu

Điều tra một lỗi chỉ xuất hiện sau khi response đi qua shared cache/CDN simulation và các client có khả năng hỗ trợ compression khác nhau.

## Bối cảnh thực tế

Public product catalog vừa bật response compression và edge caching. Phần lớn request hoạt động, nhưng một nhóm client thỉnh thoảng nhận response không đọc được sau khi một client khác truy cập cùng URL trước đó. Origin vẫn khỏe và payload business không đổi.

## Bạn cần làm gì

1. Chạy starter và reproduce theo hai thứ tự client được cung cấp.
2. So sánh request headers, response headers, cache HIT/MISS và bytes trả về.
3. Ghi ít nhất hai hypothesis vào `workspace/my-investigation.md`.
4. Sửa learner-editable code/config trong `starter/` mà không tắt caching hoặc compression.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script chạy một cache simulation local, gửi cùng resource qua hai client profile khác nhau và in timeline HIT/MISS cùng response metadata.

## Những gì cần quan sát

- Request nào populate cache trước.
- Response metadata của cache HIT khác gì cache MISS.
- Cùng URL có luôn đồng nghĩa cùng representation bytes không.
- Triệu chứng có phụ thuộc thứ tự request không.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

⚠️ Spoiler: [Reference Solution](solution/README.md)

## Expected Results

Before: kết quả phụ thuộc client nào populate shared cache trước. After: cả hai client profile luôn nhận representation tương thích, đồng thời cache và compression vẫn được sử dụng.

## Estimated Time

45 phút.
