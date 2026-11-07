# Reference Solution — chỉ mở sau khi tự thử

## 1. Symptoms

Ca chiều nhận cả các dòng ca sáng. Khi bật strict DI validation, cấu hình starter bị từ chối.

## 2. Evidence

`BatchComposer` lưu dữ liệu trong `List<string> _rows`. Service này được đăng ký scoped, nhưng `ManifestExportWorker` singleton nhận nó trực tiếp qua constructor. Root provider tạo worker, kéo theo scoped dependency được resolve ở root scope khi validation tắt. Cùng instance tồn tại qua cả hai lần `Export`.

## 3. Root cause

**Captive dependency**: singleton giữ scoped service được resolve từ root provider. `AddScoped` chỉ tạo instance mới **theo scope**, không phải theo method call.

## 4. Why the fix works

Worker singleton chỉ giữ `IServiceScopeFactory`. Mỗi `Export` tạo scope riêng, resolve `BatchComposer`, xuất dữ liệu và dispose scope. Mỗi phiên có accumulator riêng. Strict validation giờ chấp nhận object graph.

Mã tham chiếu: [Program.cs](Program.cs). Kiểm tra riêng:

```powershell
dotnet run --project ./solution/ExamManifestLab.csproj -- verify
```

## 5. Verification

Chạy `./verify.ps1` trên **starter đã sửa**, không phải solution. Assert ca sáng đúng 2 dòng, ca chiều đúng 1 dòng, đồng thời `ValidateScopes` và `ValidateOnBuild` không báo lỗi. `reproduce.ps1` sẽ không còn xác nhận lỗi sau khi fix, điều này là đúng.

## 6. Alternative fixes

- Đổi worker thành scoped và tạo scope cho từng job tại orchestration layer: hợp lệ nếu có lifecycle rõ ràng.
- Biến composer thành stateless, dùng biến local cho mỗi lần Compose: đơn giản hơn khi không cần state theo job.
- Dùng factory typed per-job: hợp lý khi job có nhiều scoped dependency, đổi lại thêm abstraction.

## 7. Wrong / tempting fixes

- `_rows.Clear()` trước mỗi job: có thể che triệu chứng tuần tự nhưng không giải quyết captive dependency và không an toàn với concurrent calls.
- Đổi `AddScoped` thành `AddTransient` mà vẫn inject vào singleton: transient vẫn bị giữ suốt đời singleton.
- Tắt `ValidateScopes`: che cảnh báo thay vì sửa ownership.
- Restart process giữa hai ca: workaround tốn kém, không phải hợp đồng lifecycle.

## 8. Production implications

Worker chạy lâu có thể trộn dữ liệu giữa job. Bật DI scope validation trong test/CI; thiết kế scope per message/job; không giữ mutable work-item state trong singleton. Với dependency `IAsyncDisposable`, cân nhắc `CreateAsyncScope`.

## 9. Trade-offs

Tạo scope per job có overhead nhỏ nhưng cho phép ownership/disposal rõ ràng. Stateless composer là lựa chọn gọn hơn nếu domain không cần state; scope factory phù hợp khi có nhiều dependency scoped, transaction hoặc context cùng một job.

## 10. Senior engineer should notice

Lifetime thuộc **object graph và scope resolution**, không thuộc tên registration một cách độc lập. Kiểm thử output đơn thuần có thể bị qua mặt bằng filtering; strict DI validation bắt lỗi cấu trúc ngay cả khi output tạm thời đúng.
