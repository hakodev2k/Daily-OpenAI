# Before
- Cả hai AddMember operation in ra `succeeded`.
- Final members chứa `owner` và chỉ một trong hai member mới.
- Không cần exception để chứng minh failure; inconsistency nằm ở final state.

Nếu không reproduce được, chạy lại `./reproduce.ps1` và không thay đổi các delay trong starter trước khi quan sát timeline.