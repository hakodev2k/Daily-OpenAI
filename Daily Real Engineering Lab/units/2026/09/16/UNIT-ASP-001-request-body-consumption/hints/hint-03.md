# Hint 3

ASP.NET Core cung cấp cơ chế cho phép request body được đọc nhiều lần trong middleware scenarios. Sau khi audit đọc xong, downstream cần nhìn stream ở trạng thái tương đương trước lần đọc đó.