# UNIT-WIRE-001 — Gateway RFID cảng từ chối khung dữ liệu niêm phong hợp lệ

## Mục tiêu
Điều tra một lỗi tương thích binary protocol qua bằng chứng wire bytes, phân biệt lỗi truyền tải với lỗi diễn giải khung dữ liệu, và xây dựng regression test cho cả đầu vào hợp lệ lẫn không hợp lệ.

## Bối cảnh thực tế
Thiết bị RFID tại cổng container gửi bản tin ngắn tới một gateway .NET. Sau khi cập nhật adapter, thiết bị vẫn gửi frame theo hợp đồng nhưng một số sự kiện không được ghi nhận. Log báo payload quá giới hạn, trong khi bản tin thực tế chỉ vài byte. Nhóm vận hành nghi ngờ firmware, mất byte trên TCP hoặc giới hạn payload.

## Bạn cần làm gì
1. Chạy starter và ghi lại bytes của header, độ dài gateway báo cáo và kích thước payload thực tế.
2. Đưa ra ít nhất hai hypothesis; chỉ ra phép thử phân biệt chúng.
3. Sửa parser trong `starter/Program.cs` mà không thay đổi wire contract hay các test vector.
4. Chạy verify, kiểm tra các frame hợp lệ và các frame không hợp lệ vẫn bị từ chối.

## Yêu cầu môi trường
- .NET SDK 8.x và PowerShell (Windows PowerShell hoặc PowerShell 7).
- Không cần Docker, NuGet bên ngoài, thiết bị thật hay tài khoản cloud.

## Chạy nhanh
Từ thư mục unit:

~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
`./reproduce.ps1` chạy starter và **thành công khi tái hiện được việc từ chối một frame hợp lệ**. Đây không phải kiểm tra bản sửa. Sau khi tự sửa, chạy `./verify.ps1`; script này luôn kiểm tra `starter/`, không chạy `solution/`.

## Những gì cần quan sát
- Header `00-04` và payload ASCII `PING`.
- Giá trị `declared` so với `actual` trong log.
- Kết quả khi đổi header nhưng giữ nguyên payload.
- Sau sửa: 3 frame hợp lệ được chấp nhận và 3 frame không hợp lệ bị từ chối.

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
**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi đã thử sửa và verify.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
45 phút. Ghi lại suy luận trong [workspace](workspace/my-investigation.md).
