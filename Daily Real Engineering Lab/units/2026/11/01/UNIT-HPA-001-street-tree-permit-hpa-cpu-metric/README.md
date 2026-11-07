# UNIT-HPA-001 — API cấp phép cắt tỉa cây không scale khi CPU tăng

## Mục tiêu

Điều tra một sự cố autoscaling của ASP.NET Core API trên Kubernetes, phân biệt ứng dụng quá tải với lỗi tín hiệu điều khiển, rồi bảo vệ quyết định sửa bằng evidence và kiểm thử local.

## Bối cảnh thực tế

API cấp phép cắt tỉa cây đường phố nhận lượng truy cập tăng mạnh. p95 từ 310 ms lên 2.4 s; hai Pod chạy nóng. HPA đã đặt từ 2 đến 8 replicas, mục tiêu CPU 70%, nhưng không scale. metrics-server vẫn có CPU usage; một audit agent chạy cùng Pod với API.

## Bạn cần làm gì

1. Đọc [snapshot](evidence/incident-snapshot.md) và [ràng buộc](docs/scenario.md).
2. Chạy chẩn đoán, ghi ít nhất ba giả thuyết cùng bằng chứng.
3. Đề xuất thay đổi tối thiểu trong `starter/deployment.yaml`; giữ nguyên HPA target.
4. Chạy verify và giải thích công thức cùng giới hạn của mô hình.
5. So sánh các phương án với reference sau khi tự hoàn thành.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7. Không cần cluster, Docker, .NET SDK, Azure hay package ngoài. Image `registry.example.invalid` là placeholder không dùng để deploy; đây là lab mô phỏng cấu hình.

## Chạy nhanh

Từ thư mục unit:

```powershell
pwsh -File ./scripts/run.ps1
pwsh -File ./scripts/reproduce.ps1
```

Với Windows PowerShell, thay `pwsh` bằng `powershell`.

## Cách reproduce vấn đề

`reproduce.ps1` kiểm tra cấu hình starter và CPU snapshot. REPRODUCE_PASS chỉ chứng minh triệu chứng ban đầu, không phải fix thành công. Sau khi sửa, script này có thể FAIL đúng thiết kế.

## Những gì cần quan sát

Đối chiếu HPA status, CPU usage theo container, resource requests/limits và các event. `kubectl top` có số liệu không có nghĩa HPA đã tính được CPU utilization. Tách root cause khỏi nhiễu trong log.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Sửa trong starter/.
4. Verify bằng `pwsh -File ./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution — chỉ xem sau khi tự điều tra](solution/README.md).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). verify.ps1 đọc **starter** đã sửa, không đọc solution.

## Estimated Time

60 phút: 10 phút reproduce, 20 phút điều tra, 20 phút sửa/verify, 10 phút trade-offs.
