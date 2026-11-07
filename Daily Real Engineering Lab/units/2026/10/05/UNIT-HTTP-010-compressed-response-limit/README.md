# UNIT-HTTP-010 — Partner export vượt giới hạn dù response header trông nhỏ

## Mục tiêu

Điều tra một HTTP client nhận response nén từ partner. Guard hiện tại dựa vào metadata response nhưng process vẫn materialize nhiều MB vào memory, vượt policy 1 MB.

Bạn cần reproduce, thu evidence ở các boundary của HTTP response, đưa hypothesis, sửa learner code và verify policy được enforce trên dữ liệu thực sự mà application tiêu thụ.

## Bối cảnh thực tế

Một worker tải export từ partner. Partner bật gzip để giảm bandwidth. Team đã thêm giới hạn response nhằm bảo vệ memory, nhưng sau deployment worker vẫn có allocation spike với một số export có tỷ lệ nén rất cao.

Request thành công, response không phải malformed, và network transfer nhỏ hơn payload mà application cuối cùng nhìn thấy.

## Bạn cần làm gì

1. Chạy fixture server.
2. Chạy starter client và ghi evidence.
3. Đưa ra ít nhất hai hypothesis.
4. Sửa `starter/Client/Program.cs`; không sửa fixture để làm payload nhỏ đi.
5. Policy phải chặn response khi dữ liệu application tiêu thụ vượt 1,000,000 bytes.
6. Chạy `verify.ps1`.
7. Chỉ sau đó xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Port 5088 local còn trống

## Chạy nhanh

Terminal 1:

~~~powershell
./run-server.ps1
~~~

Terminal 2:

~~~powershell
./reproduce.ps1
~~~

Sau khi sửa:

~~~powershell
./verify.ps1
~~~

## Cách reproduce vấn đề

`reproduce.ps1` build và chạy learner client chống lại fixture response nén có deterministic content.

## Những gì cần quan sát

- metadata response mà client nhìn thấy
- số bytes thực tế được materialize
- thời điểm policy hiện tại được kiểm tra
- boundary nào đang được đo và boundary nào chưa được đo

Không thay payload fixture, không tăng limit và không disable compression để né yêu cầu.

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

[Spoiler — chỉ xem sau khi đã reproduce và tự thử sửa](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

40–60 phút.
