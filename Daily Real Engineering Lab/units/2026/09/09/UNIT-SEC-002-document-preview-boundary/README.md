# UNIT-SEC-002 — Document Preview Boundary

## Mục tiêu

Điều tra một lỗ hổng trust-boundary trong dịch vụ preview tài liệu local, thu thập evidence từ hai đường dẫn có quan hệ khác nhau và sửa validation sao cho chỉ file thực sự nằm trong thư mục được phép mới có thể được đọc.

## Bối cảnh thực tế

Một công cụ support nội bộ cho phép agent preview file đã upload. Dịch vụ nhận đường dẫn đã được normalize và có một bước kiểm tra rằng file thuộc khu vực upload được phép. Trong một audit, team phát hiện một file thuộc khu vực archive vẫn có thể được preview dù archive không nằm trong phạm vi support được phép truy cập.

Business impact: dữ liệu ngoài phạm vi phân quyền có thể bị đọc dù ứng dụng vẫn ghi nhận request là hợp lệ.

## Bạn cần làm gì

1. Chạy starter và xác nhận file bình thường được đọc thành công.
2. Reproduce case file nằm ở một thư mục sibling có tên gần giống thư mục hợp lệ.
3. Ghi lại canonical path và kết quả access check cho cả hai case.
4. Xác định invariant mà boundary check phải chứng minh.
5. Sửa `starter/` mà không hard-code tên file hoặc sibling directory cụ thể.
6. Chạy `verify.ps1` cho đến khi case hợp lệ vẫn pass và case ngoài boundary bị reject.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần database, Docker hoặc cloud service

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script build và chạy starter. Starter tự tạo sandbox dưới thư mục temp với một thư mục được phép và một thư mục sibling, sau đó thử preview một file ở mỗi vị trí.

## Những gì cần quan sát

- canonical path của vùng được phép
- nội dung trả về ở Case A
- nội dung trả về ở Case B
- exit code của `reproduce.ps1`
- quan hệ filesystem thực tế giữa target path và allowed root

Không kết luận chỉ dựa trên việc hai chuỗi path có phần đầu giống nhau.

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

> **Spoiler:** chỉ mở sau khi bạn đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- file hợp lệ được đọc
- case sibling ngoài phạm vi vẫn có thể trả về nội dung
- reproduction kết thúc với exit code khác 0

Sau khi sửa:

- file hợp lệ vẫn được đọc
- file ngoài vùng được phép bị reject bằng `UnauthorizedAccessException`
- verification pass mà không hard-code path cụ thể của attack case

## Estimated Time

30–50 phút.
