# Hợp đồng giả lập
Origin trả body chính xác bằng `route|locale|audience`. Route R10/R20; locale vi/en; audience general/student. Tất cả GET advisory public, không có PII. Baseline CDN cache response 200 trong 10 phút, cache key chỉ gồm path `/advisories/{route}`. Ban đầu rỗng, TTL không hết hạn trong trace, không eviction. SLO correctness 100%; mục tiêu hit ratio >=40%; budget tối đa 12 entries. Cần purge mọi variant khi advisory thay đổi.
## Change request (đọc sau vòng thiết kế đầu)
Header `X-Accessibility: screen-reader` bổ sung hướng dẫn đọc màn hình vào body. Giá trị khác hoặc missing là default. Nếu sau này có endpoint account, response cá nhân không được shared cache.
