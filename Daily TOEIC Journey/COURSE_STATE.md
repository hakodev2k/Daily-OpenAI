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
| Last Accepted Lesson | Day-001 |
| Next Lesson | Day-002 |
| Next Lesson Title | Nghe trước, transcript sau |
| Next Lesson Type | Foundation / Listening routine |
| Next Module | P00-M01 — Học cách học và dùng hệ thống |
| Next Action | Create the approved lesson blueprint for Day-002 from the active phase curriculum |
| Phase Transition Status | Not applicable; Phase 0 remains active |

## 3. Generation Progress

### Generated & Accepted Lessons

| Lesson | Title | Publishing Gate | Review | Learner Completion |
|---|---|---|---|---|
| Day-001 | Bắt đầu an toàn: cách dùng một bài học | Accepted with deferred minor fixes | `reviews/Day-001-review.md` | None recorded |

### Generated but Not Accepted

None.

### Module Progress

- Current module: P00-M01
- Last accepted lesson: Day-001
- Next planned lesson: Day-002
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

- Target interface labels introduced in course: `listen`, `read`, `choose`, `check`.
- Learner mastery: Not evaluated.
- Detailed records: `state/VOCABULARY_STATE.md`.

### Pronunciation

- No pronunciation system or phoneme introduced.
- Day-001 contains audio/speech awareness only; learner performance not evaluated.

### Listening

- Audio-control access and the distinction between technical access failure and language-listening difficulty introduced as operational foundations.
- Learner performance: Not evaluated.

### Reading

- Navigation reading of headings, short Vietnamese instructions, response locations and feedback regions introduced in course.
- Learner performance: Not evaluated.

### Learning / Operational Skills

- Four lesson regions: instruction, input/activity, response area, feedback.
- Five-step learning cycle.
- Attempt before feedback/answer reveal.
- Learn Mode versus scored-test purpose.
- Exposure/seen versus independently able.

### TOEIC

No TOEIC parts, questions, timing or score targets introduced.

## 6. Review State

| Field | Current Value |
|---|---|
| Pending Retrieval Targets | 6 curriculum-planned targets |
| Next Immediate Review | Day-002: lesson navigation, attempt-before-feedback, `listen` cue and access-vs-performance distinction |
| Cumulative Review Status | Planned for later module checkpoint; no learner review event completed |
| Review Queue | `state/REVIEW_QUEUE.md` |

## 7. Quality State

| Field | Current Value |
|---|---|
| Latest Review | `reviews/Day-001-review.md` |
| Review Status | PASS WITH MINOR FIXES |
| Critical Issues | 0 |
| Major Issues | 0 |
| Deferred Minor Issues | 2 |
| Publishing Gate | Accepted with deferred minor fixes under owner-authorized policy |
| Deferred Fix Backlog | `state/DEFERRED_FIXES.md` |

Content-quality debt is separate from learner remediation. No learner remediation has been inferred from review findings.

## 8. Next Pipeline Action

The next AI run must plan exactly **Day-002 — Nghe trước, transcript sau** using:

- the architecture files;
- `CURRICULUM_MASTER.md`;
- `curriculum/phase-00-english-orientation.md`;
- this state file;
- `state/REVIEW_QUEUE.md`;
- vocabulary and skill state files.

Day-002 must retrieve the Day-001 navigation/attempt routine. Do not generate Day-003, update learner mastery or treat deferred content fixes as learner errors.
