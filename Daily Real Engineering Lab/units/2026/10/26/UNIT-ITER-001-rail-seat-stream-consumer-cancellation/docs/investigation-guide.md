# Investigation Guide

1. Chạy `run.ps1`, lưu bốn chỉ số và exit code.
2. Chạy `reproduce.ps1`; exit code 0 nghĩa là lỗi được tái hiện.
3. Đọc `scripts/LabChecks/Program.cs`, phân biệt direct token và subscriber token.
4. Theo dõi hai thời điểm: tạo enumerable và lấy enumerator. Đọc `starter/SeatAvailabilityFeed.cs`.
5. Viết hai hypothesis và phép thử bác bỏ chúng.
6. Sửa starter, chạy `verify.ps1` và đối chiếu cả bốn chỉ số.

## Khi không reproduce được
Kiểm tra `dotnet --info`, restore project harness, chạy trực tiếp lệnh `dotnet run` trong README. Không chỉnh harness để ép pass.

## Evidence
Ghi kết quả hủy trước/sau lần MoveNextAsync, token nào được truyền, token nào được kiểm tra, và một hypothesis đã loại trừ.
