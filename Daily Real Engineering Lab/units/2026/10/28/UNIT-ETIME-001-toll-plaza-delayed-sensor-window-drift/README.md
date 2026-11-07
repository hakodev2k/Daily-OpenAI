# UNIT-ETIME-001 — Lượt xe theo khung 5 phút lệch dù tổng lượt qua trạm vẫn đúng

## Mục tiêu
Điều tra sai lệch thống kê lưu lượng theo cửa sổ 5 phút trong một pipeline telemetry .NET. Phân biệt bằng chứng về dữ liệu, thứ tự nhận và phép tổng hợp; đưa ra fix có regression cho ranh giới thời gian.

## Bối cảnh thực tế
Trạm thu phí dùng các cảm biến ghi lượt vào/ra. Tổng chênh lệch lượt trong ca vẫn đúng, nhưng dashboard theo từng 5 phút không khớp log gốc. Báo cáo phục vụ điều phối làn xe nên sai số theo khung thời gian có tác động vận hành. Dữ liệu mô phỏng có các bản tin đến muộn, nhưng không yêu cầu kết nối thiết bị thật.

## Bạn cần làm gì
1. Chạy mẫu, lưu bảng các khung thời gian và tổng cuối ca.
2. Viết ít nhất ba hypothesis: lỗi tính toán, lỗi timezone, lỗi thứ tự/độ trễ dữ liệu.
3. Đối chiếu bằng chứng từ fixture, output và snapshot; bác bỏ hypothesis không phù hợp.
4. Sửa **chỉ** \`starter/WindowRollup.cs\` theo contract: các cửa sổ UTC 5 phút dạng [start, end), không làm mất sự kiện và giữ giá trị tổng.
5. Chạy verification cho cả ca cao điểm lẫn ca có timestamp offset và sự kiện đúng ranh giới.
6. Đề xuất cách xử lý báo cáo đã công bố nếu sự kiện đến muộn sau thời điểm chốt (không cần triển khai persistence).

## Yêu cầu môi trường
.NET SDK 10.x, PowerShell 7 hoặc Windows PowerShell. Không cần Docker, database, cloud hay NuGet package bên ngoài.

## Chạy nhanh
Trong thư mục unit:
\`\`\`powershell
./run.ps1
./reproduce.ps1
./verify.ps1
\`\`\`
Hoặc: \`dotnet run --project starter/GateWindowLab.csproj -- --inspect\`.

## Cách reproduce vấn đề
Giữ nguyên starter, chạy \`./reproduce.ps1\` (exit 0 khi tái hiện đúng triệu chứng). Chạy \`./verify.ps1\` để ghi baseline đỏ (exit khác 0 trước khi sửa). Không mở thư mục solution trước khi hoàn tất hypothesis.

## Những gì cần quan sát
- Có khung nào lệch dù tổng toàn bộ vẫn khớp?
- Sai lệch tập trung gần ranh giới 5 phút hay trải đều?
- Các event cùng một ca có thứ tự thời gian và thời điểm nhận giống nhau không?
- Timestamp có offset khác nhau có chỉ cùng một thời điểm UTC không?
- CPU thấp, không có retry và không có event mất có loại trừ mọi lỗi logic hay không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong \`starter/\`.
4. Verify bằng \`./verify.ps1\`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md) · [Mã tham chiếu](solution/WindowRollup.cs). Chỉ mở sau khi tự thử.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md). [Incident snapshot](evidence/incident-snapshot.md).

## Estimated Time
65 phút — L3 Production Incident / D4. Không yêu cầu tài khoản dịch vụ trả phí.
