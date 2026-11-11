namespace InventoryLab;
public sealed class StockItem { public string Sku { get; } public int Quantity { get; set; } public StockItem(string sku,int quantity){Sku=sku;Quantity=quantity;} }
public sealed record ReportLine(string Sku,int Quantity);
public sealed record Report(IReadOnlyList<ReportLine> Lines,int Total);
public static class InventoryReport {
 public static Report Build(IEnumerable<StockItem> items,Action afterDetailsPrinted) {
  var rows=items.Select(x=>new ReportLine(x.Sku,x.Quantity));
  var printed=rows.ToList();
  afterDetailsPrinted();
  var total=rows.Sum(x=>x.Quantity);
  return new Report(printed,total);
 }
}
