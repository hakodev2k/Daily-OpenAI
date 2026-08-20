# Course State

## 1. State Context

| Field | Current Value |
|---|---|
| State Purpose | Theo dõi tiến trình tạo nội dung, vị trí curriculum, kế hoạch retrieval và learner evidence mà không phụ thuộc conversation memory |
| Course Generation Status | In progress |
| Phase Generation Status | Phase 0 in progress |
| Learner Performance Status | Not evaluated |
| Learner Study Status | No learner completion recorded |

> `Generated & Accepted` nghĩa là lesson content đã được tạo và vượt publishing gate hiện hành. Trạng thái này không có nghĩa learner đã học hoặc mastered lesson.

## 2. Current Curriculum Position

| Field | Current Value |
|---|---|
| Active Phase | Phase 0 — English Orientation |
| Active Module | P00-M01 — Học cách học và dùng hệ thống |
| Last Accepted Lesson | Day-002 |
| Next Lesson | Day-003 |
| Next Lesson Title | Cách trả lời và cách dùng feedback |
| Next Lesson Type | Foundation / Learning strategy |
| Next Module | P00-M01 — Học cách học và dùng hệ thống |
| Next Action | Create the approved lesson blueprint for Day-003 from the active phase curriculum |
| Phase Transition Status | Not applicable; Phase 0 remains active |

## 3. Generation Progress

### Generated & Accepted Lessons

| Lesson | Title | Publishing Gate | Review | Learner Completion |
|---|---|---|---|---|
| Day-001 | Bắt đầu an toàn: cách dùng một bài học | Accepted with deferred minor fixes | `reviews/Day-001-review.md` | None recorded |
| Day-002 | Nghe trước, transcript sau | Accepted — PASS | `reviews/Day-002-review.md` | None recorded |

### Generated but Not Accepted

None.

### Module Progress

- Current module: P00-M01
- Last accepted lesson: Day-002
- Next planned lesson: Day-003
- Completed modules: None

## 4. Learner Progress

| Field | Status |
|---|---|
| Learner Completed Lessons | None recorded |
| Learner Mastery | Not evaluated |
| Last Learner Assessment | None |
| Learner Remediation Status | Not evaluated |
| Confirmed Learner Errors | None recorded |

## 5. Knowledge Introduced in Course

### Grammar

No grammar concepts introduced in accepted course content.

### Vocabulary

- Target functional labels introduced in course: `listen`, `read`, `choose`, `check`, `pause`, `replay`, `transcript`.
- Learner mastery: Not evaluated.
- Detailed records: `state/VOCABULARY_STATE.md`.

### Pronunciation

- No pronunciation system, IPA or phoneme introduced.
- Day-002 introduces awareness that sound and written transcript are related but different representations; learner performance not evaluated.

### Listening

- Audio-control access and the distinction between technical access failure and language-listening difficulty introduced as operational foundations.
- Day-002 introduces the seven-step Listening routine: first listen → initial record → purposeful replay → response → transcript comparison → final re-listen.
- Learner performance: Not evaluated.

### Reading

- Navigation reading of headings, short Vietnamese instructions, response locations and feedback regions introduced in course.
- Day-002 introduces reading a one-label transcript only after an attempt and using it as direct comparison evidence.
- Learner performance: Not evaluated.

### Learning / Operational Skills

- Four lesson regions: instruction, input/activity, response area, feedback.
- Five-step learning cycle.
- Attempt before feedback/answer reveal.
- Preserve the first-attempt record before support.
- Set a replay goal and return to audio after transcript use.
- Separate content guess, certainty and technical access state.
- Learn Mode versus scored-test purpose.
- Exposure/seen versus independently able.

### TOEIC

No TOEIC parts, questions, timing or score targets introduced.

## 6. Review State

| Field | Current Value |
|---|---|
| Pending Retrieval Targets | 8 curriculum-planned targets |
| Next Immediate Review | Day-003: attempt-before-feedback, Day-002 transcript reveal order and purposeful replay when audio is used |
| Cumulative Review Status | Planned for later module checkpoint; no learner review event completed |
| Review Queue | `state/REVIEW_QUEUE.md` |

## 7. Quality State

| Field | Current Value |
|---|---|
| Latest Review | `reviews/Day-002-review.md` |
| Review Status | PASS |
| Critical Issues | 0 |
| Major Issues | 0 |
| Minor Issues | 0 |
| Deferred Minor Issues | 2 open issues from accepted Day-001 only |
| Publishing Gate | Accepted — PASS |
| Deferred Fix Backlog | `state/DEFERRED_FIXES.md` |

Day-002 passed independent re-review. Its accepted content state does not imply learner completion or mastery. The two open Day-001 deferred MINOR issues remain content-maintenance debt and are not learner remediation.

## 8. Next Pipeline Action

The next AI run must plan exactly **Day-003 — Cách trả lời và cách dùng feedback** using:

- the architecture files;
- `CURRICULUM_MASTER.md`;
- `curriculum/phase-00-english-orientation.md`;
- this state file;
- `state/REVIEW_QUEUE.md`;
- the accepted Day-001 and Day-002 lessons;
- `reviews/Day-002-review.md`;
- vocabulary and skill state files.

Day-003 must retrieve attempt-before-answer and Day-002 transcript timing when audio is used. Do not generate Day-004, update learner mastery, or treat deferred content fixes as learner errors.
