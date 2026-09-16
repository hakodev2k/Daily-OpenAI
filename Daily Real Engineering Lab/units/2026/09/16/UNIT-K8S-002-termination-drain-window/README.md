# UNIT-K8S-002 — Rolling Deploy Gây Lỗi Request Ngắn Hạn

## Mục tiêu
Điều tra một deployment chạy ổn định bình thường nhưng xuất hiện một cụm request thất bại ngắn trong lúc rolling update, dựa trên timeline của application lifecycle và traffic routing.

## Bối cảnh thực tế
Một ASP.NET Core service chạy nhiều replica trên Kubernetes. Khi không deploy, error rate gần như bằng 0. Mỗi lần rolling update, dashboard lại xuất hiện một spike nhỏ 5xx kéo dài vài giây. CPU, memory và downstream dependency đều bình thường.

## Bạn cần làm gì
1. Chạy starter để tái hiện timeline của một pod đang phục vụ traffic khi rollout bắt đầu.
2. Thu thập các mốc `REQUEST`, `LIFECYCLE` và kết quả response.
3. Ghi ít nhất ba hypothesis trước khi sửa.
4. Xác định contract nào giữa application lifecycle và traffic routing đang không khớp.
5. Sửa learner-editable code/config trong `starter/` để request mới không đi vào instance trong giai đoạn nó không còn đủ khả năng hoàn thành request an toàn.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell
- Không cần Kubernetes cluster; starter mô phỏng lifecycle/routing contract cục bộ.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy một rollout simulation cố định, gửi request trước, trong và sau lifecycle transition, rồi kiểm tra có request thất bại trong cửa sổ rollout hay không.

## Những gì cần quan sát
- Thứ tự giữa lifecycle transition và request admission.
- Request nào được nhận vào trước/sau từng mốc.
- Service có healthy trước và sau rollout hay không.
- Lỗi có tập trung trong một cửa sổ lifecycle cụ thể hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** service bình thường ngoài rollout nhưng một request trong lifecycle transition thất bại.

**After:** request mới không được admission vào instance trong unsafe transition window; các request đã được nhận trước đó vẫn có cơ hội hoàn thành; functional behavior ngoài rollout không đổi.

## Estimated Time
40–60 phút.