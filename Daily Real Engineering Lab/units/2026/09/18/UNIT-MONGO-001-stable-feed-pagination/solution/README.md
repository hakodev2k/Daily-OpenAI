# Reference Solution

> Reference Solution — chỉ xem sau khi reproduce và tự thử fix.

## Symptoms
Hai page liên tiếp có thể overlap hoặc bỏ sót item khi feed nhận insert mới.

## Evidence
Page 1 đọc `[5,4]`; sau khi item `6` được insert ở đầu ordering, page 2 theo relative offset có thể đọc `[4,3]`.

## Root cause
Offset pagination (`Skip`) biểu diễn vị trí tương đối trong dataset hiện tại. Insert trước offset làm vị trí của các item cũ dịch chuyển giữa requests.

## Why the fix works
Dùng seek/cursor pagination dựa trên sort key ổn định `(CreatedAt, Id)`. Page tiếp theo yêu cầu item có key nhỏ hơn key cuối của page trước thay vì tính lại relative offset.

## Reference implementation
```csharp
static IReadOnlyList<FeedItem> GetNextPage(List<FeedItem> source, int size, FeedItem? cursor)
{
    var query = source.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).AsEnumerable();
    if (cursor is not null)
        query = query.Where(x => x.CreatedAt < cursor.CreatedAt ||
            (x.CreatedAt == cursor.CreatedAt && x.Id < cursor.Id));
    return query.Take(size).ToList();
}
```

## How to verify
Lấy page đầu, lưu last item làm cursor, insert item mới đứng trước page đầu, rồi lấy page tiếp theo. Hai page không overlap và page tiếp theo tiếp tục sau cursor cũ.

## Alternative fixes
Snapshot/read-consistency strategy có thể phù hợp nếu business yêu cầu một immutable view trong toàn browsing session, nhưng có chi phí state và storage/query semantics khác.

## Wrong or tempting fixes
Tăng page size chỉ giảm xác suất quan sát, không sửa contract. Sort chỉ theo timestamp cũng chưa đủ nếu nhiều item có cùng timestamp. Retry cùng page number vẫn tính offset trên dataset mới.

## Production implications
Cursor cần encode đầy đủ deterministic ordering key, validate direction/filter contract và không nên phụ thuộc vào mutable field.

## Trade-offs
Seek pagination ổn định và hiệu quả cho sequential traversal nhưng không hỗ trợ random page number tự nhiên như offset pagination.

## What a Senior engineer should notice
Pagination là API consistency contract. Chọn strategy phải dựa trên mutability, ordering uniqueness, navigation requirements và index design chứ không chỉ syntax của database driver.