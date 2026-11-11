# UNIT-KEYRETIRE-001 — Bản sao lưu kho lưu trữ không thể phục hồi sau khi xoay vòng khóa

## Mục tiêu
Điều tra một tình huống disaster recovery có dữ liệu sao lưu còn nguyên nhưng không thể khôi phục; đánh giá policy và chứng minh quyết định bằng replay.

## Bối cảnh thực tế
Kho hồ sơ hành chính khu vực lưu backup mã hóa theo phiên bản khóa. Nhóm vận hành luân phiên khóa hằng tháng. Sau đợt bảo trì, một số backup vẫn vượt qua kiểm tra checksum nhưng thử khôi phục thất bại.

## Bạn cần làm gì
Chạy replay, ghi nhận số backup còn trong retention, số backup khôi phục được và bằng chứng phụ thuộc giữa backup với phiên bản khóa. Đề xuất thay đổi chính sách bảo trì và kế hoạch rollout có rollback.

## Yêu cầu môi trường
.NET SDK 8.0 và PowerShell 7+ (hoặc chạy lệnh dotnet tương đương). Không cần Azure account; dữ liệu hoàn toàn giả lập.

## Chạy nhanh
`dotnet run --project starter/ArchiveReplay.csproj`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; quan sát kết quả của ngày giả lập 70. Không chỉnh sửa dữ liệu đầu vào trước lần chạy đầu.

## Những gì cần quan sát
Đối chiếu checksum, thời hạn retention, key version và khả năng restore. Ghi nhận trường hợp checksum tốt nhưng restore thất bại.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong workspace/investigation.md.
3. Thử fix trên starter/Program.cs.
4. Chạy ./verify.ps1 và diễn giải trade-off.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
Reproduce báo 2 backup không restore được dù checksum hợp lệ; verify chỉ pass khi mọi backup còn hiệu lực đều có khóa tương ứng. Đánh giá thêm thời gian phục hồi, chi phí giữ khóa và quyền truy cập.

## Estimated Time
75 phút.
