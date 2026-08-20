# Day-003 Blueprint — Cách trả lời và cách dùng feedback

## 1. Lesson Identity

| Field | Planned Value |
|---|---|
| Lesson | `Day-003` |
| Title | Cách trả lời và cách dùng feedback |
| Phase | Phase 0 — English Orientation (`P00`) |
| Module | `P00-M01` — Học cách học và dùng hệ thống |
| Level Orientation | Zero → Pre-A1 |
| Lesson Type | Foundation / Learning strategy |
| Dominant Mode | Learn Mode |
| TOEIC Orientation | Chưa tích hợp TOEIC; xây thói quen dùng feedback trước khi học nội dung ngôn ngữ |
| Operational Source | `COURSE_STATE.md`: `Next Lesson: Day-003` |
| Remediation Override | Không có; learner performance và mastery chưa được đánh giá |
| Planned Output of Next Stage | Một lesson hoàn chỉnh cho `Day-003`; blueprint này không phải final lesson |

## 2. Planning Decision and Scope Lock

Blueprint này lập kế hoạch cho đúng một lesson: **Day-003 — Cách trả lời và cách dùng feedback**.

Primary Goal ràng buộc:

> Người học phân biệt được **câu trả lời của mình**, **đáp án đúng**, **giải thích/dấu hiệu**, và **lần làm lại**; sau đó sửa response dựa trên lý do thay vì chỉ nhìn hoặc chép đáp án.

Lesson Generator không được mở rộng bài thành:

- bài grammar, pronunciation, phonics, alphabet hoặc IPA;
- bài vocabulary ngoài năm nhãn chức năng đã được duyệt;
- hệ thống mistake log hoặc full error taxonomy của Day-004;
- bài TOEIC thật, bài test tính điểm hoặc đánh giá level;
- bài kiểm tra khả năng đọc English instruction;
- bài nghe bắt buộc nếu audio access chưa được xác nhận;
- bài cập nhật learner error patterns, mastery hoặc `COURSE_STATE.md`.

## 3. Source-of-Truth Audit

Blueprint phải được thực thi theo các quyết định đã xác nhận từ repository:

- `COURSE_SPEC.md`: Vietnamese-led, Learn Mode cho phép hints và retry sau feedback; correct letter/word alone chưa đủ, explanation phải làm rõ answer, clue/reason và contrast khi hữu ích.
- `ROADMAP.md`: Phase 0 ưu tiên learning conventions và system use, chưa tạo TOEIC pressure.
- `LEARNING_SYSTEM.md`: guided practice phải fade support; independent attempt không lộ hint; feedback đi trước retry; recurring learner error patterns chỉ được tạo từ actual evidence.
- `KNOWLEDGE_DEPENDENCIES.md`: navigation, honest attempt và learning routine là prerequisites trước nội dung English.
- `CURRICULUM_MASTER.md`: `P00-M01` xây cách dùng hệ thống; Day-003 thuộc foundation/learning strategy.
- `curriculum/phase-00-english-orientation.md`: Day-003 dạy response formats, feedback anatomy, correct answer versus explanation, mistake versus misconception và reason-based retry.
- `COURSE_STATE.md`: Day-002 đã accepted; Day-003 là next authorized lesson; không có learner remediation evidence.
- `lessons/phase-00/Day-001.md`: đã giới thiệu lesson regions, `choose`, Learn Mode, attempt-before-feedback và technical access khác performance.
- `lessons/phase-00/Day-002.md`: đã giới thiệu first listen, purposeful replay, response-before-transcript và transcript comparison.
- Latest `reviews/Day-002-review.md`: PASS; Day-003 phải giữ attempt-before-answer và transcript timing khi có audio.
- State review queue: retrieve Day-001 attempt-before-answer; nếu dùng audio, retrieve Day-002 transcript reveal order; chuẩn bị Day-004 mistake log nhưng không dạy trước.

Không có `specs/` files trong repository tại thời điểm planning; không được giả định standard chưa tồn tại.

## 4. Why This Lesson Exists

Nhìn thấy đáp án không đồng nghĩa với hiểu. Learner có thể chọn sai, mở answer, nhớ vị trí hoặc chép lại mà vẫn giữ nguyên cách hiểu gây ra lỗi. Feedback chỉ có giá trị khi nó thay đổi điều learner chú ý hoặc hiểu, rồi sự thay đổi đó được kiểm tra bằng một retry không thể giải bằng trí nhớ vị trí.

Day-003 xây cycle tối thiểu:

1. trả lời trước;
2. đọc feedback sau attempt;
3. tìm đáp án đúng;
4. tìm dấu hiệu và lý do;
5. nói điểm khác giữa response cũ và rule đúng;
6. làm lại một item tương đương với arrangement thay đổi.

Lesson không nhằm chứng minh learner biết English. Mock content phải trung tính về ngôn ngữ để evidence phản ánh cách dùng feedback, không phản ánh vocabulary hoặc reading level.

## 5. Lesson Central Question

> **Làm thế nào feedback giúp tôi hiểu và trả lời lại tốt hơn, thay vì chỉ chép đáp án?**

Final lesson phải quay lại câu hỏi này trong self-explanation, mastery check và one-minute summary.

## 6. New Knowledge Budget

### New Core Knowledge — phải học ngay

1. Bốn vai trò khác nhau: **response**, **correct answer**, **explanation với clue/reason**, **retry**.
2. Feedback cycle: `attempt → feedback → locate answer → locate clue/reason → explain difference → retry`.
3. Retry đúng phải dựa trên reason và áp dụng được khi vị trí/hình thức thay đổi.
4. Correct answer cho biết “cái nào đúng”; explanation cho biết “vì sao đúng” và “dấu hiệu nào dẫn tới đó”.

### Supporting Knowledge — chỉ đủ để vận hành core

- Ba response formats: choose, match, short response.
- Cấu trúc feedback block với Vietnamese-first labels.
- `mistake` là một response/action sai; `misconception` là cách hiểu sai có thể làm lỗi lặp lại. Đây chỉ là awareness, không phải taxonomy hay learner diagnosis.
- Có thể restate clue bằng lời của learner trước retry.

### Review Knowledge — đã xuất hiện

- Day-001: attempt trước answer/feedback; cue `choose`; lesson region `feedback`.
- Day-002: response trước transcript; transcript chỉ mở sau attempt nếu audio được dùng.
- `read` và `check` có thể tái dùng với Vietnamese support nhưng không phải new targets.

### Useful Extension

- Ghi một câu ngắn: “Lần đầu tôi đã nhìn vào ___; lần làm lại tôi sẽ nhìn vào ___.”
- Nói mức chắc chắn sau khi giải thích reason, không dùng confidence làm điểm số.

### Future Preview

- Day-004: đưa actual learner mistakes vào mistake log và review queue.
- Future lessons: phân loại error, spaced review và feedback cho grammar/listening/TOEIC.

## 7. Prerequisite Audit

| Prerequisite | Status | Planning Action |
|---|---|---|
| Tìm response area và feedback area | Introduced Day-001; learner performance not evaluated | Retrieve nhanh trong changed layout |
| Attempt trước answer/feedback | Introduced Day-001 | Bắt buộc làm gate trước mọi mock feedback |
| Cue `choose` | Introduced Day-001 | Review với gloss tiếng Việt |
| Response-before-transcript | Introduced Day-002 | Chỉ retrieve nếu audio branch hoạt động |
| Dùng transcript sau attempt | Introduced Day-002 | Không giả định mastered; prompt sequence rõ |
| English reading | Not established | Không dùng làm hidden prerequisite |
| Grammar/pronunciation | Not taught | Không yêu cầu |
| Audio access | Unknown | Audio optional; text-equivalent route phải hoàn chỉnh |

Nếu learner không tìm được feedback region hoặc mở feedback trước attempt, lesson phải chuyển sang navigation/process remediation chứ không gọi đó là English error.

## 8. Terminology Audit

| Term | Classification | Learner-facing treatment |
|---|---|---|
| `choose` | Review + target functional label | “chọn (`choose`)” |
| `match` | New target functional label | “nối/ghép tương ứng (`match`)” |
| `correct` | New target functional label | “đúng (`correct`)” |
| `incorrect` | New target functional label | “chưa đúng (`incorrect`)”; tránh shame language |
| `try again` | New target functional phrase | “làm lại (`try again`)” |
| feedback | Stable course term | Dùng lần đầu: “phản hồi (`feedback`)”; sau đó ưu tiên tiếng Việt |
| response | Support term, not memorization target | “câu trả lời của tôi” trước, English trong ngoặc khi cần |
| clue | Support term | “dấu hiệu/gợi ý trong item” |
| reason / why | Support term | “lý do / vì sao” |
| retry | Support term | “lần làm lại”; không bắt memorization |
| mistake | Minimal awareness | “một lần trả lời/hành động sai” |
| misconception | Minimal awareness | “cách hiểu sai có thể gây lỗi lặp lại” |

Không dùng unglossed meta-English như `option`, `item`, `rationale`, `diagnosis`, `transfer` trong learner-facing text.

## 9. Learning Objectives

### Knowledge

Learner có thể nói bằng tiếng Việt:

- bốn phần của feedback cycle làm gì;
- correct answer khác explanation như thế nào;
- vì sao retry phải dựa trên reason, không dựa trên vị trí đáp án.

### Recognition

Learner nhận ra `choose`, `match`, `correct`, `incorrect`, `try again` khi có Vietnamese support.

### Process

Learner thực hiện hai complete mock cycles:

1. trả lời trước;
2. mở feedback;
3. chỉ ra answer, clue và reason;
4. giải thích chỗ response cũ lệch;
5. retry trên equivalent item có layout/position thay đổi.

### Reading

Learner đọc một feedback block rất ngắn và tìm được vùng “đúng” và “vì sao/dấu hiệu” mà không bị đánh giá English-instruction ability.

### Listening

Supporting only. Nếu audio hoạt động ổn định, learner thực hiện một ultra-short activity theo Day-002 sequence; không có listening score objective.

### TOEIC / Grammar / Pronunciation

Not applicable to this lesson. Các mảng này không phải target của Day-003.

## 10. Success Criteria

Final lesson phải tạo cơ hội thu observable evidence rằng learner:

- phân loại đúng response, correct answer, clue/reason và retry trong ít nhất hai feedback blocks;
- giải thích được answer alone khác explanation;
- hoàn thành hai retries dựa trên cùng rule nhưng với arrangement thay đổi;
- không mở feedback trước initial response trong assessed cycles;
- xử lý được choose, match và short response ở mức thao tác;
- phân biệt một mistake đơn lẻ với một possible misconception ở mức scenario, không tự chẩn đoán learner state;
- nếu có audio, giữ transcript đóng đến sau response;
- nói được một câu self-explanation về điều đã thay đổi từ attempt sang retry.

Đây là evidence requirements, không phải kết quả đã đạt. Không invent percentage hoặc pass.

## 11. Knowledge Map

```text
Day-001: attempt trước feedback
        +
Day-002: response trước transcript (nếu có audio)
        ↓
response → correct answer → clue/reason → retry
        ↓
reason-based correction trong layout mới
        ↓
Day-004: mistake log + review queue từ actual evidence
```

## 12. Core Mental Model

Final lesson dùng một flow nhất quán:

```text
TỰ TRẢ LỜI → MỞ PHẢN HỒI → TÌM ĐÁP ÁN → TÌM DẤU HIỆU/LÝ DO
                                                        ↓
      LÀM LẠI VỚI BỐ CỤC MỚI ← NÓI ĐIỀU CẦN ĐỔI
```

Anchor sentence bằng tiếng Việt:

> “Đáp án cho tôi biết cái gì đúng; giải thích cho tôi biết vì sao; làm lại kiểm tra xem tôi đã hiểu chưa.”

## 13. Concept Teaching Order

1. Retrieve rule “attempt trước feedback”.
2. Cho thấy một correct answer không có explanation và hỏi learner còn thiếu gì.
3. Dạy bốn role bằng một complete neutral feedback block.
4. Contrast correct answer versus explanation.
5. Dạy clue và reason bằng pointing task, không bằng definition-heavy lecture.
6. Demonstrate flawed retry: chép vị trí cũ.
7. Demonstrate strong retry: dùng same rule trong arrangement mới.
8. Introduce mistake versus misconception awareness.
9. Guided cycle với support đầy đủ.
10. Faded cycle, independent cycles, mixed retrieval và mastery check.

Không đưa error taxonomy hoặc Day-004 log vào trước khi core cycle ổn định.

## 14. Required Explanation Angles

Generator phải giải thích core từ ít nhất năm góc:

- **Function:** mỗi phần của cycle trả lời câu hỏi gì.
- **Contrast:** answer versus explanation; mistake versus misconception.
- **Sequence:** vì sao attempt phải trước feedback và retry phải sau understanding.
- **Evidence:** changed-layout retry cho biết learner hiểu reason tốt hơn việc chọn lại cùng vị trí.
- **Safety:** mock error không phải learner weakness; technical/audio issue không phải language error.

## 15. Intuitive Explanation Requirement

Dùng analogy nhẹ: xem đường đi trên bản đồ khác với tự đi lại đoạn đường. Correct answer giống điểm đến; clue/reason giống chỉ dẫn đường; retry là tự đi lại với điểm xuất phát hoặc bố cục thay đổi.

Analogy chỉ hỗ trợ intuition. Sau analogy phải quay ngay về one concrete feedback block.

## 16. Example Progression

### Example A — Correct answer alone is incomplete

Neutral `choose` task bằng symbols hoặc Vietnamese labels. Show response and correct answer only. Learner identifies what is still unknown: clue/reason.

### Example B — Complete feedback anatomy

Same low-stakes task, now display all fields:

- Câu trả lời của tôi
- Đáp án đúng
- Dấu hiệu
- Lý do
- Vì sao lựa chọn khác không phù hợp (chỉ khi useful)
- Làm lại

### Example C — `match`

Match identical shapes/icons or a simple visual rule. Feedback points to the visible relation, not English knowledge.

### Example D — Weak retry versus strong retry

Weak: remember “option 2”. Strong: locate the defining clue after answer order changes. Ask learner to explain why only the second demonstrates understanding.

### Example E — Short response

Use a one-symbol, one-number or Vietnamese word response. Avoid spelling assessment. Feedback requires learner to restate reason before retry.

### Example F — Optional audio branch

Only if a stable, validated ultra-short asset is available. Keep transcript closed, take response, then reveal feedback/transcript. If unavailable, replace with a text-equivalent activity; do not create missing evidence.

## 17. Example Analysis Requirements

For every worked example, final lesson must identify:

1. learner response;
2. correct answer;
3. exact clue;
4. reason connecting clue to answer;
5. why a tempting wrong response fails when educationally useful;
6. what must change in retry;
7. whether retry changes layout sufficiently to block position memory.

Examples must not imply the mock response came from the actual learner.

## 18. Vocabulary Plan

| Target | Function | Introduction | Retrieval |
|---|---|---|---|
| `choose` | select one response | Brief review from Day-001 | Opening retrieval + mixed practice |
| `match` | connect corresponding elements | Demonstrate with visual pairs | Guided + independent task |
| `correct` | label answer as right | Feedback block | Role-location task + quiz |
| `incorrect` | label response as not yet right | Contrast with `correct`; non-shaming gloss | Feedback reading + correction |
| `try again` | begin retry | Attach to changed-layout retry | Both mastery cycles |

Recognition and functional use matter more than memorized definitions. Do not add a large vocabulary list. `feedback`, `clue`, `reason`, `response`, `retry` are Vietnamese-first support terms, not target memorization items.

## 19. Pronunciation Plan

Not applicable to this lesson. Do not require repetition, phonemic analysis, IPA, stress or pronunciation scoring for the five functional labels. Optional audio playback of a label may exist only as access/support, not as mastery evidence.

## 20. Listening Plan

Listening is optional and supporting only.

If and only if a stable asset is confirmed:

1. state a simple purpose;
2. keep transcript hidden;
3. play one ultra-short neutral signal or taught label;
4. record response;
5. reveal feedback and transcript;
6. locate clue/reason;
7. replay after comparison.

If asset access is uncertain or fails, switch to the complete text-equivalent route. Record technical status separately; do not mark incorrect listening or lower mastery.

## 21. Required Audio Assets

No new audio asset is required for core mastery. Generator may reuse a validated Day-002 neutral asset only when its path, controls and transcript behavior are confirmed in the repository/runtime. It must not invent a filename.

Fallback requirement: every concept, guided cycle and mastery criterion must remain achievable without audio.

## 22. Reading Plan

Reading material is limited to tiny Vietnamese-first feedback blocks with the five English functional labels glossed. Tasks:

- point to `correct`;
- point to the clue;
- underline or restate “why” in Vietnamese;
- find the `try again` instruction.

No paragraph reading, English passage comprehension or speed requirement.

## 23. TOEIC Application Plan

Not applicable to this lesson. Do not create a TOEIC-format item or use TOEIC scoring. One sentence may preview that the same feedback cycle will later help explain traps, but this cannot become practice content.

## 24. Paraphrasing Plan

Required functional equivalence:

- `try again` ↔ “làm lại”.

Optional supporting equivalences:

- `correct` ↔ “đúng”;
- `incorrect` ↔ “chưa đúng”.

Do not teach synonym families. The aim is recognizing course instructions.

## 25. Required Comparisons

### Correct answer versus explanation

| Correct answer | Explanation |
|---|---|
| Identifies what is right | Shows why it is right |
| May be copied | Provides a rule/clue usable elsewhere |
| Does not by itself reveal misunderstanding | Helps identify what attention or reasoning should change |

### Mistake versus misconception

| Mistake | Misconception |
|---|---|
| One incorrect response/action | A wrong understanding that may cause repeated mistakes |
| Can happen by slip or inattention | Needs recurring actual evidence before being treated as a pattern |
| Mock example may illustrate it | Must not be written into learner state from mock content |

### Retry by position versus retry by reason

Changed-layout examples must make the distinction observable.

## 26. Common Mistakes and Preventive Teaching

| Anticipated behavior | Prevention / Response |
|---|---|
| Opens answer before attempting | Closed reveal region and explicit attempt gate |
| Copies correct answer only | Require clue/reason location before retry |
| Treats explanation as a longer answer | Ask separate “what?” and “why?” questions |
| Retries by remembered position | Shuffle/rearrange equivalent item |
| Reads every field but cannot state change | Sentence frame: “Tôi đã chú ý __; lần này tôi chú ý __ vì __.” |
| Calls one mock error a learner pattern | Explicitly label mock content and prohibit state inference |
| Confuses technical audio failure with incorrect response | Separate access branch and text fallback |
| Feels `incorrect` means personal failure | Gloss as “câu trả lời này chưa đúng”, not identity judgment |

## 27. Vietnamese Learner Considerations

- Explanations of learning terms must be in Vietnamese.
- English labels always receive immediate Vietnamese gloss on first use.
- Do not assess ability to read English instructions.
- Prefer icons, symbols, simple numbers or Vietnamese content so target is the feedback process.
- Avoid culturally opaque trivia and color-only distinctions that reduce accessibility.
- Keep response choices mutually exclusive and visually separable.
- Use non-shaming language and frame retry as normal learning evidence.
- Do not infer laziness, ability or misconception from navigation/access mistakes.

## 28. Exercise Architecture

Final lesson should contain this progression:

1. **Immediate retrieval:** attempt-before-feedback; conditional transcript-order check.
2. **Role spotting:** locate response, answer, clue/reason and retry in one block.
3. **Guided read-feedback cycle:** prompts provided step by step.
4. **Answer versus explanation contrast:** decide what information each statement supplies.
5. **Clue/reason selection:** choose the explanation tied to visible evidence.
6. **Error correction:** repair an incomplete feedback block.
7. **Guided changed-layout retry:** same rule, new arrangement.
8. **Faded `match` cycle:** fewer labels and no answer-visible-before-attempt.
9. **Independent short-response cycle:** complete without hints during attempt.
10. **Mixed review:** Day-001 attempt rule and Day-002 conditional transcript rule.
11. **Conceptual/self-explanation questions.**
12. **Quiz and mastery check:** two sample cycles, at least one with reduced support.

## 29. Exercise Depth and Volume

Use enough items to expose the difference between copying and understanding:

- 1–2 opening retrieval prompts;
- 2 role-location tasks;
- 2 answer-versus-explanation contrasts;
- 2 clue/reason tasks;
- 1 incomplete-feedback correction;
- at least 2 changed-layout retries;
- 2 mastery sample activities;
- 3–5 short quiz/concept questions.

These are design ranges, not arbitrary caps. Add another equivalent item only if needed for clear remediation, not to create volume.

## 30. Guided Practice and Hint Fading

### High support

- Vietnamese field labels visible;
- arrow showing feedback order;
- clue visually highlighted after response;
- sentence frames for why and retry.

### Medium support

- field labels remain, but learner locates clue without highlight;
- reason choices are provided;
- answer order changes.

### Low support

- learner completes attempt before reveal;
- identifies answer/clue/reason without step-by-step hint;
- states reason in own Vietnamese words;
- retries equivalent item with changed layout.

No hint may reveal the answer during an independent attempt.

## 31. Independent Practice Requirement

Independent practice must include two neutral activities using different response formats, preferably one `match` and one short response or choose. For each:

1. prompt and response area appear before feedback;
2. learner records a response;
3. feedback reveals answer and reason;
4. learner names the clue/reason;
5. learner retries an equivalent changed-layout item.

At least one cycle must not provide a sentence frame until after the first independent explanation attempt.

## 32. Mixed Practice

Mixed practice may combine:

- find response area (Day-001);
- attempt before feedback (Day-001);
- use `choose` (Day-001);
- if audio exists, response before transcript and replay after comparison (Day-002);
- new feedback anatomy and reason-based retry (Day-003).

Do not mix grammar, pronunciation or TOEIC content.

## 33. Error-Correction Practice

Present at least three fictional scenarios:

1. learner copies “B” but cannot say why;
2. learner reads reason but retries by choosing the old position after order changes;
3. feedback says only “incorrect” and gives no clue/reason.

Learner identifies what is missing and repairs the process. Clearly label all scenarios as mock examples; none may become learner-state evidence.

## 34. Production and Translation

Production is minimal and process-focused:

- complete “Đáp án là __; dấu hiệu là __; vì vậy __.”;
- restate one clue/reason in own Vietnamese words;
- translate only the functional instruction `try again` as “làm lại”.

Do not require English sentence production.

## 35. Real-World Application

Use one non-language learning analogy or micro-task, such as following a simple icon rule. Learner experiences that knowing the result differs from knowing the rule. Keep it under one small block so analogy does not replace course practice.

## 36. Mini-Conversation Policy

Not applicable to this lesson. A dialogue would add language load without improving the feedback-process target. If the generator uses a teacher–learner exchange for modeling, it must be Vietnamese-first, no more than a few turns, and function only as demonstration.

## 37. Diagnostic Section

Final lesson should distinguish these diagnostic dimensions:

| Observation | Likely layer | Immediate action |
|---|---|---|
| Feedback opened before response | Process/navigation | Re-run attempt gate |
| Cannot identify correct-answer field | Feedback navigation | Re-label regions and point once |
| Finds answer but not clue/reason | Feedback comprehension | Contrast what versus why |
| Explains reason but fails changed-layout retry | Transfer/application | One more guided equivalent item |
| Copies old option position | Memory strategy | Rearrange and require clue statement first |
| Audio does not play | Technical access | Use text fallback; no language judgment |

Do not convert one observation into a recurring misconception or mastery claim.

## 38. Retrieval Practice

Opening retrieval must ask learner to apply, not merely recognize:

- “Bạn làm gì trước khi mở feedback/answer?”
- one changed-layout navigation prompt locating response and feedback regions.
- conditional only: “Nếu có audio, transcript mở trước hay sau response?”

The Day-002 question must not appear as an audio requirement when no validated asset exists.

## 39. Spaced Repetition Plan

| Future point | Retrieval target |
|---|---|
| Day-004 | Use today’s cycle before recording an actual mistake and next review |
| Around Day-005 | Retrieve clue/reason before another changed-layout retry |
| Day-006 module checkpoint | Complete combined lesson/audio/feedback routine |
| Day-007 onward | Preserve response-before-feedback and first-listen-before-transcript |
| About one week equivalent | Delayed reason-based retry without seeing old answer position |

The State Updater may schedule these only from accepted course content; learner performance remains unknown until evidence exists.

## 40. Concept Extension and What-If Cases

Include short reasoning prompts:

- What if my first answer is already correct—do I still need explanation?
- What if feedback says only “incorrect”?
- What if I remember the old option position but not the reason?
- What if two explanations both sound possible?
- What if audio fails before I respond?

Expected principles: correct response can still benefit from reason checking; incomplete feedback should not support confident retry; position memory is insufficient; ambiguity must be fixed by the material; technical failure is not language evidence.

## 41. Conceptual Questions

Require brief Vietnamese answers to questions such as:

1. Why can copying a correct answer hide a misunderstanding?
2. What does the explanation add beyond the answer?
3. Why should a retry change the layout?
4. When can one mistake become evidence of a misconception?
5. Why must mock errors stay out of learner error state?

Accept semantically equivalent responses; do not demand exact wording.

## 42. Self-Explanation

At the end of each mastery cycle, learner completes:

> “Lần đầu tôi dựa vào ____. Feedback cho tôi thấy dấu hiệu/lý do ____. Khi làm lại, tôi ____.”

If the first answer was correct:

> “Câu trả lời của tôi đúng. Tôi biết không chỉ vì answer nói vậy, mà vì dấu hiệu ____ dẫn tới ____.”

Vietnamese free wording is valid.

## 43. Quiz Plan

Quiz should test concepts, not English knowledge:

- order the feedback cycle;
- identify which text is answer versus explanation;
- choose the best retry behavior;
- identify a missing clue/reason in poor feedback;
- distinguish one mistake from evidence of recurring misconception.

Use 3–5 concise items with one defensible answer each. Keep answers and explanations separated from questions.

## 44. Answer Explanation Requirements

Every closed question and mock activity must provide:

- correct answer;
- relevant context;
- exact clue or observable rule;
- why it leads to the answer;
- why wrong choices fail when useful;
- correction or contrast;
- a retry instruction when the skill requires transfer.

“Correct: B” or a copied word is never sufficient feedback.

## 45. Why-Wrong and Ambiguity Prevention

- All options must be mutually exclusive in the stated context.
- A question cannot have two plausible correct answers due to wording.
- Visual/symbol rules must be explicitly observable.
- Avoid color-only differences and tiny typography.
- Open-ended answers must list acceptable semantic equivalents.
- Wrong-option explanations should target the confusion, not shame learner.
- Retry items must preserve the underlying rule while changing surface order.
- Feedback/answer must not be visible before required attempt.

## 46. Optional Challenge

Learner receives a fictional incomplete feedback block and rewrites it to add:

1. a clear answer;
2. one exact clue;
3. one reason;
4. a changed-layout retry.

This is optional and cannot be required for core mastery.

## 47. Mastery Check

Mastery check must contain two sample activities requiring no English knowledge.

### Sample 1 — Supported but complete

- use `choose` or `match`;
- attempt before feedback;
- identify answer, clue and reason;
- complete changed-layout retry;
- explain difference between attempt and retry.

### Sample 2 — Reduced support

- use a different response format;
- no hint during attempt;
- learner locates feedback roles independently;
- retry changes position/layout;
- learner restates reason in own Vietnamese words.

Evidence dimensions must remain separate:

- role recognition;
- explanation comprehension;
- reason-based transfer;
- process/navigation;
- conditional audio access.

Passing is not assumed. The final lesson must provide a place to record evidence without writing state itself.

## 48. Remediation Plan

| Evidence gap | Remediation |
|---|---|
| Opens feedback early | Repeat a one-item closed-reveal routine |
| Confuses answer and explanation | Use “cái gì?” versus “vì sao?” sorting |
| Cannot find clue | Highlight one clue, then fade highlight on next item |
| Cannot connect clue to answer | Provide a Vietnamese because-frame, then retry |
| Copies answer position | Rearrange options and ask for clue before selection |
| Cannot restate reason | Offer two Vietnamese paraphrases, then remove choices |
| Audio unavailable | Switch to text-equivalent route; record access issue only |
| Two retries fail for different one-off reasons | Keep observations separate; do not claim misconception |

After remediation, use a fresh equivalent item, not the exact same position pattern.

## 49. Learner Checklist

Final lesson ends with a checklist similar to:

- [ ] Tôi trả lời trước khi mở feedback.
- [ ] Tôi tìm được đáp án đúng.
- [ ] Tôi tìm được dấu hiệu và lý do.
- [ ] Tôi có thể nói answer khác explanation thế nào.
- [ ] Tôi làm lại bằng lý do, không chỉ nhớ vị trí.
- [ ] Tôi hiểu `choose`, `match`, `correct`, `incorrect`, `try again` với hỗ trợ tiếng Việt.
- [ ] Nếu có audio, tôi chỉ mở transcript sau response.

Checkboxes are self-observation, not automatic mastery evidence.

## 50. One-Minute Summary

Required summary content:

> “Tôi tự trả lời trước. Sau đó tôi dùng feedback để tìm đáp án, dấu hiệu và lý do. Tôi nói điều cần đổi rồi làm lại với bố cục khác. Nếu tôi chỉ chép đáp án, tôi chưa kiểm tra được mình đã hiểu.”

Keep the summary Vietnamese-first and short enough to retrieve later.

## 51. Flashcard Plan

Create a small functional set, not a large deck:

1. `choose` → chọn;
2. `match` → nối/ghép tương ứng;
3. `correct` → đúng;
4. `incorrect` → chưa đúng;
5. `try again` → làm lại;
6. correct answer versus explanation;
7. feedback cycle order.

Cards 6–7 should be concept/retrieval cards in Vietnamese, not translation cards. No mastery claim from card exposure.

## 52. Homework Plan

Optional, low-load homework:

- revisit one completed mock block without seeing the old answer first;
- state clue/reason;
- solve a newly arranged equivalent item;
- write one Vietnamese sentence about what changed.

Do not ask learner to create or edit mistake log yet. That belongs to Day-004.

## 53. Time and Session Splitting

No hard time cap. Recommended natural split if learner needs a pause:

- Session A: retrieval, four roles, answer-versus-explanation, first guided cycle.
- Session B: mistake-versus-misconception awareness, faded cycles, quiz and mastery check.

Each resumed session starts with a 30–60 second retrieval of the central flow. Do not shorten explanation or remove retry solely to meet a time target.

## 54. Difficulty Profile

| Dimension | Planned Level |
|---|---|
| Conceptual novelty | Low–moderate: four connected roles |
| English load | Very low |
| Reading load | Very low, Vietnamese-first blocks |
| Memory load | Low; visible flow during guided practice |
| Transfer demand | Moderate; changed-layout retry is essential |
| Listening load | Optional/minimal |
| TOEIC pressure | None |

Difficulty should come from understanding the learning process, not decoding English.

## 55. Beginner Safety

- Use one new distinction at a time.
- Keep feedback blocks visually stable during teaching, then change layout only for transfer.
- Never hide a language prerequisite inside a symbol task.
- Avoid long English labels and unexplained abbreviations.
- Keep answer reveal physically separated from response area.
- Permit pause and Vietnamese explanation.
- Technical failure routes must preserve dignity and valid progress.
- `incorrect` labels a response, not a person.

## 56. Duplication Check

Day-001 content should be retrieved, not retaught: lesson regions, Learn Mode and attempt-before-feedback.

Day-002 content should be retrieved conditionally, not retaught: first listen, transcript timing and replay sequence.

New Day-003 value is the anatomy and use of detailed feedback plus changed-layout retry. The lesson must not duplicate Day-004 mistake log/review queue or future error classification.

## 57. Content Accuracy Requirements

- Answer and explanation must remain distinct fields.
- Clue must be present in the task/context, not invented after the fact.
- Reason must logically connect clue to answer.
- Retry must preserve the rule while changing irrelevant surface details.
- One mistake cannot establish a recurring misconception.
- Mock responses cannot be treated as learner evidence.
- Audio access failure cannot be scored as listening weakness.
- All multiple-choice questions need one defensible answer.

## 58. Content Boundaries

### Must cover

- four feedback roles;
- complete feedback cycle;
- choose, match and short-response formats;
- answer versus explanation;
- mistake versus misconception at introductory level;
- two reason-based changed-layout retries;
- Day-001 attempt-before-feedback retrieval;
- conditional Day-002 transcript-order retrieval;
- detailed answer explanations.

### May cover

- confidence note;
- restating clue in learner’s own Vietnamese words;
- one optional ultra-short audio activity with stable access;
- one non-language analogy.

### Must not cover

- grammar, pronunciation, IPA or alphabet teaching;
- full error taxonomy or mistake-log workflow;
- real TOEIC questions or scoring;
- timed/graded test;
- English reading ability assessment;
- persistence-layer/state editing;
- invented learner errors, mastery or completion;
- unvalidated audio filenames/assets.

## 59. Next-Lesson Connection

Day-004 will teach the introductory mistake log and review queue. Day-003 must leave learner able to produce the inputs Day-004 needs:

- original response;
- correct answer;
- clue/reason;
- what changed on retry.

Do not yet teach log fields, review dates, scheduling mechanics or recurring error categories. Preview with one sentence only: actual mistakes from future work can be saved for review after today’s feedback cycle is understood.

## 60. Instructions to Lesson Generator

Generate `lessons/phase-00/Day-003.md` only after this blueprint is accepted.

The lesson must:

1. be Vietnamese-led and zero-beginner safe;
2. state the central question and core flow early;
3. keep all feedback hidden until an attempt where the format permits;
4. use neutral mock content requiring no English knowledge;
5. teach all four roles explicitly;
6. include choose, match and short-response operation;
7. include at least two changed-layout retries;
8. fade hints before independent practice;
9. provide detailed answers separated from questions;
10. include conceptual questions, self-explanation, quiz, mastery and remediation;
11. make audio optional with a complete text fallback;
12. label all fictional errors as mock and never infer learner state;
13. preview Day-004 without teaching it;
14. avoid placeholders and ambiguous questions.

The generator must not redesign the curriculum, update state, mark completion, claim mastery or create any other file.

## 61. Expected Evidence for State Updater

If and only if a final Day-003 lesson is later generated, reviewed, accepted and learner evidence is available, the State Updater may look for:

### Course-content evidence

- feedback cycle introduced;
- target labels `choose`, `match`, `correct`, `incorrect`, `try again` taught/reviewed;
- answer-versus-explanation and mistake-versus-misconception distinctions covered;
- Day-001 attempt rule retrieved;
- conditional Day-002 transcript order retrieved;
- Day-004 preparation established.

### Learner evidence — do not invent

- whether learner attempted before feedback;
- whether learner located answer, clue and reason;
- whether retry transferred under changed layout;
- whether English labels were recognized with support;
- whether any audio access issue occurred;
- whether an error recurred enough to justify a pattern.

Mock examples, model answers and planned exercises are not learner errors or mastery evidence. If no learner performance is recorded, state must remain `Not evaluated`.

## 62. Planner Final Audit

### Curriculum alignment

- [x] Plans exactly authorized `Day-003`.
- [x] Matches Foundation / Learning strategy goal.
- [x] Prepares directly for Day-004 without teaching it early.

### Prerequisites and load

- [x] Retrieves Day-001 attempt rule.
- [x] Retrieves Day-002 transcript order only when audio applies.
- [x] Introduces no hidden grammar, pronunciation or English-reading prerequisite.
- [x] Limits new targets to a manageable feedback cycle and five functional labels.

### Explanation and exercises

- [x] Separates answer, clue/reason and retry.
- [x] Includes required comparisons and edge cases.
- [x] Progresses from guided to independent practice.
- [x] Requires two changed-layout retries and detailed explanations.
- [x] Prohibits ambiguous options and premature answer reveal.

### Skills and beginner safety

- [x] Vocabulary is functional and glossed.
- [x] Pronunciation, grammar and TOEIC are correctly excluded.
- [x] Listening is optional and access-safe.
- [x] Reading load is Vietnamese-first and minimal.

### Mastery, remediation and state safety

- [x] Success criteria are observable but not pre-awarded.
- [x] Remediation uses fresh equivalent items.
- [x] Mock errors cannot enter learner state.
- [x] No learner performance, mastery or completion is invented.

### Generator readiness

- [x] Another AI can generate the complete Day-003 lesson without redesigning objectives, sequence, activities, feedback, mastery or remediation.

Blueprint status: **READY FOR LESSON GENERATION AFTER REVIEW/ACCEPTANCE**.
