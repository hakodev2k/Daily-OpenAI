# UNIT-JSONCASE-001 — Thiết bị bệnh viện gửi JSON hợp lệ nhưng kết quả cảnh báo bị sai

## Mục tiêu
Điều tra một sự cố hợp đồng JSON giữa gateway thiết bị và backend .NET; phân biệt dữ liệu đầu vào hợp lệ về cú pháp với dữ liệu hợp lệ về ngữ nghĩa; thiết kế regression test cho ranh giới dữ liệu.

## Bối cảnh thực tế
Gateway nhận bản tin từ nhiều firmware của thiết bị theo dõi nhiệt độ tủ thuốc. Một lô bản tin có cùng mã thiết bị nhưng nhiệt độ được backend ghi nhận khác dashboard thiết bị. Đội vận hành phát hiện cảnh báo nhiệt độ cao bị bỏ sót sau bản nâng cấp firmware.

## Bạn cần làm gì
1. Chạy starter và thu thập log bản tin, kết quả và exit code.
2. Viết ít nhất hai hypothesis, phân biệt lỗi firmware, mapping và cảnh báo.
3. Xác định điều kiện hợp đồng khiến bản tin cần bị từ chối thay vì diễn giải tùy ý.
4. Chỉnh sửa **starter/Program.cs**; không sửa oracle trong `starter/ContractChecks.cs`.
5. Chạy verify và ghi quyết định vận hành vào workspace.

## Yêu cầu môi trường
.NET SDK 8.0 (không cần Azure, Docker hay NuGet ngoài).

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Reproduce chỉ thành công khi chứng minh starter có hành vi sai; exit code 0 của script không có nghĩa ứng dụng đúng.

## Những gì cần quan sát
So sánh bản tin đơn trường, bản tin chứa hai cách viết trường, kết quả phân loại cảnh báo và lỗi trả về. Ghi lại vì sao một bản tin có thể parse thành công nhưng không nên được chấp nhận.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [reference solution](solution/README.md) sau khi tự sửa và verify.

## Expected Results
Trước: bản tin mâu thuẫn có thể được nhận mà không có lỗi; sau: bản tin hợp lệ được xử lý nhất quán và bản tin mơ hồ bị từ chối với thông điệp có thể giám sát.

## Estimated Time
45–60 phút.
