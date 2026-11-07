# UNIT-PDB-001 — Điều phối bảo trì node cho API điều xe cứu thương bị kẹt ở bước drain

## Mục tiêu
Điều tra chênh lệch giữa mô hình dự báo khả năng bảo trì và tín hiệu thật từ Kubernetes PodDisruptionBudget (PDB); đưa ra quyết định về availability, replica capacity và voluntary eviction.

## Bối cảnh thực tế
Một dịch vụ điều phối xe cứu thương chạy ba replica trên Kubernetes. Đội hạ tầng muốn drain một node để vá hệ điều hành. Dashboard nội bộ dự báo có thể di chuyển một Pod mà vẫn đạt chính sách availability, nhưng Kubernetes không cho phép eviction. SLO yêu cầu không làm gián đoạn xử lý cuộc gọi khẩn cấp.

## Bạn cần làm gì
1. Đọc evidence và chính sách triển khai giả lập; ghi ít nhất ba giả thuyết trước khi xem source.
2. Chạy mô hình cục bộ, đối chiếu dự báo với các trường hợp PDB quan sát được.
3. Sửa riêng `starter/PdbCalculator.cs` để mô hình phản ánh đúng hợp đồng chính sách, không hard-code case mẫu.
4. Verify với nhiều quy mô replica và tình huống degraded.
5. Đề xuất hai phương án bảo trì an toàn, kèm trade-off về capacity, chi phí, rủi ro gián đoạn.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell 5.1+ hoặc PowerShell 7 trên Windows. Không cần cluster, Docker, Azure hay tài khoản bên ngoài. Đây là **mô hình local**, không phải lệnh thay đổi Kubernetes thực.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

`reproduce.ps1` exit 0 khi xác nhận **starter có lỗi**; không có nghĩa đã sửa xong.

## Cách reproduce vấn đề
Đối chiếu output `run.ps1` với `evidence/drain-timeline.md`, tập trung trường hợp ba Pod đều Ready và chính sách `minAvailable: "80%"`. Sau đó chạy `reproduce.ps1`.

## Những gì cần quan sát
- Số Pod Ready, số replica dự kiến, và giá trị `disruptionsAllowed`.
- Mô hình cho phép bao nhiêu voluntary eviction, Kubernetes cho phép bao nhiêu.
- Hệ quả khi số replica hoặc số Pod Ready thay đổi; phân biệt voluntary disruption với Pod failure.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify bằng `./scripts/verify.ps1` (exit 0 mới đạt).
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra và sửa.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
70 phút · L5 Design Decision · D4.
