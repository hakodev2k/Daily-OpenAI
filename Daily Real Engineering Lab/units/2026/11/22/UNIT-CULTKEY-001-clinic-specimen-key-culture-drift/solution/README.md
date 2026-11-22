# Reference solution — UNIT-CULTKEY-001

Root cause: `ToUpper(CultureInfo.CurrentCulture)` biến đổi ký tự `i` khác nhau ở `tr-TR` và `en-US`. Với định danh kỹ thuật ASCII, khóa phải có quy tắc so sánh độc lập locale.

Reference dùng `Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)` cho cả insert và lookup; không normalize bằng culture hiện tại. `OrdinalIgnoreCase` phù hợp với định danh được định nghĩa case-insensitive; nếu hợp đồng yêu cầu case-sensitive thì dùng `Ordinal`. Không áp dụng quy tắc này một cách máy móc cho tên người hay ngôn ngữ tự nhiên.

Sai lầm thường gặp: đổi thành `ToUpperInvariant()` chỉ ở bước lookup nhưng không đổi khi insert; ép culture toàn process để che vấn đề; dùng `CurrentCultureIgnoreCase` vẫn phụ thuộc locale; chuyển mọi thứ thành lowercase mà không có hợp đồng định danh.

Kiểm tra `dotnet run --project solution/SpecimenLab.csproj -- --verify` (chỉ để đối chiếu sau khi tự làm). Script verify chính vẫn chạy `starter/`. Khi mở rộng, bổ sung dữ liệu Unicode và phân biệt quy tắc canonicalization với chính sách định danh business.
