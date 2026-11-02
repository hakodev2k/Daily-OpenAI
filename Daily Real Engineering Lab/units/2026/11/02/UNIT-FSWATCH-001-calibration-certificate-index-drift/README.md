# UNIT-FSWATCH-001 — Chứng chỉ hiệu chuẩn không đồng bộ với cổng tra cứu

## Mục tiêu

Điều tra sai lệch giữa thư mục chứng chỉ và chỉ mục tra cứu .NET, thu thập bằng chứng, đề xuất cơ chế phục hồi, và kiểm chứng đầy đủ thêm/sửa/xóa.

## Bối cảnh thực tế

Một phòng kiểm định công nghiệp lưu chứng chỉ hiệu chuẩn dưới dạng tệp. Cổng tra cứu dùng chỉ mục in-memory. Sau đợt nhập dữ liệu, một chứng chỉ mới không xuất hiện, chứng chỉ đã thu hồi vẫn hiển thị trạng thái cũ, và chứng chỉ đã xóa vẫn được tìm thấy. Một số thông báo thay đổi đã được xử lý bình thường. Không có exception.

## Bạn cần làm gì

1. Chạy starter và ghi initialIndex, phase1Index, phase2Index, notificationsReceived, maintenanceTicks.
2. Viết ít nhất ba hypothesis và kiểm tra từng hypothesis bằng bằng chứng.
3. Chỉ sửa starter/CertificateIndexer.cs, giữ nguyên Program.cs và các scripts.
4. Bảo đảm chỉ mục khớp các tệp .cert thực tế sau mỗi maintenance tick, kể cả thay đổi không được thông báo; bỏ qua .tmp và không giữ bản ghi đã xóa.
5. Chạy verify.ps1 trước khi đọc reference solution.

## Yêu cầu môi trường

.NET SDK 8 trở lên; PowerShell 5.1 hoặc 7. Target net8.0, không dùng NuGet ngoài, database, Docker hoặc cloud. Chương trình dùng thư mục tạm và tự dọn dẹp.

## Chạy nhanh

    ./reproduce.ps1
    # Sửa starter/CertificateIndexer.cs
    ./verify.ps1

Hoặc chạy trực tiếp: dotnet run --project starter/CertificateIndexLab.csproj. Exit code 3 biểu thị lỗi ban đầu; exit code 0 biểu thị contract đạt.

## Cách reproduce vấn đề

reproduce.ps1 chạy starter với dữ liệu cố định và một chuỗi notification giả lập có thể lặp lại. Script thành công chỉ khi ứng dụng xuất LAB_RESULT=EXPECTED_FAILURE và exit code 3. Sau khi sửa, chạy verify.ps1.

## Những gì cần quan sát

- So sánh chỉ mục và thư mục thực tế qua hai pha cập nhật.
- Phân biệt process liveness với dữ liệu đúng; ghi nhận notificationsReceived và maintenanceTicks.
- Kiểm tra thêm mới, cập nhật, xóa, tệp tạm và tick lặp lại không có thay đổi.
- Tìm bằng chứng ở [incident notes](evidence/incident-notes.md) và starter/Program.cs.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ đọc sau khi tự điều tra:** [Reference Solution](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

50–75 phút. Ghi điều tra trong [workspace/my-investigation.md](workspace/my-investigation.md).
