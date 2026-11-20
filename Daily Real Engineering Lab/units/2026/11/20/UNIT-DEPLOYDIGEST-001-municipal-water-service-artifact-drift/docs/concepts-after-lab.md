# Concepts after lab

- **Tag** là tên tham chiếu có thể đổi; **digest** xác định image manifest.
- **Build once, promote many** giữ artifact nhất quán giữa môi trường.
- **Provenance** liên kết source, builder, workflow và chữ ký; digest tự nó không chứng minh build đáng tin.
- **Runtime verification** so sánh imageID của Pod với digest kỳ vọng.
- **Rollback readiness** đòi hỏi retention, compatibility schema/config và dữ liệu.

Tài liệu: [OCI Image Spec](https://github.com/opencontainers/image-spec), [SLSA](https://slsa.dev/spec/v1.0/), [Kubernetes Images](https://kubernetes.io/docs/concepts/containers/images/).
