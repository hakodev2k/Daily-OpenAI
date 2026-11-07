# UNIT-REVAL-001 — Rare-book catalog: bản ghi trống sau cache eviction

## Mục tiêu
Điều tra một lỗi HTTP cache revalidation làm catalog đôi khi trả nội dung rỗng dù upstream vẫn khỏe; dùng bằng chứng request/response để xác nhận tính đúng đắn sau khi sửa.

## Bối cảnh thực tế
Dịch vụ tra cứu sách quý của một thư viện số truy vấn catalog từ đối tác. Client giữ bản sao phản hồi để giảm băng thông. Sau đợt memory pressure, dashboard đôi khi hiện danh mục trống. Sự cố chỉ xảy ra khi cache bị thu hồi một phần; request không bị timeout và upstream vẫn phản hồi nhanh.

## Bạn cần làm gì
1. Chạy fixture và ghi lại chuỗi HTTP status, request headers và dữ liệu client nhận được.
2. Đưa ra ít nhất hai giả thuyết về trạng thái cache, upstream và hợp đồng HTTP.
3. Chỉnh sửa **chỉ** file \`starter/CatalogClient.cs\` để dữ liệu không bị mất sau eviction, vẫn giữ lợi ích revalidation và không âm thầm chấp nhận phản hồi không thể sử dụng.
4. Chạy verify để kiểm tra cả nhánh cache hit, eviction, thay đổi phiên bản và phản hồi upstream bất thường.

## Yêu cầu môi trường
- .NET SDK 8.x (khuyến nghị 8.0.100 trở lên), PowerShell 5.1+ hoặc PowerShell 7.
- Không cần Azure, database, NuGet bên ngoài hay internet lúc chạy. Fixture sử dụng \`HttpMessageHandler\` giả lập.

## Chạy nhanh
Tại thư mục unit:

~~~powershell
.\reproduce.ps1
~~~

Nếu đang dùng bash và đã cài .NET SDK:

~~~bash
cd starter
dotnet run --configuration Release -- --reproduce
~~~

## Cách reproduce vấn đề
Chạy lệnh trên với starter nguyên bản. Fixture thực hiện một chuỗi request có bước memory pressure giữa các lần đọc. Exit code 0 của \`reproduce.ps1\` nghĩa là **đã xác nhận triệu chứng của starter**, không có nghĩa là hệ thống đã đúng.

## Những gì cần quan sát
- Kích thước payload trả về trước và sau memory pressure.
- Thứ tự status 200/304 và việc request có mang conditional header hay không.
- Catalog upstream có thực sự đổi nội dung hay không.
- Khi sửa xong, chạy \`.\verify.ps1\`: phải có kết quả đúng sau eviction, vẫn tận dụng 304 khi có dữ liệu hợp lệ và không che giấu 304 bất thường.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng \`.\verify.ps1\`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ đọc sau khi đã tự sửa và verify: [Giải thích và mã tham chiếu](solution/README.md).

## Expected Results
**Before:** fixture báo REPRODUCED, chỉ ra lần đọc thứ hai không giữ được bản catalog mặc dù upstream không đổi. **After:** verify báo VERIFIED cho bốn hợp đồng: eviction, revalidation, cập nhật bản mới và phản hồi 304 bất thường. Kết quả không phụ thuộc tốc độ máy.

## Estimated Time
40–55 phút. Ghi evidence và hypotheses vào [workspace](workspace/my-investigation.md).
