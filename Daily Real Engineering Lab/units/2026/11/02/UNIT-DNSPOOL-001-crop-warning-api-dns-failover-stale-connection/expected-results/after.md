# After
`./verify.ps1` trên starter đã sửa trả 0 nếu 2/2 trước cutover và 5/5 sau cutover thành công, không có post-cutover failure, tối đa 5 connections và 5 DNS lookups. Policy quá ngắn có thể vượt connection budget.
