# After

Tại checkpoint `visible=north:r1,south:r1`, `consistent=True`; sau publish `visible=north:r2,south:r2`, `consistent=True`. `verify.ps1` chỉ PASS khi cả hai snapshot đúng và không có bản ghép.
