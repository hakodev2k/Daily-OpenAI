# UNIT-REDIS-004 — Cache Miss Burst Under Concurrency

## Mục tiêu

Điều tra một read path hoạt động tốt khi cache đã warm nhưng tạo tải downstream tăng đột biến khi một key phổ biến hết hạn trong lúc có nhiều request đồng thời.

## Bối cảnh thực tế

Một product API cache dữ liệu catalog có chi phí tải tương đối cao. Trong vận hành bình thường latency thấp. Tuy nhiên ngay sau một số thời điểm cache entry hết hạn, dependency load và request latency cùng tăng mạnh trong một khoảng ngắn rồi tự trở lại bình thường.

## Bạn cần làm gì

1. Chạy starter ở trạng thái ban đầu.
2. Reproduce burst bằng script được cung cấp.
3. Quan sát số request, số lần downstream loader chạy và latency tổng thể.
4. Ghi ít nhất ba hypothesis vào `workspace/my-investigation.md`.
5. Sửa `starter/` để giữ đúng functional behavior nhưng cải thiện hành vi dưới concurrent cache miss.
6. Chạy `verify.ps1`.
7. Chỉ sau khi tự thử mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần Redis server; lab dùng local deterministic cache simulation để cô lập cơ chế.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Tất cả request trả về đúng dữ liệu hay không.
- `loaderCalls` thay đổi thế nào khi nhiều request cùng bắt đầu với cache rỗng.
- Tổng thời gian của burst.
- Sau burst, request tiếp theo có sử dụng cached value không.

Không tối ưu dựa trên phỏng đoán. Hãy dùng output làm evidence trước khi sửa.

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

[Reference Solution — spoiler](solution/README.md)

## Expected Results

Before: functional result đúng, nhưng một cold-key burst làm downstream loader chạy nhiều lần gần như đồng thời.

After: functional result vẫn đúng; cùng một key trong cùng miss window không còn nhân số lần expensive load theo số concurrent callers. Request cho key khác vẫn độc lập.

Nếu không reproduce được, kiểm tra `dotnet --version`, chạy trực tiếp `dotnet run --project starter -- --reproduce`, và đảm bảo bạn chưa sửa starter trước khi chạy reproduction.

## Estimated Time

35–50 phút.