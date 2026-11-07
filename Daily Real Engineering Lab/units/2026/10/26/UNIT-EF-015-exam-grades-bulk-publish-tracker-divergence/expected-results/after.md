# After — learner fix

~~~powershell
./verify.ps1
# Persisted: Id=41, Score=92, Status=Published, Note=QA-reviewed
# CONTRACT PASS (exit code 0)
~~~

Đây là assertion trạng thái persisted, không phải chỉ là output của một tracked object. Sau khi sửa, reproduce.ps1 không còn pass vì symptom ban đầu đã biến mất — điều này là bình thường.
