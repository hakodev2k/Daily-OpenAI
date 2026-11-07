# Sau khi sửa
- API hợp lệ tiếp tục trả 200 JSON và dữ liệu không đổi.
- Điều hướng SPA tiếp tục trả 200 HTML.
- Cả ba đường dẫn API không hợp lệ trả **404** với `application/json` và mã `api_route_not_found`.
- URL static asset không tồn tại tiếp tục 404.
- `verify.ps1` trả 0 trên starter đã sửa. `dotnet run --project contract/Contract.csproj -- --mode reference` có thể dùng để tự kiểm reference sau khi học, nhưng **không thay thế verify của starter**.

Nếu verify chưa qua: đọc output từng path, kiểm tra route API catch-all không chặn route hợp lệ và không biến SPA navigation thành JSON 404.
