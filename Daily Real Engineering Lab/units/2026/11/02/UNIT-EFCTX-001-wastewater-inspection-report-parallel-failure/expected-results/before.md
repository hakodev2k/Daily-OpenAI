# Before — starter

- `./setup.ps1`: restore/build thành công nếu SDK và NuGet sẵn sàng.
- `./reproduce.ps1`: exit 0 **chỉ khi** phát hiện lỗi đồng thời dự kiến; log có `InvalidOperationException` và thông điệp về second operation.
- `./verify.ps1`: exit khác 0; báo cáo chưa đáp ứng contract.
- Nếu thấy timeout ở gate, kiểm tra package restore, exception trước truy vấn hoặc đường chạy không đi qua interceptor; timeout không phải kết quả reproduce hợp lệ.
- Không sử dụng millisecond cố định làm điều kiện pass.
