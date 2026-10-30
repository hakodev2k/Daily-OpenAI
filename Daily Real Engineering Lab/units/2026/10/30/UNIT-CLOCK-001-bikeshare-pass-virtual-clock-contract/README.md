# UNIT-CLOCK-001 — Vé xe đạp bị từ chối trong bài kiểm thử thời gian

## Mục tiêu

Điều tra sự sai khác giữa thời gian mô phỏng trong test và quyết định xác thực vé; đưa logic về một hợp đồng thời gian có thể kiểm chứng, không phụ thuộc chờ đợi thực tế.

## Bối cảnh thực tế

Dịch vụ bike-sharing cấp vé mở khóa có hiệu lực trong một khoảng UTC. Nhóm QA chạy lại kịch bản tại các mốc sát giờ bắt đầu và hết hạn bằng một đồng hồ mô phỏng. Bản build mới trả về `NotYetValid` ngay cả khi kịch bản được đặt trong khoảng hiệu lực. Chạy thử theo thời gian thực lại khó tái hiện.

## Bạn cần làm gì

1. Chạy starter, ghi nhận quyết định ở sáu mốc thời gian.
2. Phân biệt lỗi so sánh ranh giới, sai múi giờ và sai nguồn thời gian.
3. Viết hypothesis và bằng chứng trước khi chỉnh `starter/PassGate.cs`.
4. Giữ nguyên hợp đồng `[ValidFromUtc, ExpiresAtUtc)`, sửa để test có thể kiểm chứng bằng đồng hồ được cấp.
5. Chạy `verify.ps1` và đánh giá ảnh hưởng tới môi trường production.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK (không cần Docker, NuGet bên ngoài, Azure hay đồng hồ hệ thống giả).

## Chạy nhanh

Từ thư mục unit:

```powershell
.\setup.ps1
.\reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
.\run.ps1
.\reproduce.ps1
.\verify.ps1
```

`reproduce.ps1` thành công khi phát hiện đúng triệu chứng ban đầu; `verify.ps1` dự kiến thất bại trước khi sửa và thành công sau khi bạn sửa `starter/`. Cả hai chạy qua `harness/` liên kết trực tiếp tới `starter/PassGate.cs`, không dùng code trong `solution/`.

## Những gì cần quan sát

- Giá trị `DiagnosticNowUtc` và quyết định `Evaluate` có phản ánh cùng mốc mô phỏng không?
- Các quyết định trước giờ bắt đầu, ngay trước giờ hết hạn, đúng giờ hết hạn và khi quay đồng hồ trở lại có nhất quán không?
- Mã thoát của hai script khác nhau như thế nào?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và thử sửa.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time

55–75 phút. Bài tập ở mức L4 / D4, tập trung refactoring và verification dựa trên bằng chứng.
