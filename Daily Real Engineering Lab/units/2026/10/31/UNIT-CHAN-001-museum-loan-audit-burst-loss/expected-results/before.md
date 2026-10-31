# Before — starter

Chạy reproduce dự kiến xuất:
~~~text
submitted=9 stored=3
storedSequences=[7,8,9]
missingSequences=[1,2,3,4,5,6]
REPRODUCED: accepted operations are missing from audit storage.
~~~
Đây là kỳ vọng từ static review, chưa phải kết quả chạy trong lượt xuất bản. verify.ps1 dự kiến FAIL ở case 9 hoặc 40; một event đơn lẻ vẫn thành công.
