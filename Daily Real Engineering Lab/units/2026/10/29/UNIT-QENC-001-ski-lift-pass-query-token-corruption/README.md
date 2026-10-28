# UNIT-QENC-001 — Vé cáp treo hợp lệ bị đối tác từ chối theo từng mã

## Mục tiêu

Điều tra lỗi tích hợp HTTP xảy ra có chọn lọc; thu thập bằng chứng ở ranh giới client–partner, sửa đường đi dữ liệu và bảo vệ contract bằng regression tests.

## Bối cảnh thực tế

Một khu trượt tuyết phát hành vé cáp treo điện tử. Ứng dụng quét vé gọi partner validation API. Phần lớn vé hoạt động, nhưng một số vé vừa phát hành bị trả về trạng thái không hợp lệ. Đội vận hành nghi ngờ vé hết hạn, lệch đồng hồ hoặc lỗi dữ liệu ở partner. Không có dịch vụ bên ngoài nào cần thiết cho lab.

## Bạn cần làm gì

1. Chạy bản starter và ghi lại triệu chứng.
2. Đối chiếu mã vé tại nguồn, URI request và mã mà partner giả lập tiếp nhận.
3. Viết ít nhất hai hypothesis và chọn một phép kiểm chứng có thể bác bỏ từng hypothesis.
4. Sửa **starter/PartnerClient.cs** mà không thay đổi danh tính vé hay partner simulator.
5. Chạy verify, so sánh với reference solution, giải thích tác động production.

## Yêu cầu môi trường

.NET 8 SDK/runtime (hoặc SDK mới hơn với runtime roll-forward phù hợp), PowerShell 7 hoặc Windows PowerShell, không cần NuGet ngoài SDK.

## Chạy nhanh

Từ thư mục unit:

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

Chạy `./reproduce.ps1`. Script build starter rồi chỉ thành công khi chứng minh được mismatch dự kiến. Trước khi sửa, `./verify.ps1` phải thất bại; sau khi sửa, nó phải thành công.

## Những gì cần quan sát

So sánh dữ liệu tại ba vị trí: mã được phát hành, URI được gửi, giá trị partner đọc được. Ghi lại trường hợp hoạt động và trường hợp bị từ chối; không dựa riêng vào mã lỗi 401 để suy luận nguyên nhân.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng regression cases.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra:** [Reference Solution](solution/README.md).

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

50 phút. Ghi điều tra tại [workspace/my-investigation.md](workspace/my-investigation.md).
