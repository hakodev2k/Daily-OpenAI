# After

- verify.ps1 trả exit code 0, bucket của cùng identity giống nhau trên hai replica.
- 0% không bật ai; 100% bật tất cả; cohort 20% là tập con của 40%.
- Cohort phân tách tenant và phân phối 50% trên fixture đủ rộng.
- reproduce.ps1 sau khi fix trả nonzero vì lỗi không còn; đây là điều được mong đợi.
- Trước production rollout, cần versioning/migration để tránh rebucket người dùng cũ.
