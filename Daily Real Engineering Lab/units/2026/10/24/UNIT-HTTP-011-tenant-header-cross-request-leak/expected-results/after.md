# After — starter do bạn sửa
`./scripts/verify.ps1` exit **0** khi serial và overlap đều cho `order-a → tenant-alpha`, `order-b → tenant-beta`. `./scripts/reproduce.ps1` khi đó exit **1** vì lỗi cũ không còn; đây là hành vi chủ đích. Cả hai request vẫn trả HTTP 202 qua fake handler.
