# Before — starter

- `./scripts/setup.ps1`: build dự kiến thành công nếu SDK phù hợp.
- `./scripts/reproduce.ps1`: exit 0 vì phát hiện đúng triệu chứng ban đầu.
- Bản ghi hợp lệ: `PackageCount=12`; audit count 0 trước lỗi.
- Bản ghi lỗi: `FormatException` được truyền lên; audit count cuối cùng là 1.
- Stack trace chứa `ImportCoordinator.Import` nhưng thiếu `SourceParser.Parse`.
- `./scripts/verify.ps1`: exit khác 0, in `VERIFY FAILED`.

Nếu không tái hiện được: kiểm tra `dotnet --info`, chạy trực tiếp `dotnet run --project checks/Verifier.csproj -- reproduce`, và xác nhận bạn chưa chỉnh sửa `starter/`. Không so sánh số dòng hay đường dẫn máy.
