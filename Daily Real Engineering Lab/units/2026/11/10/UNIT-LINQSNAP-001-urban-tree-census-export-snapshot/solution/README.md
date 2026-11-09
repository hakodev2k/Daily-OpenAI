# Reference Solution — chỉ xem sau khi tự điều tra

## Triệu chứng và evidence
Trong báo cáo đầu, `Count=2` nhưng `Ids=[T-101,T-102,T-104]`. Báo cáo sau phản ánh đủ ba bản ghi đã xác nhận.

## Root cause
`Where` trả về query deferred. `Count()` và `Select(...).ToArray()` enumerate hai lần, trong khi nguồn mutable bị cập nhật ở giữa. Đây không phải vấn đề thread-safety bắt buộc: lịch trình tuần tự cũng đủ tạo lỗi.

## Fix tham khảo
Thay phần thân `BuildReport` trong `starter/Program.cs` bằng:

```csharp
static CensusReport BuildReport(List<TreeObservation> observations, Action beforeExport)
{
    var snapshot = observations.Where(x => x.IsConfirmed).ToArray();
    var count = snapshot.Length;
    beforeExport();
    var ids = snapshot.Select(x => x.Id).ToArray();
    return new CensusReport(count, ids);
}
```

## Vì sao hiệu quả
Một báo cáo chốt tập observation tại thời điểm bắt đầu. `beforeExport` không thay đổi snapshot đã tạo; báo cáo tiếp theo vẫn có thể đọc source mới. Chạy `./scripts/verify.ps1` để kiểm tra count, thứ tự ID và freshness.

## Trade-offs
`ToArray` tăng bộ nhớ O(n); phù hợp batch nhỏ và yêu cầu snapshot trong một process. Nếu dữ liệu lấy từ DB và nhiều writer, cần transaction isolation hoặc snapshot/version theo DB; materialize phía application **không** tự bảo đảm snapshot nhất quán xuyên nhiều truy vấn hay nhiều service.

## Wrong / tempting fixes
- Gọi `Count()` lần nữa sau export: che sai lệch ở một trường, không định nghĩa thời điểm báo cáo.
- `lock` riêng tại nơi tính count: không bảo vệ toàn bộ ranh giới đọc và các writer không dùng cùng lock.
- Cache query `IEnumerable`: giữ expression, không giữ kết quả.
- Bỏ cập nhật dữ liệu đồng bộ: đánh đổi freshness toàn hệ thống để tránh một bug báo cáo.

## Hồi quy mở rộng
Thử thêm cập nhật trạng thái `IsConfirmed` (với model mutable trong một biến thể) và nguồn dữ liệu lớn; đánh giá memory/latency và snapshot contract.
