# Before — starter chưa sửa

`./reproduce.ps1`: exit 0, `REPRODUCED`, hai pha đều chạy một lần, caller thấy `ArchiveFinalizeException` trong tình huống `both`.

`./verify.ps1`: exit khác 0; fixture `both` FAIL; ba fixture còn lại PASS. Không dựa vào thời gian chạy.

Nếu reproduce không khớp, kiểm tra SDK .NET 8, chắc chắn chưa sửa `ArchiveExporter` và không thay fixture.
