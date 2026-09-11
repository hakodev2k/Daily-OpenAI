# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Một burst gồm 20 request cho cùng một hot key đều trả đúng price, nhưng nguồn dữ liệu bị gọi nhiều lần trong cùng cửa sổ cache miss.

## Evidence

`requestCount=20` trong khi `sourceCalls` gần với số request thay vì số key cần tải. Điều này cho thấy cache vẫn đúng về dữ liệu nhưng không coalesce concurrent miss.

## Root cause

Cache-aside path chỉ làm `TryGet` rồi gọi source khi miss. Nhiều caller cùng thấy miss trước khi caller đầu tiên hoàn tất source load và populate cache, nên tất cả cùng đi xuống source.

## Why the fix works

Reference solution dùng lock bất đồng bộ theo key. Caller đầu tiên giành quyền load; các caller còn lại chờ. Sau khi vào critical section, code **check cache lần thứ hai**. Vì caller đầu tiên đã populate cache, các caller sau trả cache hit thay vì gọi source.

## How to verify

Copy/adapt cách sửa vào `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Expected: `requestCount=20`, `sourceCalls=1`, `allPricesCorrect=True`.

## Alternative fixes

- Distributed single-flight/lock khi nhiều process cùng chia sẻ Redis và downstream rất đắt.
- Redis-specific stale-while-revalidate hoặc probabilistic early refresh cho hot key lớn.
- Pre-warming cho tập key nhỏ, dự đoán được.

Các lựa chọn này chỉ hợp lý khi topology và business constraints cần chúng.

## Wrong / Tempting Fixes

- Tăng TTL: chỉ làm stampede ít xảy ra hơn, không loại bỏ failure mode khi expiry vẫn đến.
- Scale database ngay: tăng capacity nhưng giữ nguyên amplification factor.
- Dùng một global lock cho mọi key: sửa symptom nhưng serialize các key độc lập và tạo bottleneck mới.
- Retry source call: có thể làm amplification tệ hơn khi source đang degrade.

## Production implications

Với Redis dùng chung bởi nhiều API instance, per-process lock chỉ coalesce request trong một process. Cần đánh giá traffic distribution, cost của duplicate load, idempotency của source và độ phức tạp vận hành trước khi thêm distributed coordination.

## Trade-offs

Single-flight giảm duplicate load nhưng thêm coordination, lifecycle management cho per-key locks và có thể tạo contention trên hot key. Distributed locking còn thêm failure modes riêng như lease expiry và lock owner crash.

## What a Senior engineer should notice

Cache hit ratio cao không đồng nghĩa dependency được bảo vệ tốt. Cần đo miss concurrency, source amplification và behavior tại expiry boundary. Thiết kế cache phải xét cả steady state lẫn transition state khi entry biến mất hoặc refresh.
