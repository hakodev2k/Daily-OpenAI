# Scenario

Team có 8 backend developers, deploy 3–6 lần/tuần. Pipeline hiện tại:

`Git tag -> Dev build -> Dev deploy`

`same Git tag -> Staging build -> tests -> Staging deploy`

`same Git tag -> Production build -> Production deploy`

Các stage dùng cùng source tag nhưng runner image có thể được cập nhật độc lập. Package restore không dùng một artifact cache snapshot chung. Environment-specific settings được transform trong publish step.

## Evidence từ release gần nhất

| Stage | Source tag | Package checksum | Result |
|---|---|---|---|
| Staging | v4.18.2 | `sha256:A71...` | integration + smoke pass |
| Production | v4.18.2 | `sha256:C92...` | deployed |

Không có evidence cho thấy khác checksum đã gây incident. Vấn đề cần quyết định là mức assurance mà pipeline hiện tại thực sự cung cấp.

## Constraints
- Production approval phải tham chiếu được artifact cụ thể.
- Secrets không được bake vào package.
- Mỗi environment có endpoint và feature configuration khác nhau.
- Rollback target: dưới 10 phút.
- Team không muốn tăng đáng kể số pipeline components phải vận hành.
- Có yêu cầu audit: chứng minh production release đã qua staging validation.

## Phương án cần so sánh
A. Giữ rebuild riêng cho từng environment.
B. Build một versioned artifact rồi promote qua các environment.
C. Build riêng nhưng cố định runner/dependency inputs để hướng tới reproducible builds.

Bạn có thể đề xuất phương án D nếu giải thích được vì sao.

## Decision checklist
- Artifact identity có rõ không?
- Staging evidence có áp dụng trực tiếp cho production binary không?
- Runtime configuration được tách thế nào?
- Rollback cần những bước gì?
- Dependency/toolchain drift được kiểm soát ra sao?
- Chi phí vận hành và migration là gì?
- Có trường hợp nào rebuild-per-environment vẫn hợp lý không?