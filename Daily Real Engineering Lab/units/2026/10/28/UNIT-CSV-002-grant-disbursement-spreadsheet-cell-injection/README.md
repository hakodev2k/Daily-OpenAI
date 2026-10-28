# UNIT-CSV-002 — Báo cáo giải ngân CSV hiển thị công thức ngoài ý muốn

## Mục tiêu
Điều tra ranh giới giữa CSV serialization và cách spreadsheet diễn giải dữ liệu từ đối tác; sửa export pipeline mà vẫn giữ đúng dữ liệu, cấu trúc CSV và Amount.

## Bối cảnh thực tế
Một nền tảng giải ngân tài trợ phi lợi nhuận xuất CSV cho kế toán. Một số tên người thụ hưởng do đối tác nhập bị spreadsheet diễn giải như biểu thức, dù API trả HTTP 200, số cột đúng và tổng tiền không sai. Nhóm vận hành cần hotfix tại export boundary trước kỳ đối soát.

## Bạn cần làm gì
1. Chạy starter và ghi bằng chứng về các cell bị phân loại khác text thuần.
2. Đưa ít nhất ba hypothesis, phân biệt CSV syntax và spreadsheet semantics.
3. Sửa **starter/GrantCsvExporter.cs**; không thay đổi database hoặc numeric Amount.
4. Chạy verify trên starter đã sửa, kiểm tra comma, double quote, newline và Amount.
5. Chỉ sau đó mở reference solution và phân tích compatibility giữa các spreadsheet clients.

## Yêu cầu môi trường
.NET SDK 8.x (global.json), PowerShell. Không cần Excel, database, Docker, cloud hoặc external NuGet. **Không chạy nội dung công thức trong spreadsheet**; fixture là chuỗi kiểm thử.

## Chạy nhanh
~~~powershell
./run.ps1
./reproduce.ps1
# Sửa starter/GrantCsvExporter.cs
./verify.ps1
~~~
Nếu PowerShell script bị policy chặn, chạy `dotnet run --project ./starter/GrantExport.csproj -- reproduce` hoặc `-- verify` trực tiếp.

## Cách reproduce vấn đề
`reproduce.ps1` trả exit 0 khi **starter ban đầu** tái hiện triệu chứng. `verify.ps1` chạy **learner-edited starter**, không chạy reference solution. Sau khi fix, reproduce có thể không còn pass.

## Những gì cần quan sát
- CSV parse đúng records nhưng một số DisplayName vẫn bị phân loại formula-like.
- Field chứa comma, quote, newline có round-trip không?
- DisplayName có trust boundary khác Amount không?
- Thành phần nào quyết định ý nghĩa cell sau khi parse CSV?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Tự sửa starter.
4. Verify correctness và rủi ro.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ mở sau khi tự điều tra](solution/README.md).

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
60–75 phút. Có [Senior challenge](extensions/senior-challenge.md).
