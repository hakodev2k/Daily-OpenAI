# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Sau swap, production chạy `v2` đúng như mục tiêu nhưng `StorageContainer` trở thành `staging-images`. Request vẫn thành công nên lỗi có thể bị bỏ qua nếu chỉ kiểm tra HTTP status hoặc application version.

## 2. Evidence

Starter cho thấy:

```text
PRODUCTION_VERSION=v2
PRODUCTION_ENVIRONMENT=Production
PRODUCTION_STORAGE=staging-images
```

`EnvironmentName` ở lại với production slot, trong khi `StorageContainer` di chuyển cùng swap.

## 3. Root cause

`StorageContainer` là environment-specific configuration nhưng đang được khai báo như configuration có thể swap. Khi App Service slot swap diễn ra, giá trị staging đi sang production cùng application content.

## 4. Why the fix works

Đánh dấu storage configuration là slot-specific làm cho giá trị của nó gắn với deployment slot thay vì đi theo swap. Application version vẫn được promote, còn production resource boundary vẫn được giữ nguyên.

Trong simulator, sửa cả hai khai báo `StorageContainer` thành:

```csharp
["StorageContainer"] = new("prod-images", StickyToSlot: true)
```

và:

```csharp
["StorageContainer"] = new("staging-images", StickyToSlot: true)
```

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

```text
PRODUCTION_VERSION=v2
PRODUCTION_ENVIRONMENT=Production
PRODUCTION_STORAGE=prod-images
VERIFICATION=PASS
```

## 6. Alternative fixes

Có thể tách resource selection khỏi app settings và resolve bằng managed configuration/service discovery theo environment, nhưng đó là complexity lớn hơn. Với App Service slots và một vài environment-specific values, deployment slot settings thường là boundary đơn giản hơn.

## 7. Wrong / Tempting Fixes

- Hard-code `prod-images` trong code: phá portability và dễ gây lỗi khi chạy staging/local.
- Chạy script sửa setting sau mỗi swap: tạo một failure window và phụ thuộc ordering của deployment steps.
- Tắt slot swap và deploy trực tiếp production: tránh triệu chứng nhưng mất lợi ích của staged validation; không giải quyết hiểu biết về configuration boundary.
- Chỉ thêm smoke test kiểm tra HTTP 200: functional health không chứng minh resource isolation đúng.

## 8. Production implications

Các setting như database connection, storage account/container, Key Vault URI hoặc external endpoint có thể thuộc về environment boundary. Trước slot swap cần phân loại rõ setting nào nên swap và setting nào phải stay with slot.

## 9. Trade-offs

Không phải mọi setting đều nên sticky. Feature flags hoặc application-version-specific configuration đôi khi cần đi cùng deployment. Đánh dấu quá nhiều setting là slot-specific cũng có thể khiến staging không phản ánh đúng production behavior.

## 10. What a Senior engineer should notice

Senior engineer kiểm tra deployment không chỉ bằng "version đã lên" mà bằng invariant của production boundary: application version, identity, data destination, secrets, downstream endpoints và observability context phải đúng sau transition. Slot swap là một state transition cần verification, không chỉ là thao tác routing traffic.
