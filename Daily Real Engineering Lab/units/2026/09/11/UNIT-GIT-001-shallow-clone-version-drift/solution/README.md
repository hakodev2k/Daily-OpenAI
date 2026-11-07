# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Local repository suy ra được `1.2.0`, nhưng CI checkout trả `0.0.0-dev` dù đang ở đúng commit mới nhất.

## Evidence

CI checkout là shallow repository. `git tag` không thấy tag release và `git describe --tags --abbrev=0` không thể đi qua history đã bị cắt.

## Root cause

Pipeline tối ưu clone bằng shallow history, trong khi versioning logic giả định repository luôn có đầy đủ reachable history và tags. Khi giả định này sai, script nuốt lỗi và biến thiếu metadata thành một version có vẻ hợp lệ.

## Why the fix works

Reference implementation kiểm tra shallow state, phục hồi history/tags cần thiết bằng `git fetch --tags --unshallow`, rồi mới chạy version derivation. Nếu vẫn không có release tag, script fail rõ ràng thay vì publish fallback giả.

## How to verify

Thay nội dung `starter/Get-Version.ps1` bằng cách sửa của bạn và chạy:

```powershell
./verify.ps1
```

Kết quả phải chứa `verifiedVersion=1.2.0` và `LAB_VERIFY_PASS`.

## Alternative fixes

- Cấu hình CI checkout với full history ngay từ đầu nếu versioning luôn phụ thuộc Git history.
- Dùng version được pipeline/release system truyền vào như immutable build metadata thay vì suy ra từ repository ở mọi stage.
- Fetch chỉ tags/history cần thiết nếu repository lớn và chi phí full unshallow đáng kể.

## Wrong or misleading fixes

- Hard-code `1.2.0`: chỉ che symptom và phá release kế tiếp.
- Giữ `0.0.0-dev` như fallback production: build vẫn xanh nhưng artifact identity sai.
- Chỉ chạy `git fetch --tags` rồi giả định luôn đủ: tag có thể tồn tại nhưng commit/history cần cho `git describe` vẫn chưa reachable trong một số shallow topology.
- Bỏ shallow clone ở mọi pipeline mà không đo chi phí: có thể đúng nhưng cần hiểu trade-off thời gian checkout và repository size.

## Production implications

Artifact version là một phần của observability, rollback, SBOM/provenance và incident response. Version sai có thể khiến đội vận hành đối chiếu nhầm binary với source commit.

## Trade-offs

Full history đơn giản và đáng tin cậy hơn nhưng tăng network/time. Pipeline-supplied version giảm phụ thuộc Git metadata ở build stage nhưng cần một nguồn version authoritative và contract rõ giữa release orchestration với build.

## What a Senior engineer should notice

Vấn đề không nằm ở cú pháp `git describe` mà ở hidden environmental contract: script build phụ thuộc repository topology nhưng contract đó không được kiểm tra. Khi tối ưu CI, phải xác định những build steps nào phụ thuộc history, tags, submodules hoặc LFS trước khi thu hẹp checkout.
