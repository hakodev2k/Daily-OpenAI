# UNIT-JSON-002 — Hồ sơ thông quan đổi quyết định kiểm tra dù API vẫn xử lý thành công

## Mục tiêu
Điều tra sự sai lệch giữa payload từ đối tác và quyết định nghiệp vụ sau khi .NET parse JSON; thu thập bằng chứng, đề xuất hợp đồng ingress rõ ràng và viết regression tests cho các đường lỗi.

## Bối cảnh thực tế
Một cổng thông quan tiếp nhận tờ khai từ nhiều hãng vận chuyển. Trường `requiresInspection` quyết định có đưa lô hàng vào luồng kiểm tra hay không. Trong đợt đối soát, cùng một hồ sơ hiển thị trạng thái cần kiểm tra ở phía đối tác nhưng phía tiếp nhận lại đánh dấu không cần kiểm tra. API không báo lỗi; log chỉ có kết quả nghiệp vụ. Toàn bộ dữ liệu lab là synthetic.

## Bạn cần làm gì
1. Chạy starter và lưu hai payload cùng kết quả parse.
2. Viết ít nhất ba giả thuyết (dữ liệu đầu vào, mapping, business rule, logging).
3. Điều tra ranh giới ingress, mô tả chính xác invariant mà API cần bảo vệ.
4. Chỉ sửa `starter/ManifestIntake.cs`; giữ nguyên `Program.cs` và scripts.
5. Không phá backward compatibility của các payload hợp lệ khác kiểu viết hoa/thường.
6. Chạy `verify.ps1` trên code của bạn trước khi đọc solution.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 5.1+ hoặc 7+. Không cần Docker, NuGet bổ sung, tài khoản cloud hay dữ liệu thật.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 và `REPRODUCTION_CONFIRMED` nghĩa là đã quan sát đúng triệu chứng starter, không có nghĩa hệ thống đã được sửa.

## Những gì cần quan sát
- Hai request chứa những thông tin nào và kết quả quyết định có giống nhau không?
- HTTP 2xx hoặc parse thành công chứng minh được điều gì, và không chứng minh được điều gì?
- Log cần lưu thêm loại bằng chứng nào mà không ghi thông tin nhạy cảm?
- Khi sửa, payload hợp lệ nào phải tiếp tục được chấp nhận?

Xem [incident evidence](evidence/incident-timeline.md) và ghi kết quả vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ đọc sau khi tự điều tra và sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút · L2 / D4 · Validation: static-review (chưa chạy .NET build/tests trong môi trường tạo lab).
