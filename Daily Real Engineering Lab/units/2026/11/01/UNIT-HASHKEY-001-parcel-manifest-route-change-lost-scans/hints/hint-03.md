# Hint 3
Kiểm tra `IEqualityComparer<Parcel>`, đặc biệt tính nhất quán của `Equals` và `GetHashCode` trong toàn bộ vòng đời key. Một thuộc tính thay đổi sau insert có thể khiến lookup dùng bucket khác. Hãy xác định identity ổn định theo yêu cầu nghiệp vụ.
