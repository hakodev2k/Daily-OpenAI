# UNIT-ZIPPATH-001 — Gói ảnh di sản ghi tệp ra ngoài thư mục nhập

## Mục tiêu
Tái hiện một sự cố nhập ZIP, thu thập bằng chứng về đường dẫn output, tìm nguyên nhân gốc và bảo vệ filesystem trust boundary bằng regression tests.

## Bối cảnh thực tế
Một dịch vụ số hóa di sản nhận ZIP ảnh và manifest từ đối tác. Sau khi worker báo nhập thành công, hệ thống phát hiện một file lạ ở vị trí không thuộc thư mục đích. Cùng batch đó, ảnh hợp lệ vẫn được xử lý. Đội vận hành phải điều tra trước khi tiếp tục nhập hàng loạt.

## Bạn cần làm gì
1. Chạy starter và ghi bằng chứng; phân biệt lỗi quyền truy cập, cleanup và xử lý tên entry.
2. Ghi tối thiểu ba hypotheses và phép thử bác bỏ từng giả thuyết.
3. Sửa `starter/ArchiveImportService.cs` mà không chỉnh test harness.
4. Chạy `verify.ps1`, bảo vệ negative cases và valid nested entries.
5. So sánh với reference solution sau khi đã tự giải.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET 8 SDK theo `global.json`. Không cần mạng, Docker, Azure hay database. Test chỉ tạo dữ liệu trong thư mục tạm và tự cleanup.

## Chạy nhanh
Tại thư mục unit, chạy:

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Trên starter gốc, `./reproduce.ps1` phải in `REPRODUCED` và đường dẫn tệp ngoài `extract/`. Exit code 0 ở đây nghĩa là **lỗi đã được tái hiện**, không phải code đã an toàn. Sau khi sửa, `reproduce.ps1` có thể thất bại như dự kiến; dùng `verify.ps1` để đánh giá fix.

## Những gì cần quan sát
- Đường dẫn thực tế của từng output so với thư mục đích.
- Một entry hợp lệ trước entry bất thường có bị ghi một phần không.
- File ngoài vùng nhập có xuất hiện lại trong lần chạy cô lập không.
- Positive case và negative case có hành vi khác nhau như kỳ vọng không.
- Các giả thuyết nào phù hợp với bằng chứng, giả thuyết nào bị loại.

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
**Spoiler:** [Reference Solution](solution/README.md), code tại `solution/ArchiveImportService.cs`. `verify.ps1` chỉ chạy code bạn chỉnh trong `starter/`.

## Expected Results
Starter: phát hiện file nằm ngoài thư mục nhập. Sau fix: `./verify.ps1` in `VERIFIED`, input nguy hiểm bị từ chối trước khi ghi, gói hợp lệ vẫn được nhập. Không phụ thuộc timing hoặc external services.

## Estimated Time
65 phút — L3 / D4.
