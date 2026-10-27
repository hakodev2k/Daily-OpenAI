# Reference Solution — chỉ mở sau khi tự điều tra

## 1. Symptoms

Pipeline đối soát xuất thứ tự sai ở batch chứa điều chỉnh âm/dương lớn; dữ liệu thông thường có vẻ đúng. Các record bằng DeltaMinorUnits còn có thứ tự Reference không ổn định theo hợp đồng.

## 2. Evidence

Fixture boundary dùng NEG = -2,000,000,000 và POS = 2,000,000,000. Starter so sánh bằng phép trừ trên int 32-bit; hiệu toán học vượt phạm vi biểu diễn. Fixture tie có cùng 50 minor units nhưng Reference Z-9 xuất hiện trước A-1. Các kết quả này là dự đoán từ static review; chưa chạy runtime trong tác vụ phát hành.

## 3. Root cause

Comparer sử dụng unchecked(left.DeltaMinorUnits - right.DeltaMinorUnits). Phép trừ int bị wraparound: 2,000,000,000 - (-2,000,000,000) không giữ được dấu đúng. Điều này vi phạm quan hệ thứ tự mà Array.Sort cần. Khi hai giá trị bằng nhau, comparer trả 0 dù hợp đồng yêu cầu sắp tiếp theo Reference.

## 4. Why the fix works

Sử dụng left.DeltaMinorUnits.CompareTo(right.DeltaMinorUnits) để lấy quan hệ thứ tự mà không tính hiệu có thể overflow. Nếu hai số bằng nhau, dùng StringComparer.Ordinal.Compare(left.Reference, right.Reference). Như vậy có total order rõ ràng, độc lập với input order.

Source đầy đủ: [Program.cs](Program.cs), [project](SettlementOrderLab.csproj).

~~~csharp
var byAmount = left.DeltaMinorUnits.CompareTo(right.DeltaMinorUnits);
return byAmount != 0
    ? byAmount
    : StringComparer.Ordinal.Compare(left.Reference, right.Reference);
~~~

## 5. How to verify

Sau khi sửa starter/Program.cs, chạy ./verify.ps1 tại root của unit. Cả ordinary, boundary và tie phải PASS. Có thể đối chiếu bản solution riêng bằng:

~~~powershell
dotnet run --project solution/SettlementOrderLab.csproj -- --scenario all
~~~

Không dùng việc chạy solution thay thế cho verify starter đã sửa.

## 6. Alternative fixes

- Comparer<SettlementAdjustment>.Create với CompareTo + tie-break: ngắn hơn nhưng cùng hợp đồng.
- OrderBy(DeltaMinorUnits).ThenBy(Reference, StringComparer.Ordinal): hợp lý cho batch vừa, nhưng có thể cấp phát và thay đổi pipeline; phải đánh giá performance và memory.
- So sánh hiệu bằng long: an toàn với hiệu của hai int nhưng kém trực tiếp hơn CompareTo; cần xét lại nếu model đổi kiểu.

## 7. Wrong or misleading fixes

- Đổi sang Math.Abs của hiệu: phá quan hệ âm/dương và có biên MinValue.
- Đảo dấu kết quả hoặc Reverse toàn bộ output: sửa một fixture nhưng làm hỏng fixture khác.
- Dùng checked để ném OverflowException: phát hiện sai nhưng chưa giải quyết hợp đồng sắp xếp.
- Chỉ sort theo DeltaMinorUnits: bỏ sót tie-break mà partner yêu cầu.

## 8. Production implications

Sai thứ tự có thể làm checksum batch khác nhau, tạo diff đối soát giả, gây retry không cần thiết. Cần contract tests cho giá trị min/max, cặp trái dấu, giá trị bằng nhau, và property tests cho tính phản đối xứng/bắc cầu của comparer.

## 9. Trade-offs

So sánh số bằng CompareTo có chi phí nhỏ và rõ nghĩa. Tie-break ordinal tạo thứ tự xác định giữa môi trường/culture. Nếu Reference không duy nhất, cần xác định thêm một khóa định danh ổn định; không tự ý coi sort ổn định là thay thế hợp đồng.

## 10. Senior engineer should notice

Comparator là một contract có tính chất toán học, không chỉ là một phép tính tối ưu. Test dữ liệu thực tế cần bao phủ giá trị biên, dấu, ties và thứ tự đầu vào. Đừng dựa vào hành vi ổn định ngẫu nhiên của Array.Sort. Khi đối soát production, phân biệt lỗi ordering với lỗi thiếu hoặc trùng record.
