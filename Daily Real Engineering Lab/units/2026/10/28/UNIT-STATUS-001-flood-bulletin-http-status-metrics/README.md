# UNIT-STATUS-001 — Dashboard báo toàn HTTP 200 dù API cảnh báo lũ trả 429 và 503

## Mục tiêu
Điều tra sự bất nhất giữa HTTP response mà người dùng nhận được và dữ liệu status-code dùng cho dashboard vận hành. Thu thập bằng chứng, xác định phạm vi ảnh hưởng, sửa trong workspace của bạn và chứng minh regression không còn.

## Bối cảnh thực tế
API cung cấp bản tin cảnh báo ngập cho các quận. Trong một đợt cao điểm, client ghi nhận nhiều 429/503 và một số 404, nhưng dashboard nội bộ báo 100% response thành công. Đội trực vận hành không biết nên tin phía nào. Hệ thống mẫu mô phỏng một phần request pipeline của ASP.NET Core, không cần dịch vụ cloud.

## Bạn cần làm gì
1. Chạy mẫu và lưu bảng client status/metric status.
2. Viết ít nhất hai hypothesis về điểm phát sinh sai lệch, chỉ ra evidence ủng hộ/bác bỏ.
3. Xác định contract đo lường cho response 200, 429, 503 và 404.
4. Chỉnh mã trong `starter/Program.cs` để metric phản ánh kết quả cuối của các response bình thường.
5. Chạy `verify.ps1`, ghi kết quả và nêu giới hạn của giải pháp với exception/streaming response.

## Yêu cầu môi trường
.NET SDK 10.x và PowerShell 7 hoặc Windows PowerShell. Không cần NuGet package bên ngoài, Docker, Azure hay database.

## Chạy nhanh
Tại thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```
`reproduce.ps1` phải exit 0 với starter ban đầu khi triệu chứng được tái hiện. `verify.ps1` phải exit khác 0 trước khi sửa, rồi exit 0 sau khi sửa đúng. Có thể chạy trực tiếp: `dotnet run --project starter/TelemetryLab.csproj -- --inspect`.

## Cách reproduce vấn đề
Không sửa starter trước lần chạy đầu. Chạy `./reproduce.ps1`, ghi lại các cặp giá trị của bốn request. So sánh với `evidence/incident-snapshot.md`. Sau đó chạy `./verify.ps1` để lấy baseline regression đỏ.

## Những gì cần quan sát
- Status trả về cho client có đúng theo kịch bản không?
- Các status ghi vào metric có khớp từng request không?
- Chênh lệch xảy ra với tất cả request hay chỉ một số status?
- Log latency và CPU có thực sự giải thích chênh lệch status không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ đọc sau khi tự thử: [Giải pháp tham chiếu](solution/README.md) và [mã tham chiếu](solution/Program.cs).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
50 phút (L2 investigation, D4). Phần mở rộng: đánh giá middleware ordering khi exception handler biến exception thành HTTP 500.
