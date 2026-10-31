# Expected Results

## Before
- 'run.ps1' in stable 250ms, backward-step -29750ms, forward-step 45250ms.
- 'reproduce.ps1' thành công khi phát hiện triệu chứng gốc.
- 'verify.ps1' thất bại trên starter gốc.

## After
- Cả ba duration là 250ms.
- UTC start/end vẫn phản ánh clock adjustment của fixture.
- 'verify.ps1' thành công; 'reproduce.ps1' không còn xác nhận lỗi.
- Không phụ thuộc tốc độ máy; simulator không sleep.
