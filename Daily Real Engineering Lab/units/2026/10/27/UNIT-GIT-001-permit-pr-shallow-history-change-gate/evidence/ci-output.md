# Evidence snapshot

Trích xuất từ fixture cục bộ; tự chạy reproduce.ps1 để xác minh:

~~~text
$ git -C .lab-runtime/shallow rev-parse --is-shallow-repository
true

$ git -C .lab-runtime/shallow rev-parse HEAD
<feature-head-sha>

$ git -C .lab-runtime/shallow diff HEAD^ HEAD --name-only
fatal: ambiguous argument 'HEAD^': unknown revision or path not in the working tree

$ ./reproduce.ps1
REPRODUCED: CI change gate fails on the local checkout.
~~~

Thông tin bổ sung: source ở HEAD đầy đủ; branch feature có hai file thay đổi so với base được ghi tại .lab-runtime/base.sha. Hãy kiểm chứng các commit object thực sự có trong checkout.
