# Reference Solution — spoiler

## Evidence

Cùng `DateOnly(2026, 10, 5)` tạo chuỗi khác nhau khi `CurrentCulture` thay đổi. Starter formatter dùng overload không chỉ định contract format.

## Root cause

Production code vô tình dùng process culture như một phần của integration protocol. Culture là ambient state của environment, nên machine/runner khác nhau có thể tạo wire value khác nhau.

## Fix

Dùng explicit partner format và culture-independent formatting tại integration boundary:

~~~csharp
return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
~~~

## Wrong fixes

- Đổi culture của toàn process trong production để test pass: tạo global side effects và che assumption.
- Sửa expected value theo CI machine: biến partner contract thành environment-dependent.
- Dùng timezone conversion: input là `DateOnly`; timezone không giải thích symptom.
- Skip test trên Linux: bỏ regression signal thay vì sửa contract.

## Production implications

Formatting cho UI có thể culture-sensitive. Formatting cho protocol, cache key, signature input, persistence key hoặc external integration thường cần contract explicit.

## Senior-level takeaway

Khi test chỉ fail theo environment, hãy inventory ambient dependencies: culture, timezone, filesystem case sensitivity, path separator, line endings, environment variables, clock và runtime version. Sau đó biến dependency quan trọng thành explicit contract hoặc kiểm soát nó tại test boundary.
