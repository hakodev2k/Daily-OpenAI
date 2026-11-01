# Ràng buộc nghiệp vụ và kỹ thuật

- ASP.NET Core permit API và audit agent cùng Pod; hai Pod tương đương.
- HPA autoscaling/v2, CPU Resource utilization target 70%, min 2, max 8.
- CPU snapshot cố định để tính toán; không phải telemetry live.
- Không tắt audit agent, không thay HPA target để che lỗi, không scale thủ công làm giải pháp lâu dài.
- Chỉ chỉnh `starter/deployment.yaml` trong bài chính; trình bày lý do sizing.
- Evidence và event là dữ liệu mô phỏng. Không có cluster thật, không được tuyên bố production đã được kiểm chứng.

Deliverables: YAML sửa, hypothesis/evidence, reproduce và verify output, tính toán, đề xuất rollout/rollback.
