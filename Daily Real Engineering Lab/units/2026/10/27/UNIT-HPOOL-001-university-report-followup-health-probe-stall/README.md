# UNIT-HPOOL-001 — Partner report preview stalls the follow-up health probe

## Mục tiêu
Điều tra HTTP outbound timeout: report preview trả headers thành công nhưng health probe kế tiếp timeout dù partner vẫn hoạt động. Phân biệt lỗi phía origin với lỗi resource management phía client.

## Bối cảnh thực tế
Cổng kiểm định chất lượng của trường đại học cần kiểm tra metadata của báo cáo lớn từ partner rồi xác nhận health endpoint. Dashboard báo report ready nhưng health probe thất bại; partner không ghi nhận outage.

## Bạn cần làm gì
1. Chạy starter, thu thập request counters và exception.
2. Viết hai giả thuyết trong workspace/my-investigation.md; đối chiếu evidence.
3. Sửa starter/PartnerClient.cs; không sửa fake server hoặc assertions.
4. Chạy verify và chứng minh report lẫn health đều tới partner, response là PONG.
5. Chỉ sau đó so sánh với reference solution.

## Yêu cầu môi trường
.NET SDK 8.0.x; PowerShell 7+ hoặc dotnet CLI; không cần Docker, Azure, NuGet ngoài. Fake partner chỉ bind loopback 127.0.0.1 trên cổng ngẫu nhiên.

## Chạy nhanh
~~~powershell
./reproduce.ps1
./verify.ps1
~~~
Hoặc:
~~~bash
dotnet run --project starter/PartnerProbe.csproj -- reproduce
dotnet run --project starter/PartnerProbe.csproj -- verify
~~~

## Cách reproduce vấn đề
reproduce.ps1 khởi động partner mô phỏng report có body đang truyền rồi gọi health với deadline ngắn. Khi in EXPECTED FAILURE và exit 0, triệu chứng đã tái hiện. Nếu không tái hiện, xem evidence/incident.md.

## Những gì cần quan sát
- Report đã tới partner chưa?
- Health đã tới origin trước deadline chưa?
- Timeout xuất hiện sau khi nhận headers hay trong khi kết nối?
- Có bằng chứng partner thật sự chậm không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify và kiểm tra contract.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md), chỉ đọc sau khi tự điều tra.

## Expected Results
Trước: reproduce in EXPECTED FAILURE, verify trả nonzero. Sau fix: verify in PASS; reportRequests=1, healthRequests=1, body PONG. Không dựa vào timing tuyệt đối giữa các máy.

## Estimated Time
45–60 phút, L3 / D4.
