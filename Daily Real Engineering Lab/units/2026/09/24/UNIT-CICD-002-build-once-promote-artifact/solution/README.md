# Reference Solution — inspect only after your own decision

Đây là một defensible solution dưới constraints đã cho, không phải lựa chọn duy nhất đúng.

## Symptoms
Cùng source tag được build nhiều lần; production package không có identity trùng với artifact đã qua staging tests.

## Evidence
Checksum khác nhau chứng minh output artifact khác nhau. Nó không tự chứng minh production binary sai, nhưng làm yếu claim “binary chạy production chính là binary đã được staging validate”.

## Root cause
Release boundary đang coi source revision là release identity trong khi build còn có các input khác và environment transform nằm trong publish step.

## Decision
Ưu tiên Option B: build một immutable, versioned artifact một lần; lưu checksum/SBOM/build metadata; promote chính artifact đó qua Staging và Production. Environment-specific endpoints, secrets và feature configuration được inject ở deploy/runtime boundary thay vì bake vào binary.

## Why this works
Approval và test evidence gắn với một artifact identity cụ thể. Promotion không tạo binary mới, nên production nhận đúng deployable đã được staging kiểm tra. Rollback có thể chọn lại artifact version trước đó mà không cần rebuild source cũ.

## Verification
Một release record nên chứng minh được chuỗi: commit -> build run -> artifact version/checksum -> staging test result -> approval -> production deployment. Kiểm tra checksum của artifact trước mỗi deployment và smoke-test environment-specific behavior sau deploy.

## Alternatives
Option C có giá trị khi tổ chức cần reproducible builds vì supply-chain assurance: pin SDK, dependencies và build environment để cùng inputs tạo cùng output. Tuy nhiên reproducibility không tự thay thế artifact promotion; nó giải một assurance khác. Option A có thể chấp nhận khi output bắt buộc khác theo target platform/environment, nhưng cần định nghĩa rõ artifact lineage và test strategy cho từng output.

## Wrong / Tempting Fixes
- Chỉ kiểm tra cùng Git tag: chưa chứng minh cùng binary.
- Bake secrets/settings vào artifact để “đóng băng mọi thứ”: tăng security và rotation risk.
- Rebuild khi rollback: tạo thêm một artifact mới đúng lúc incident đang diễn ra.
- Chỉ pin runner image nhưng vẫn để dependency resolution trôi: giảm một nguồn drift nhưng chưa đóng toàn bộ build inputs.

## Production implications
Cần artifact repository/retention policy, immutable version naming, provenance metadata, deployment-time configuration validation và quyền promotion tách khỏi quyền build khi cần audit.

## Trade-offs
Build-once tăng storage và artifact-governance requirements nhưng giảm ambiguity của release identity. Runtime configuration tăng nhu cầu validation ở deployment boundary. Reproducible builds bổ sung assurance nhưng có chi phí engineering cao hơn.

## What a Senior engineer should notice
Mục tiêu không phải áp dụng slogan “build once, deploy many”; mục tiêu là thiết kế một chain of evidence đủ mạnh cho release, rollback và audit trong constraints thực tế.