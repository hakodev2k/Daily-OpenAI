# After

Sau khi tự sửa `starter/InspectionImportService.cs`, chạy `./verify.ps1`: ba case `PASS` và `VERIFY PASS (3/3)`. Sau import thành công, review phải được persist; sau exception, setting phải được restore; setting ban đầu disabled phải được giữ nguyên.
