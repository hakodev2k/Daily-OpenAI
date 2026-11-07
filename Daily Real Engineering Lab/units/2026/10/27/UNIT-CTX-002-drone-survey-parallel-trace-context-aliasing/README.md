# UNIT-CTX-002 — Drone Survey: Audit nhầm tile khi xử lý song song

## Mục tiêu
Điều tra lỗi correlation của pipeline C#/.NET khi nhiều ảnh tile được kiểm tra đồng thời. Phân biệt tính đúng đắn của từng bản ghi audit với số lượng bản ghi được tạo.

## Bối cảnh thực tế
Một dịch vụ kiểm định ảnh drone nhận một survey gồm nhiều tile. Từng tile được chuẩn bị song song, sau đó cả batch đi qua một điểm đồng bộ trước khi ghi audit. Dịch vụ vẫn tạo đủ số bản ghi nhưng dashboard đôi lúc gắn kết quả kiểm định với tile khác, làm sai lịch sử truy vết.

## Bạn cần làm gì
- Chạy pipeline và ghi lại correlation thực tế của mỗi tile.
- Xác định những giả thuyết có thể giải thích lỗi chỉ xuất hiện khi chạy nhiều tile.
- Sửa code trong thư mục starter/TraceLab mà không bỏ bước đồng bộ batch.
- Chứng minh cả case một tile và nhiều tile đều đúng, không chỉ làm giảm tỷ lệ lỗi.

## Yêu cầu môi trường
- .NET 8 SDK hoặc SDK tương thích có thể build net8.0, .NET 8 runtime.
- PowerShell 7+ hoặc Windows PowerShell; không cần Docker, database hay dịch vụ trả phí.
- Không dùng NuGet package ngoài .NET SDK.

## Chạy nhanh
Tại thư mục unit:

~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

Hoặc dùng dotnet CLI:

~~~powershell
dotnet run --project starter/TraceLab/TraceLab.csproj -- reproduce
~~~

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên starter chưa chỉnh sửa. Exit code 0 nghĩa là triệu chứng mong đợi đã được tái hiện; không có nghĩa pipeline đúng. Chạy verify.ps1 trước khi sửa để thấy quality gate thất bại (exit code 1). Sau đó sửa starter và chạy lại verify.ps1.

## Những gì cần quan sát
- Số audit rows vẫn bằng số tile đầu vào.
- Case single-tile và các case parallel có thể có kết quả correlation khác nhau.
- Trong parallel, so sánh expected và audit cho từng dòng, không dựa vào thứ tự output.
- Giá trị tile cuối cùng xuất hiện trong audit có thể thay đổi giữa các lần chạy; bất biến cần kiểm tra là mỗi row khớp tile tương ứng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence vào workspace/my-investigation.md.
3. Thử fix trong starter.
4. Verify bằng verify.ps1, bao gồm các case parallel.
5. Chỉ sau đó mới xem reference solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi đã tự thử sửa và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
35–45 phút (L1 / D3).
