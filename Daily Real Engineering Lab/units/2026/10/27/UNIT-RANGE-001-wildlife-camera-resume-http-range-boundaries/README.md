# UNIT-RANGE-001 — Ảnh camera động vật bị lỗi sau khi tải từng HTTP byte range

## Mục tiêu

Điều tra lỗi dữ liệu nhị phân khi client ghép các phản hồi HTTP 206 Partial Content; dùng request/response evidence để chứng minh nguyên nhân, sửa và kiểm tra regression.

## Bối cảnh thực tế

Hệ thống lưu ảnh từ camera theo dõi động vật hoang dã cho phép tải tiếp tệp theo các đoạn nhỏ. Ảnh nhỏ có lúc tải được, nhưng ảnh lớn ghép xong có kích thước bất thường và checksum khác ảnh gốc. HTTP server luôn trả 206, không có lỗi mạng.

## Bạn cần làm gì

Chạy fixture local, ghi lại kích thước và hex output cho ba ảnh giả lập (10, 12 và 1 byte). Đưa ra ít nhất hai hypothesis, xác định request nào gây lệch dữ liệu, sửa code tại `starter/RangeLab/ChunkDownloader.cs`, và chứng minh toàn bộ fixture đạt kiểm tra.

## Yêu cầu môi trường

.NET SDK 8.x, PowerShell. Không cần Docker, Internet, Azure hoặc NuGet package bên ngoài. Nếu SDK khác, xem `global.json`.

## Chạy nhanh

Từ thư mục unit:

```powershell
dotnet --version
.\run.ps1
.\reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
.\reproduce.ps1
```

Script chạy code nguyên bản trong `starter/`, chỉ thành công khi tái hiện được hai fixture lỗi. `run.ps1` in thêm kết quả quan sát, không đánh giá fix.

## Những gì cần quan sát

Ghi `sourceBytes`, `downloadedBytes`, `sameBytes`, `firstMismatch` và `requests`. Xem thêm [trace tham khảo](evidence/range-trace.md). Chú ý sự khác biệt giữa ảnh một đoạn và ảnh nhiều đoạn; đừng dựa vào status code 206 để kết luận tính toàn vẹn.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix chỉ ở `starter/RangeLab/ChunkDownloader.cs`.
4. Verify bằng `.\verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) và [reference code](solution/ChunkDownloader.cs) sau khi đã tự sửa và chạy verify.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md). `reproduce.ps1` kiểm tra starter lỗi; `verify.ps1` kiểm tra code bạn chỉnh trong starter, không chạy reference.

## Estimated Time

60 phút (L3 Production Incident / D4).
