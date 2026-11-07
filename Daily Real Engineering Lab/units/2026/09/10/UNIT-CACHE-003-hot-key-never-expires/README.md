# UNIT-CACHE-003 — Hot cache key không bao giờ nhận cấu hình mới

## Mục tiêu

Điều tra một lỗi cache expiration policy trong đó một key được đọc liên tục vẫn phục vụ dữ liệu cũ dù nguồn dữ liệu đã thay đổi từ lâu.

## Bối cảnh thực tế

Pricing API cache một `PricingRule` để giảm tải cho configuration store. Operations thay đổi rule từ `v1` sang `v2`. Những tenant ít traffic nhận `v2` sau vài phút, nhưng tenant nhiều traffic tiếp tục nhận `v1` trong thời gian rất dài. Restart process làm mọi tenant nhận đúng `v2` ngay lập tức.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại timeline của cache hit, source version và thời điểm expiration.
3. Đưa ra ít nhất hai hypothesis giải thích vì sao hot tenant khác cold tenant.
4. Sửa code trong `starter/` để cache vẫn có thể tận dụng locality nhưng dữ liệu không được sống vô hạn chỉ vì key được truy cập thường xuyên.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Redis server, database, Docker hay cloud account. Lab dùng fake clock + in-memory cache policy để mô phỏng cơ chế một cách deterministic.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chỉ pass khi starter chứng minh được rằng source đã đổi sang `v2` nhưng hot key vẫn trả `v1` sau thời lượng vượt quá freshness requirement.

## Những gì cần quan sát

- Version hiện tại trong source.
- Version được trả về từ cache.
- Thời điểm truy cập gần nhất của cache entry.
- Tuổi thực của cache entry kể từ khi được tạo.
- Khác biệt giữa hot key và cold key.

Không giả định rằng “TTL = 5 phút” đồng nghĩa dữ liệu luôn cũ tối đa 5 phút. Hãy kiểm tra TTL đang được tính từ mốc nào và mốc đó có bị thay đổi bởi read traffic hay không.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Investigation Workspace

[my-investigation.md](workspace/my-investigation.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

### Starter

- Cold tenant eventually reloads `v2`.
- Hot tenant continues returning `v1` even after the configured freshness window has elapsed.

### Sau khi sửa

- Hot traffic may extend short-term reuse if desired.
- No cache entry can exceed the defined maximum age from creation.
- After the maximum age, the next read must reload `v2` from source.

## Estimated Time

30–45 phút.
