# Hint 2

EF Core chỉ tự sinh update cho entity mà DbContext đang theo dõi với state phù hợp. So sánh cách entity được load với state mà `ChangeTracker` nhìn thấy.
