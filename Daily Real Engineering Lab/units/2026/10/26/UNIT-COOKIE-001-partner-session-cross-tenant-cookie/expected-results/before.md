# Before — starter chưa sửa

`pwsh -File ./reproduce.ps1` phải exit 0 với xu hướng output:

```text
alpha.afterLogin=OK session=alpha
beta.beforeLogin=Forbidden session=alpha
REPRODUCED: cross-tenant session observed
```

`pwsh -File ./verify.ps1` phải exit 1: Beta trước login không phải 401, và Alpha có thể mất session sau khi Beta login.

Nếu không tái hiện: kiểm tra .NET SDK/runtime, đảm bảo chạy đúng `starter/`, không dùng reference solution trước, không có process khác chiếm cổng (Kestrel tự chọn cổng).
