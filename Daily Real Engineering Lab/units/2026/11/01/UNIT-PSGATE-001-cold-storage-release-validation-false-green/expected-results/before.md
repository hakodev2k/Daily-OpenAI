# Before — trạng thái starter
Chạy `scripts/reproduce.ps1` phải in `REPRODUCED` và trả exit code 0 vì đã xác nhận triệu chứng. Validator trong nhánh `fail` in `VALIDATOR_FAIL` nhưng wrapper cũng in `QUALITY_GATE_PASS`; tiến trình cha quan sát exit code 0. `scripts/verify.ps1` phải trả nonzero.
