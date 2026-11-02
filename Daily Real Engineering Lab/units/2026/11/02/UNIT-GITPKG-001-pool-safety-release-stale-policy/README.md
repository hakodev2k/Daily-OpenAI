# UNIT-GITPKG-001 — Bản phát hành cổng kiểm định hồ bơi chứa policy đã bị loại bỏ

## Mục tiêu
Điều tra sự khác biệt giữa Git revision đã duyệt và tập file thực sự được đưa vào release artifact. Tái hiện, thu thập bằng chứng, sửa pipeline đóng gói và viết negative regression test.

## Bối cảnh thực tế
Cổng kiểm định an toàn hồ bơi của thành phố vừa thay thế bộ quy tắc cũ. Git history và pull request đều cho thấy file policy cũ đã bị xóa, nhưng một bản release lại chứa thêm các policy không thuộc revision hiện tại. Production có nguy cơ tải nhầm quy tắc. Nhóm CI dùng runner có workspace tái sử dụng giữa các job.

## Bạn cần làm gì
1. Tái hiện sự cố bằng repository Git cục bộ do script tạo; không sử dụng tài khoản cloud.
2. So sánh Git tree, working tree, package manifest và thư mục artifact.
3. Viết ít nhất ba hypothesis, xác định bằng chứng phân biệt.
4. Sửa duy nhất `starter/Package.ps1` (hoặc luồng đóng gói tương đương) để artifact chỉ chứa nội dung được phê duyệt.
5. Kiểm chứng cả trường hợp xuất hiện một file lạ mới sau lần setup.

## Yêu cầu môi trường
- Windows PowerShell 5.1 hoặc PowerShell 7+.
- Git CLI có trong PATH.
- Không cần .NET, Docker, Azure hay dịch vụ trả phí.
- Mọi thao tác tạo/xóa test workspace nằm trong `.lab-work/` của unit.

## Chạy nhanh
Từ thư mục unit:
```powershell
./reproduce.ps1
```
Sau khi chỉnh sửa:
```powershell
./setup.ps1
./verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` tạo hai Git commit giả lập, tái tạo một số file còn sót từ job trước, đóng gói bằng starter và kiểm tra dấu hiệu sai lệch. Script trả exit thành công **khi phát hiện đúng lỗi dự kiến**, không phải khi bản release đã đúng.

## Những gì cần quan sát
- `git -C .lab-work ls-files -- release/*.json` so với `git -C .lab-work status --short`.
- `.lab-work/artifact/manifest.json` và `.lab-work/artifact/package/`.
- File nào thuộc commit hiện tại, file nào chỉ tồn tại trong workspace?
- Vì sao smoke test chỉ kiểm tra một policy hợp lệ có thể bỏ sót lỗi?
- Artifact có thay đổi nếu một file không liên quan xuất hiện trong workspace không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution và phân tích trade-offs](solution/README.md). Chỉ mở sau khi tự thử.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
60–75 phút (L3 / D4).
