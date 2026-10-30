# UNIT-JBATCH-001 — Bảng kê hoàn trả thiết bị thể thao: lô bị từ chối nhưng số liệu đã thay đổi

## Mục tiêu

Điều tra một batch import .NET 8 có kết quả từ chối nhưng dữ liệu tồn kho không còn khớp với trạng thái trước khi import. Thu thập evidence, viết hypothesis, sửa đường xử lý và chứng minh contract khi dữ liệu hợp lệ hoặc không hợp lệ.

## Bối cảnh thực tế

Các trung tâm thể thao cộng đồng gửi bảng kê hoàn trả dụng cụ về kho. Một file JSON có hàng cuối không đúng hợp đồng được báo là bị từ chối, nhưng bộ phận vận hành thấy số lượng một số mã dụng cụ thay đổi. Quy định nghiệp vụ: một batch được chấp nhận đầy đủ hoặc không làm thay đổi kho.

## Bạn cần làm gì

1. Chạy chế độ reproduce trước khi sửa; ghi lại trạng thái kho và exception.
2. Viết ít nhất ba giả thuyết, phân biệt lỗi parse, lỗi dữ liệu nghiệp vụ và lỗi lưu trữ.
3. Chỉ sửa starter/BatchImporter.cs. Không sửa Program.cs hoặc ManifestStore.cs vì đây là contract harness.
4. Đảm bảo lô JSON lỗi cú pháp/kiểu, lô vi phạm quy tắc nghiệp vụ đều không thay đổi kho; lô hợp lệ vẫn được áp dụng.
5. Chạy verify trên chính starter đã sửa; ghi trade-offs về batch lớn trước khi xem reference.

## Yêu cầu môi trường

.NET SDK 8.x và PowerShell 5.1+ hoặc 7. Không cần Docker, NuGet package ngoài SDK, database hay dịch vụ trả phí.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
./run.ps1
./reproduce.ps1
~~~

Hoặc:

~~~powershell
dotnet run --project ./starter/IntakeLab.csproj -c Release -- --reproduce
~~~

## Cách reproduce vấn đề

Chạy reproduce.ps1 trên starter ban đầu. Script trả exit code 0 chỉ khi cả tín hiệu từ chối và sai lệch số lượng kỳ vọng được quan sát. Sau khi sửa, chạy verify.ps1; reproduce không còn cần pass vì triệu chứng gốc đã được khắc phục.

## Những gì cần quan sát

- Exception phát sinh ở vị trí nào trong vòng đời xử lý file?
- Có bao nhiêu mã hàng còn lại trong kho sau khi request bị từ chối?
- Kết quả của một file hợp lệ có khác với file lỗi không?
- Contract nào phải giữ nguyên ngay cả khi payload lớn, có duplicate hoặc bị cancellation?

Xem evidence/incident.md và ghi workspace/my-investigation.md trước khi mở hints.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis và evidence.
3. Tự sửa starter/BatchImporter.cs.
4. Verify cả correctness và tính toàn vẹn của batch.
5. Chỉ sau đó xem reference solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Giải thích](solution/README.md) và [mã tham khảo](solution/BatchImporter.cs) — chỉ xem sau khi tự điều tra.

## Expected Results

Trước sửa: lô không hợp lệ bị từ chối nhưng còn 2 mã trong kho; reproduce PASS. Sau sửa: verify PASS cả lô sai JSON, lô sai quy tắc và lô hợp lệ. verify phải FAIL trên starter ban đầu.

## Estimated Time

50–65 phút, L3 / D4. Nếu dotnet không chạy, kiểm tra SDK và chạy lệnh từ thư mục unit.
