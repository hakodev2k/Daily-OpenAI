# Reference solution — spoiler

`Select` deferred: `ToList()` chụp chi tiết trước callback, `rows.Sum()` duyệt lại nguồn sau callback. Sửa `var rows=items.Select(x=>new ReportLine(x.Sku,x.Quantity));` thành `var rows=items.Select(x=>new ReportLine(x.Sku,x.Quantity)).ToArray();` trong `starter/InventoryReport.cs`. Verify phải PASS với tổng 20.

Wrong fixes: `AsEnumerable()` không materialize; chỉ khóa reader không bảo vệ writer; chụp sau callback đổi ý nghĩa report. Trade-off: O(n) memory; nguồn DB cần transaction isolation nếu dữ liệu đổi ngay trong khi materialize.
