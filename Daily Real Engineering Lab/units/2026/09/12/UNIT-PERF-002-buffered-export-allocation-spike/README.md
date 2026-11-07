# UNIT-PERF-002 — Export ổn ở functional test nhưng memory tăng mạnh khi chạy đồng thời

## Mục tiêu

Điều tra một report-export path trả dữ liệu đúng nhưng tạo lượng managed allocation lớn không tương xứng với payload, khiến throughput giảm khi nhiều export chạy cùng lúc.

## Bối cảnh thực tế

Một background report service xuất file lớn sang storage. Functional tests đều xanh và CPU không cao bất thường, nhưng khi nhiều tenant chạy export gần nhau, process memory tăng nhanh, GC hoạt động dày hơn và một số job chậm đáng kể.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce allocation symptom bằng script có sẵn.
3. Ghi hypothesis vào `workspace/my-investigation.md`.
4. Xác định phần nào của data path làm peak allocation tăng mà không tạo thêm business value.
5. Sửa code trong `starter/`.
6. Chạy `verify.ps1` để kiểm tra cả correctness và allocation property.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay cloud account

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy starter với payload cố định và xác nhận lượng allocation trong export path cao hơn đáng kể so với mức cần thiết để ghi dữ liệu ra destination stream.

## Những gì cần quan sát

- `PAYLOAD_BYTES`
- `ALLOCATED_BYTES`
- ratio giữa allocation và payload
- số lớp buffer/copy tồn tại trên đường đi của bytes
- destination stream có thực sự yêu cầu toàn bộ payload phải tồn tại thêm một bản sao trong memory hay không

Không tối ưu theo cảm giác; dùng output đo được để kiểm chứng hypothesis.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- output functional vẫn đúng
- export path allocate một vùng memory có kích thước xấp xỉ payload hoặc lớn hơn dù destination có thể nhận dữ liệu trực tiếp
- khi concurrency tăng, nhiều allocation lớn có thể cùng tồn tại

### After

- output bytes không đổi
- `verify.ps1` xác nhận allocation của export path thấp hơn threshold
- learner fix được kiểm tra trực tiếp từ `starter/`, không phải reference solution

## Troubleshooting

Nếu `dotnet` không nằm trong `PATH`, cài .NET 8 SDK hoặc mở terminal đã load đúng SDK. Kết quả allocation có thể dao động nhẹ giữa runtime builds nên script dùng threshold thay vì yêu cầu một con số tuyệt đối.

## Estimated Time

35–55 phút.