# UNIT-CONTRACT-001 — Tiến hóa hợp đồng manifest v1 → v2 mà không làm gián đoạn đối tác

## Mục tiêu
Tự tái hiện regression tương thích B2B API; phân tích evidence, sửa ingress boundary, xác minh contract tests và bảo vệ rollout bằng Architecture Decision Record.

## Bối cảnh thực tế
Các hãng vận tải gửi manifest. Một nhóm vẫn dùng payload v1, nhóm khác đã chuyển sang v2. Sau một release, manifest hợp lệ bị từ chối, trong khi một số request không hợp lệ lại được chấp nhận. Không thể buộc tất cả đối tác nâng cấp đồng thời.

## Bạn cần làm gì
1. Reproduce và ghi lại những contract cases sai kỳ vọng.
2. Viết ít nhất hai hypothesis, phân biệt symptom với nguyên nhân.
3. Chọn strategy cho hai version, unknown version, missing fields và rollout.
4. Chỉ sửa `starter/ManifestIngress.cs`, không sửa expected cases trong `starter/Program.cs`.
5. Chạy verification và ghi ADR tại [workspace](workspace/my-investigation.md).

## Yêu cầu môi trường
.NET SDK 10.x, PowerShell; không cần Azure, Docker, database hay external NuGet packages.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```
Hoặc `dotnet run --project starter/FreightContracts.csproj -- --self-test`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter chưa sửa. Script exit 0 khi chứng minh được ít nhất một mismatch; lệnh dotnet run trực tiếp exit khác 0 khi còn lỗi.

## Những gì cần quan sát
Phân loại payload hợp lệ bị reject và payload sai hợp đồng được accept. So sánh bằng chứng trong [incident timeline](evidence/incident-timeline.md) với [business constraints](docs/decision-brief.md); kiểm tra các default có làm sai nghĩa dữ liệu hay không. README cố ý không tiết lộ root cause.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** Chỉ mở [reference solution](solution/README.md) và [code](solution/ManifestIngress.cs) sau khi đã tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `./verify.ps1` chạy trên starter đã sửa, phải exit 0 và không làm yếu negative tests.

## Estimated Time
85 phút: 15' reproduce, 30' implement, 20' verify, 20' ADR và trade-offs.
