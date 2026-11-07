# UNIT-SQLITE-001 — Đồng bộ thiết bị thành công nhưng lịch sử kiểm định biến mất

## Mục tiêu

Điều tra mất dữ liệu liên kết trong ứng dụng đồng bộ offline, thu thập bằng chứng SQL và refactor có regression contract.

## Bối cảnh thực tế

Nhân viên bảo trì máy bơm ở các trạm cấp nước dùng ứng dụng offline. Khi thiết bị được đổi tên, hệ thống đồng bộ metadata thành công, nhưng một số lịch sử Inspections biến mất. Không có exception; kiểm tra foreign key vẫn hợp lệ.

## Bạn cần làm gì

Xác định nguyên nhân, sửa tối thiểu để cập nhật nhãn thiết bị mà không mất lịch sử, đồng thời bảo đảm tạo thiết bị mới và đồng bộ lặp lại vẫn đúng.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK; NuGet access cho lần restore đầu. Không cần database server, Docker hoặc Azure.

## Chạy nhanh

~~~powershell
Set-Location 'Daily Real Engineering Lab/units/2026/11/02/UNIT-SQLITE-001-water-pump-offline-sync-inspection-loss'
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Trước sửa, script phải in REPRODUCED và exit code 0. Mỗi lần chạy tạo SQLite in-memory mới, không sửa dữ liệu thật.

## Những gì cần quan sát

Đếm Inspections của P-017 trước và sau hai lần đồng bộ; đối chiếu nhãn thiết bị, hàng P-099 mới, foreign key violations và hành vi retry. Không xem việc không có exception là bằng chứng toàn vẹn nghiệp vụ.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong workspace.
3. Tự sửa starter/EquipmentImporter.cs, không thay assertion trong starter/Program.cs.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ xem [solution/README.md](solution/README.md) và [reference source](solution/EquipmentImporter.cs) sau khi tự điều tra.

## Expected Results

Trước sửa: nhãn được cập nhật nhưng 2 bản ghi Inspections biến mất. Sau sửa: cả hai bản ghi còn nguyên sau hai lần đồng bộ, nhãn cuối đúng, thiết bị mới được thêm, không có foreign key violation.

~~~powershell
./verify.ps1
~~~

Verify phải FAIL (exit 1) trên starter gốc và PASS (exit 0) sau fix. Nếu không reproduce được, kiểm tra SDK, NuGet restore và đảm bảo chưa sửa starter.

## Estimated Time

70 phút. Tham khảo [incident evidence](evidence/incident-log.md), [workspace](workspace/my-investigation.md), [concepts sau lab](docs/concepts-after-lab.md).
