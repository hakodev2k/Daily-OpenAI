# After — sau khi tự sửa starter

`pwsh -File ./verify.ps1` phải exit 0 với các quan sát:

```text
alpha.afterLogin=OK session=alpha
beta.beforeLogin=Unauthorized session=none
beta.afterLogin=OK session=beta
alpha.afterBetaLogin=OK session=alpha
PASS: tenant sessions isolated
```

`reproduce.ps1` lúc này exit 1 (`NOT_REPRODUCED`) vì triệu chứng cũ không còn; đây là chủ đích. Không chấp nhận một fix khiến cả Alpha lẫn Beta luôn 401 hoặc khiến session mất sau lần request thứ hai.
