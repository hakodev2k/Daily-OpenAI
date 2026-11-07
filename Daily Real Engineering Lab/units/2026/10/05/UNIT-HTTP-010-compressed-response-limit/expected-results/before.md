# Expected Results — Before

Fixture tạo một logical payload hơn 8 MB với tỷ lệ gzip rất cao.

Starter client cho thấy policy 1 MB không ngăn application materialize payload lớn. Evidence quan trọng là metadata quan sát được và số bytes thực tế sau khi content được đọc.
