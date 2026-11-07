# Expected results — learner fix
`./verify.ps1` exit **0** và 5 case PASS. 4 giây < budget 10 giây; 12 giây > budget; đúng 10 giây thì expired; `Start()` lại reset cửa sổ. `./reproduce.ps1` thường exit **1** sau fix vì lỗi starter đã biến mất.
