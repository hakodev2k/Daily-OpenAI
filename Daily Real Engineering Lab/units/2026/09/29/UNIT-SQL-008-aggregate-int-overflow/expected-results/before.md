# Expected Results — Before

Sau `setup.ps1`, starter query phải fail với SQL Server error 8115.

Dataset cố định:

- 1,100,000,000
- 1,100,000,000
- 1,100,000,000

Tổng toán học mong đợi là:

~~~text
3300000000
~~~

Không row riêng lẻ nào vượt range của `int`, nhưng starter aggregate không trả được kết quả.
