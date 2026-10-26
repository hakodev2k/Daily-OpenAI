# Business contracts

- Partner A (35% traffic) dùng v1: `manifestId`, `partnerId`, `weightKg`; không có `schemaVersion`. V1 không khai báo hazard class. Canonical mapping phải giữ `NOT_APPLICABLE` (chưa khai báo), không được suy diễn thành `NONE`.
- Partner B (65%) dùng v2: `schemaVersion: 2`, `grossWeightKg`, `hazardClass` bắt buộc; hazard chỉ nhận `NONE`, `FLAMMABLE`, `CORROSIVE`.
- Hai version đều cần manifestId/partnerId không rỗng, trọng lượng > 0.
- V2 có đồng thời `weightKg` và `grossWeightKg` là ambiguous; unknown version phải reject.
- Downstream chỉ nhận `CanonicalManifest`. Legacy v1 được giữ tối thiểu 60 ngày; rollback dưới 15 phút.
- Không được coi `NOT_APPLICABLE` là xác nhận hàng hóa an toàn; downstream phải áp dụng policy riêng.

Dữ liệu synthetic, không chứa credentials hay thông tin thật.
