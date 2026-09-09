# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Hai component đều được mô tả là dùng singleton `FeatureCatalog`, nhưng `InstanceId` khác nhau và state update không được routing path quan sát.

## Evidence

Starter in ra hai `InstanceId` khác nhau. `catalog.CurrentRoute` đổi sang `v2`, trong khi `EndpointRouter.CurrentRoute` vẫn là `v1`.

## Root cause

`services.BuildServiceProvider()` được gọi trong lúc đang cấu hình services. Provider tạm thời tạo một singleton graph riêng. `EndpointRouter` giữ `FeatureCatalog` lấy từ graph đó, còn application provider cuối cùng tạo một `FeatureCatalog` singleton khác.

`Singleton` chỉ có nghĩa là một instance trên mỗi `IServiceProvider`, không phải một instance toàn process bất kể có bao nhiêu provider.

## Why the fix works

Đăng ký `EndpointRouter` bằng factory do container cuối cùng thực thi:

```csharp
services.AddSingleton<EndpointRouter>(sp =>
    new EndpointRouter(sp.GetRequiredService<FeatureCatalog>()));
```

Như vậy dependency được resolve từ cùng composition root và cùng provider graph.

## How to verify

Chạy:

```powershell
./verify.ps1
```

Hai `InstanceId` phải giống nhau và route sau update phải là `v2`.

## Alternative fixes

- Constructor-inject `FeatureCatalog` trực tiếp vào component được container tạo.
- Nếu chỉ cần configuration lúc registration, dùng Options/configuration binding thay vì tạo provider tạm.
- Với startup work phức tạp, chuyển logic sang hosted startup component có dependency được resolve từ container chính.

## Wrong or misleading fixes

- Đổi `FeatureCatalog` thành static: che dependency-graph bug và đưa global state vào process.
- Thêm lock: concurrency không phải nguyên nhân của hai object graph.
- Đổi registration thành transient/scoped: không giải quyết việc tạo nhiều provider và còn làm semantics khó dự đoán hơn.
- Đồng bộ thủ công state giữa hai instances: sửa symptom thay vì composition root.

## Production implications

Secondary providers có thể tạo duplicate singleton resources như caches, SDK clients, background components hoặc mutable registries. Bug thường khó thấy vì type/lifetime registration nhìn vẫn hợp lệ.

## Trade-offs

Container factory giữ composition rõ và đơn giản, nhưng không nên biến factory thành service locator phức tạp. Constructor injection vẫn là lựa chọn dễ đọc hơn khi có thể.

## What a Senior engineer should notice

Lifetime phải được reasoning cùng với provider boundary. Khi debug DI, đừng chỉ hỏi service là Singleton/Scoped/Transient; hãy hỏi instance được resolve từ provider nào và composition root có bị tách đôi không.
