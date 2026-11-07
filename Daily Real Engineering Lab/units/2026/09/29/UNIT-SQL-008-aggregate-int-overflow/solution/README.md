# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Query hoạt động với dataset nhỏ nhưng fail bằng SQL Server error 8115 khi tổng của nhiều `AmountCents` lớn hơn trước.

Từng row vẫn là giá trị `int` hợp lệ.

## 2. Evidence

Dataset có ba row, mỗi row bằng `1,100,000,000`.

Tổng mong đợi là `3,300,000,000`, lớn hơn maximum của SQL Server `int` là `2,147,483,647`.

Starter query aggregate trực tiếp expression kiểu `int`.

## 3. Root cause

Trong SQL Server, `SUM` trên expression kiểu `int` trả kết quả kiểu `int`.

Do đó accumulator của aggregate không tự động trở thành `bigint` chỉ vì tổng toán học cần range lớn hơn.

Khi phép cộng vượt range của `int`, SQL Server phát sinh arithmetic overflow trước khi có thể trả result.

## 4. Why the fix works

Reference query widen **input expression** trước khi aggregate:

~~~sql
SELECT SUM(CAST(AmountCents AS bigint)) AS TotalCents
FROM dbo.SettlementLine
WHERE MerchantId = 42;
~~~

Bây giờ `SUM` nhận expression kiểu `bigint`, vì vậy result type cũng có range phù hợp và trả `3300000000`.

## 5. How to verify

Áp dụng fix vào `starter/query.sql` rồi chạy:

~~~powershell
./verify.ps1
~~~

Script reset dataset, chạy learner query và yêu cầu exact result `3300000000`.

## 6. Alternative fixes

### Thay schema thành bigint

Nếu domain cho phép **một settlement line riêng lẻ** có thể vượt range của `int`, đổi column sang `bigint` có thể là quyết định đúng.

Nếu từng row luôn nhỏ nhưng aggregate có thể lớn, chỉ widen expression tại aggregate có thể ít invasive hơn.

### Dùng decimal

Nếu monetary model cần fractional unit hoặc precision khác cents-as-integer, `decimal(p,s)` có thể phù hợp hơn. Đây là quyết định domain/schema, không phải fix mặc định cho mọi hệ thống.

## 7. Wrong or misleading fixes

### CAST bên ngoài SUM

~~~sql
CAST(SUM(AmountCents) AS bigint)
~~~

Không giải quyết root cause. Overflow xảy ra trong phép `SUM(int)` trước khi outer cast được áp dụng.

### Catch error rồi trả partial total

Settlement total phải chính xác; partial result làm sai đối soát.

### Chia dataset thành nhiều query nhỏ bằng tay

Có thể né threshold trong một số trường hợp nhưng tạo correctness và concurrency problems, đồng thời không sửa type contract của aggregate.

### Hard-code hoặc clamp về int max

Che lỗi bằng dữ liệu sai.

## 8. Production implications

Type safety cần xét cả **giá trị từng row** và **range của phép tổng hợp**.

Một schema có thể lưu mọi record hợp lệ nhưng reporting/analytics vẫn fail khi aggregate vượt range return type.

Ở application boundary, kết quả này cũng nên map sang .NET `long`, không phải `int`.

## 9. Trade-offs

Widen tại query là thay đổi nhỏ, rõ intent và tránh migration nếu row domain vẫn phù hợp với `int`.

Đổi schema sang `bigint` tăng storage/index width nhưng có thể đúng hơn nếu domain của từng row cũng đang lớn dần.

`decimal` phù hợp khi monetary precision yêu cầu khác, nhưng có storage/CPU/type-mapping trade-offs riêng.

## 10. What a Senior engineer should notice

Senior engineer nên hỏi:

- range của source values là bao nhiêu?
- range của **aggregate** là bao nhiêu?
- SQL return type contract là gì?
- .NET type nhận result có đủ range không?
- đây là query-local issue hay dấu hiệu schema domain đã thay đổi?
- regression test nào sẽ giữ lại threshold case này?

Điểm chính là reasoning về end-to-end numeric range, không chỉ sửa một câu SQL cho hết lỗi.
