# UNIT-TEST-013 — Test pass local nhưng fail trên CI Linux

## Mục tiêu

Điều tra một test .NET deterministic theo từng environment nhưng cho kết quả khác nhau giữa máy developer và CI.

Bạn cần thu thập evidence về process environment, xác định assumption ẩn trong production code, sửa learner-editable code và verify dưới hai culture khác nhau.

## Bối cảnh thực tế

Một billing service tạo chuỗi ngày để gửi sang legacy partner. Test pass trên laptop của team nhưng fail sau khi pipeline chuyển sang Linux runner mới.

Input giống nhau, timezone không liên quan và không có network call.

## Bạn cần làm gì

1. Chạy `reproduce.ps1`.
2. Ghi evidence về culture và actual output.
3. Đưa ra ít nhất ba hypotheses.
4. Sửa `starter/PartnerDateFormatter.cs`.
5. Không sửa tests hoặc scripts để né failure.
6. Chạy `verify.ps1`.
7. Sau đó mới xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

~~~powershell
./reproduce.ps1
~~~

Sau khi sửa code:

~~~powershell
./verify.ps1
~~~

## Cách reproduce vấn đề

Script chạy cùng test harness hai lần với hai culture khác nhau. Starter code chỉ đáp ứng contract ở một environment.

## Những gì cần quan sát

- current culture
- cùng một input date
- actual string ở từng run
- partner contract yêu cầu format nào
- failure thuộc timezone, locale hay dữ liệu

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

[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

30–45 phút.
