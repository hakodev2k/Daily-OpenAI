# UNIT-AZURE-007 — Token Works, API Still Returns 401

## Mục tiêu
Điều tra một lỗi authentication service-to-service nơi bước lấy access token thành công nhưng downstream API vẫn trả `401 Unauthorized`.

## Bối cảnh thực tế
Một worker xử lý hóa đơn được chuyển sang mô hình tương tự Managed Identity. Log cho thấy token được cấp thành công và request có `Authorization: Bearer ...`, nhưng Invoice API từ chối request. Team nghi ngờ token hết hạn, lỗi HTTP client hoặc API bị down.

Lab dùng identity provider và API giả lập local để tập trung vào contract authentication, không cần Azure subscription.

## Bạn cần làm gì
1. Reproduce lỗi.
2. Thu thập evidence từ output của identity provider và API.
3. Ghi ít nhất hai hypothesis.
4. Sửa `starter/` để request được API chấp nhận mà không nới lỏng validation phía API.
5. Chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy starter với dữ liệu deterministic và chỉ thành công khi triệu chứng `401` được tái hiện đúng.

## Những gì cần quan sát
- Identity provider có cấp token hay không.
- Metadata nào của token được API quan sát.
- HTTP status cuối cùng.
- Có hay không dấu hiệu token hết hạn hoặc request thiếu Bearer token.

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
[Spoiler — chỉ xem sau khi đã tự điều tra](solution/README.md)

## Expected Results
Trước fix: token acquisition thành công nhưng API trả `401`. Sau fix: `./verify.ps1` in `VERIFY_PASS`; API vẫn giữ nguyên validation contract.

## Estimated Time
45 phút.
