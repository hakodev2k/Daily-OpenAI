# Daily TOEIC Journey — Repository and Pipeline Architecture

## 1. Design goals

The repository must remain understandable and reliable across hundreds of lessons and many AI-assisted generation runs. It must:

- preserve source-of-truth information outside conversation history;
- separate stable policy, planned curriculum, final learning artifacts, review evidence, and mutable learner state;
- make prerequisites and review obligations machine-readable enough for future automation while remaining human-auditable;
- prevent the same fact from being maintained in several places;
- support separately generated audio and verified transcripts;
- preserve version history and traceability from architecture to lesson to state update;
- allow a fresh planner or generator to continue safely using repository files alone.

The architecture favors a small number of authoritative files plus predictable per-phase/per-lesson artifacts. New files should be created only when they have a distinct owner, lifecycle, or access pattern.

## 2. Recommended structure

```text
Daily TOEIC Journey/
├── README.md
├── COURSE_SPEC.md
├── ROADMAP.md
├── LEARNING_SYSTEM.md
├── KNOWLEDGE_DEPENDENCIES.md
├── REPOSITORY_ARCHITECTURE.md
├── COURSE_STATE.md
├── CHANGELOG.md
│
├── specs/
│   ├── LESSON_STANDARD.md
│   ├── EXERCISE_STANDARD.md
│   ├── ASSESSMENT_STANDARD.md
│   ├── VOCABULARY_STANDARD.md
│   ├── LISTENING_STANDARD.md
│   ├── READING_STANDARD.md
│   ├── PRONUNCIATION_STANDARD.md
│   ├── TOEIC_STANDARD.md
│   ├── CONTENT_SCHEMA.md
│   └── STATE_SCHEMA.md
│
├── curriculum/
│   ├── CURRICULUM_INDEX.md
│   ├── concept-catalog.md
│   ├── vocabulary-domains.md
│   ├── phase-00/
│   │   ├── PHASE_PLAN.md
│   │   └── units/
│   │       └── unit-00-01.md
│   └── phase-01/ ... phase-07/
│
├── lessons/
│   ├── phase-00/
│   │   └── lesson-0001/
│   │       ├── LESSON.md
│   │       ├── ANSWERS.md
│   │       └── manifest.yaml
│   └── phase-01/ ... phase-07/
│
├── assessments/
│   ├── item-bank/
│   ├── weekly/
│   ├── cumulative/
│   ├── phase/
│   └── toeic-mode/
│
├── reviews/
│   ├── lesson-reviews/
│   ├── assessment-reviews/
│   ├── audio-reviews/
│   └── release-log.md
│
├── state/
│   ├── REVIEW_QUEUE.md
│   ├── VOCABULARY_STATE.md
│   ├── GRAMMAR_STATE.md
│   ├── SKILL_STATE.md
│   ├── ERROR_PATTERNS.md
│   ├── ASSESSMENT_HISTORY.md
│   └── archive/
│
├── audio/
│   ├── AUDIO_INDEX.md
│   ├── phase-00/
│   │   └── lesson-0001/
│   │       ├── l0001-a01.mp3
│   │       ├── l0001-a01.transcript.md
│   │       └── l0001-a01.meta.yaml
│   └── phase-01/ ... phase-07/
│
├── templates/
│   ├── lesson-plan-template.md
│   ├── lesson-template.md
│   ├── review-template.md
│   ├── assessment-template.md
│   └── audio-metadata-template.yaml
│
└── tools/
    ├── README.md
    ├── validate-structure.*
    ├── validate-links.*
    ├── validate-state.*
    └── build-audio-index.*
```

Directories that are not yet operational need not be populated immediately. The naming conventions and ownership rules should remain stable.

## 3. Source-of-truth hierarchy

When information conflicts, use this authority order within each concern:

1. `COURSE_SPEC.md` — constitutional constraints and course identity.
2. `ROADMAP.md` — phase outcomes and transition profile.
3. `LEARNING_SYSTEM.md` — operational learning, review, mastery, and remediation rules.
4. `KNOWLEDGE_DEPENDENCIES.md` — high-level prerequisite relationships.
5. `specs/` — detailed artifact and data rules that implement the four files above.
6. `curriculum/` — planned instantiation of concepts into phases and units.
7. final `lessons/`, `assessments/`, and `audio/` — released learning artifacts.
8. `state/` and `COURSE_STATE.md` — mutable evidence and next-action status.
9. `reviews/` — quality evidence and release decisions, not educational policy.

This hierarchy is concern-specific: learner performance in `state/` can cause a planned lesson to be delayed, but it cannot silently redefine the course's constitutional rules.

## 4. Root files

| Path | Purpose and required content | Owner / update stage | Stability | Must not duplicate |
|---|---|---|---|---|
| `README.md` | Human entry point: course purpose, navigation, study/start instructions, current release, and links to authoritative files | Course maintainer | Stable with occasional updates | Full constitutional policy, full roadmap, learner state |
| `COURSE_SPEC.md` | Vision, learner, outcomes, CEFR/TOEIC distinction, tracks, language policy, permanent constraints and quality rules | Architecture governance | Highly stable | Phase detail, daily plans, current learner data |
| `ROADMAP.md` | Phase definitions, outcomes, approximate orientations, part progression, milestones, transition requirements | Curriculum architecture | Stable; reviewed on major curriculum change | Daily lesson lists, individual scores, detailed algorithms |
| `LEARNING_SYSTEM.md` | Mastery loop, modes, exercises, spacing, interleaving, errors, assessment, remediation, progression | Learning-system architect | Stable | Concrete learner queue, full dependency catalog, daily content |
| `KNOWLEDGE_DEPENDENCIES.md` | Major concept graphs and ordering decisions | Curriculum/CEFR architect | Stable with controlled revisions | Learner status, daily schedule, repeated lesson explanations |
| `REPOSITORY_ARCHITECTURE.md` | Structure, ownership, lifecycle, pipeline inputs/outputs, naming, validation | Systems architect | Stable | Educational content or current progress |
| `COURSE_STATE.md` | Small dashboard: active learner/course instance, active phase/unit/lesson, blockers, last finalized artifact, next authorized action, pointers to detailed state | State updater after finalization | Highly mutable | Full vocabulary/grammar/error lists, lesson content |
| `CHANGELOG.md` | Deliberate changes to architecture, schemas, phase plans, and released content, with migration notes | Maintainer/release stage | Append-oriented | Full review reports or git history narrative |

`COURSE_STATE.md` is an index and checkpoint, not an all-purpose database. It should stay short enough for every pipeline stage to read.

## 5. `specs/` — implementation standards

These files are created in a later specification run. They must elaborate the constitutional documents without contradicting them.

| File | Owns | Primary updater | Must not contain |
|---|---|---|---|
| `LESSON_STANDARD.md` | Lesson families; universal/conditional sections; IDs; metadata; accessibility; finalization rules | Lesson architecture | Actual lesson sequence or learner results |
| `EXERCISE_STANDARD.md` | Item types, construction, distractors, solution depth, difficulty tags, validation | Exercise design | Large live item bank or scores |
| `ASSESSMENT_STANDARD.md` | Assessment layers, construct maps, conditions, scoring/interpretation, security, parallel forms | Assessment design | Learner history or exact phase plan |
| `VOCABULARY_STANDARD.md` | Selection, lexical-entry depth, domain coverage, sense/collocation/paraphrase fields, status rules | Lexical curriculum | Daily word allocation and learner-specific state |
| `LISTENING_STANDARD.md` | Listening lesson protocol, scripts, transcript control, speakers/accents/rate, audio QA | Listening/audio design | Audio binaries or lesson-specific transcripts |
| `READING_STANDARD.md` | Text genres, readability/difficulty control, evidence, question types, multi-document design | Reading design | Released passages or scores |
| `PRONUNCIATION_STANDARD.md` | Perception/production sequence, notation, Vietnamese-relevant checks, connected speech | Pronunciation design | Claims about an individual learner without evidence |
| `TOEIC_STANDARD.md` | Parts 1–7 constraints, realism, mode conditions, pacing, simulation rules | TOEIC assessment design | Copied proprietary test items or CEFR equivalence claims |
| `CONTENT_SCHEMA.md` | Required metadata/IDs for concepts, units, lessons, exercises, assessments, audio references | Systems/content architecture | Pedagogical policy already owned elsewhere |
| `STATE_SCHEMA.md` | Allowed statuses, evidence records, queue fields, error taxonomy links, archive/migration rules | State systems | Actual learner values |

Detailed standards cite sections of root architecture instead of copying them. If a standard needs a short restatement, it must label the root as authoritative.

## 6. `curriculum/` — planned learning sequence

### `CURRICULUM_INDEX.md`

Purpose: global map of phases, units, their status, concept coverage, and links. It supports navigation and coverage validation.

- **Owner:** curriculum planning stage.
- **Lifecycle:** mutable during planning; versioned/frozen by released block.
- **Contains:** phase/unit IDs, titles, prerequisite pointers, planned modes, status (`draft`, `reviewed`, `released`, `superseded`), and links.
- **Excludes:** full lesson prose, learner scores, and duplicated roadmap descriptions.

### `concept-catalog.md`

Purpose: registry of stable concept IDs used across plans, lessons, exercises, and state.

- **Owner:** curriculum architecture; updated under change control.
- **Contains:** ID, canonical name, track, high-level dependency node, brief scope, intended depth by phase, aliases, and deprecation mapping.
- **Excludes:** long teaching explanations, learner status, and every example.

### `vocabulary-domains.md`

Purpose: coverage map for lexical frequency bands, semantic domains, TOEIC domains, and cross-domain recycling.

- **Owner:** vocabulary curriculum planning.
- **Contains:** domain IDs, coverage aims, phase windows, prerequisite/core vocabulary relationships, and links to planned units.
- **Excludes:** mutable learner word status and arbitrary daily lists.

### `phase-NN/PHASE_PLAN.md`

Purpose: instantiate the roadmap phase into unit-level curriculum.

- **Owner:** curriculum planner; approved by curriculum review.
- **Contains:** entry/exit evidence, unit sequence, concepts and depth, vocabulary/domain coverage, skill integrations, review/assessment placements, dependencies, load rationale, and planned TOEIC exposure.
- **Excludes:** complete lesson text, current learner performance, and copied root policies.

### `phase-NN/units/unit-NN-NN.md`

Purpose: define a coherent learning block from which individual lesson plans are generated.

- **Owner:** curriculum planning.
- **Contains:** unit outcomes, lesson candidates without forcing calendar dates, prerequisite graph slice, new/review/extension knowledge, assessment blueprint, audio/text needs, and remediation branches.
- **Excludes:** fabricated mastery claims and final answer keys unless the unit itself is an assessment design artifact.

Curriculum planning is normative—what should be taught when ready. Learner state is descriptive—what this learner has demonstrated. Do not merge them.

## 7. `lessons/` — final instructional artifacts

Each lesson uses a directory so lesson text, solutions, metadata, and audio references remain atomic.

### Naming

- Directory: `lessons/phase-NN/lesson-NNNN/`
- Stable lesson ID: `LNNNN`
- Main files: `LESSON.md`, `ANSWERS.md`, `manifest.yaml`
- Revision uses metadata/version history, not a new unrelated ID unless learning scope changes materially.

### `LESSON.md`

- **Purpose:** learner-facing final lesson, excluding answers that would spoil an independent attempt when separation is needed.
- **Owner:** lesson generation, then lesson fixing; immutable per released version.
- **Contains:** required lesson-standard components, activity IDs, audio references, transcript reveal instructions/links, and state-report prompts.
- **Excludes:** internal chain-of-thought, review discussion, unapproved draft content, and learner mastery assertions.

### `ANSWERS.md`

- **Purpose:** learner-facing answer key and detailed instructional solutions.
- **Owner:** lesson generation/fixing; independently reviewed.
- **Contains:** complete answers, rule/evidence reasoning, why-wrong explanations, trap and next-time clues, transcript analysis where appropriate.
- **Excludes:** answers to unrelated lessons and unsupported diagnoses about the learner.

### `manifest.yaml`

- **Purpose:** compact machine-readable lesson contract.
- **Owner:** lesson planner/generator; validated at finalization.
- **Contains:** lesson/version IDs, phase/unit, mode, objectives, concept IDs with roles, prerequisites, vocabulary IDs or sets, activity/item/audio IDs, review targets, expected outputs, status, and hashes or revision references if implemented.
- **Excludes:** full lesson prose and duplicated answer explanations.

Drafts should live in a temporary workflow location or version-control branch, not beside released files with ambiguous names such as `final-final2.md`.

## 8. `assessments/` — assessment artifacts

- `item-bank/`: validated original items and metadata, with exposure/reuse controls if required.
- `weekly/`: learning-cycle reviews; “weekly” is a familiar label, not a forced seven-day advancement rule.
- `cumulative/`: broader interleaved and durability checks.
- `phase/`: entry/exit and readiness instruments.
- `toeic-mode/`: timed part sets, sections, and mock forms.

Each assessment package should contain the instrument, answer/explanation file, manifest/blueprint, and release status. The assessment design/review stages own it. Learner responses and results belong in state/history, not inside the reusable master form. Proprietary TOEIC items must not be copied into the repository.

## 9. `reviews/` — independent quality evidence

### Directories

- `lesson-reviews/`: review reports keyed by lesson ID/version.
- `assessment-reviews/`: construct, key, ambiguity, difficulty, and condition reviews.
- `audio-reviews/`: transcript match, intelligibility, pace, silence, speaker, and file-integrity checks.
- `release-log.md`: concise record of approved/rejected versions and links to reports.

### Ownership and lifecycle

- **Owner:** independent review stage; fixing stage may append resolution references but must not rewrite the original findings.
- **Stability:** append-oriented audit evidence.
- **Contains:** artifact/version, checklist, findings by severity, required fixes, verification results, reviewer identity/type, date, and decision.
- **Excludes:** general policy, learner mastery state, or hidden answer changes not reflected in the released artifact.

A lesson is final only when blocking findings are fixed and the reviewed version matches the released version.

## 10. `state/` — operational learner memory

State files are mutable, evidence-based, and updated only after a final artifact is completed or a valid assessment event occurs. They use stable IDs and concise records.

### `REVIEW_QUEUE.md`

- **Purpose:** due and future retrieval/remediation work.
- **Owner:** state-update stage; consumed by curriculum/lesson planning.
- **Contains:** item/concept/skill ID, reason, due window, priority factors, last evidence, required review type, and status.
- **Excludes:** full teaching content and generic curriculum sequence.

### `VOCABULARY_STATE.md`

- **Purpose:** learner lexical evidence by sense and skill.
- **Contains:** lexical ID, intended depth, written/spoken recognition, contextual understanding, pronunciation/production where targeted, collocation/paraphrase evidence, last/next review, and source.
- **Excludes:** canonical domain policy and full dictionary entries already owned by vocabulary artifacts.

### `GRAMMAR_STATE.md`

- **Purpose:** learner evidence for foundation/grammar concepts.
- **Contains:** concept ID, status, evidence dimensions, confusions, last/next evidence, prerequisite impact, and sources.
- **Excludes:** the dependency graph and full grammar lessons.

### `SKILL_STATE.md`

- **Purpose:** listening, reading, pronunciation/perception, paraphrase, real-world, autonomy, and TOEIC subskill profiles.
- **Contains:** subskill ID, phase expectation, evidence by mode, accuracy/independence/fluency/transfer/durability, trends, and active constraints.
- **Excludes:** a single undifferentiated “English score.”

### `ERROR_PATTERNS.md`

- **Purpose:** important and recurring error diagnoses.
- **Contains:** pattern ID, taxonomy, examples by reference, recurrence, suspected cause/confidence, intervention, review link, and status.
- **Excludes:** every trivial one-time typo and entire copied questions.

### `ASSESSMENT_HISTORY.md`

- **Purpose:** comparable record of assessment results and conditions.
- **Contains:** assessment/form/version, date, mode, timing/audio/material conditions, skill results, confidence, interpretation, and linked error/state updates.
- **Excludes:** reusable master assessment content and unsupported score conversions.

### `archive/`

Stores superseded snapshots or migrated state according to `STATE_SCHEMA.md`. Archiving must preserve traceability and must not be used to hide inconvenient evidence.

## 11. `audio/` — separately generated listening assets

Audio is organized by the lesson that uses it, while `AUDIO_INDEX.md` provides global discovery and QA status.

### Asset naming

`lNNNN-aNN.ext`, for example `l0001-a01.mp3`. IDs referenced in lesson activities and manifests must match exactly.

### Per-asset files

- Audio file (`.mp3`, `.wav`, or approved format): final learner asset.
- `lNNNN-aNN.transcript.md`: canonical transcript with optional speaker labels and analysis annotations kept in clearly separated sections.
- `lNNNN-aNN.meta.yaml`: ID, lesson/activity, version, language/locale or intended accent, speaker roles, speech rate classification or measured rate where useful, duration, generation/source record, transcript version, mode/replay policy, and QA status.

### Ownership and validation

- **Script owner:** lesson/listening content stage.
- **Audio generation owner:** audio production stage.
- **QA owner:** independent audio review.
- **Stability:** immutable per released version; regenerate with a new version when script/audio changes.

Audio QA checks:

- transcript–audio agreement;
- no missing/extra words that alter an answer;
- pronunciation and intelligibility;
- intended speed, pauses, speaker identity, and accent characteristics;
- absence of clipping, artifacts, accidental answer cues, or excessive silence;
- all listening items remain valid against the final waveform;
- transcript reveal timing is correctly referenced from the lesson.

The learner-facing listening flow is: first listen without transcript, controlled repeats, comprehension response, transcript reveal, sound/meaning/paraphrase analysis, optional dictation/shadowing, and later transcript-free retrieval. Actual audio is not generated as part of architecture design.

## 12. `templates/` and `tools/`

Templates provide structure but never override standards. Each template cites its governing specification and includes a version. Optional sections must be marked so generators do not mechanically fill irrelevant headings.

Tools perform validation and indexing. They may check links, IDs, schema fields, missing audio, release status, prerequisite references, and state integrity. A validator reports errors; it must not silently rewrite educational content or learner state. `tools/README.md` documents supported commands, inputs, outputs, and non-destructive behavior.

## 13. Pipeline architecture

```text
Course Architecture
        ↓
Curriculum Planning
        ↓
Lesson Planning
        ↓
Lesson Generation
        ↓
Independent Lesson Review
        ↓
Lesson Fixing (when required)
        ↓
Final Lesson + Answers + Audio references
        ↓
Learner Completion / Assessment Evidence
        ↓
State Update
        ↓
Next-Lesson Selection
```

### 13.1 Course Architecture

**Reads:** educational requirements and controlled revisions.  
**Produces:** the five root architecture files and later detailed specifications.  
**Must ensure:** coherent authority, no duplicated ownership, and compatibility across phase, dependency, and learning rules.

### 13.2 Curriculum Planning

**Reads:** all root architecture, relevant `specs/`, concept catalog, vocabulary domains, released curriculum, and aggregate constraints.  
**Produces:** phase plans, unit plans, coverage maps, assessment placements, and prerequisite links.  
**Must not:** write learner-specific mastery or full daily lessons.

### 13.3 Lesson Planning

**Reads:** relevant phase/unit plan, current `COURSE_STATE.md`, detailed state slices, review queue, error patterns, lesson/exercise standards, and available audio/text dependencies.  
**Produces:** a lesson plan with objectives, modes, concept roles, load budget, activity progression, review insertions, required assets, and state evidence expected.  
**Must resolve:** whether the learner is ready, whether remediation is needed, and which due reviews fit.

### 13.4 Lesson Generation

**Reads:** approved lesson plan, governing standards, prerequisite lesson references, canonical concept/vocabulary records, and templates.  
**Produces:** draft `LESSON.md`, `ANSWERS.md`, manifest, original item content, transcript scripts, and asset requests.  
**Must not:** assume knowledge outside the plan or claim the learner has mastered presented content.

### 13.5 Independent Lesson Review

**Reads:** generated package plus the same plan and governing sources; reviews independently of the generator's claims.  
**Checks:** accuracy, natural English, Vietnamese clarity, prerequisites, load, answer defensibility, distractors, detailed solutions, mode separation, links, accessibility, TOEIC realism, and audio/script agreement when available.  
**Produces:** version-specific findings and a release decision.  
**Must not:** silently edit the lesson while pretending it was reviewed unchanged.

### 13.6 Lesson Fixing

**Reads:** draft, review findings, and governing sources.  
**Produces:** revised package and a resolution map linking every blocking finding to a change or reasoned disposition.  
**Then:** the changed version is revalidated; material changes receive independent re-review.

### 13.7 Final Lesson

**Release gate:** all required files present, links/IDs valid, blocking review issues closed, answer/audio consistency verified, manifest version matches content, and release recorded.  
**Produces:** immutable released version used by the learner.

### 13.8 State Update

**Reads:** released manifest, actual learner responses/results, mode/conditions, and review interpretation.  
**Produces:** concise evidence updates in state files, queue changes, error pattern changes, and `COURSE_STATE.md` checkpoint.  
**Must not:** infer performance for unattempted activities, equate exposure with mastery, or copy entire lessons into state.

### 13.9 Next Lesson

**Reads:** curriculum availability plus current state and dependencies.  
**Selects:** next planned lesson, targeted review, remediation, assessment, or parallel-track work.  
**Decision rule:** mastery and prerequisite evidence govern; calendar day and prewritten order are advisory only.

## 14. Artifact status and version lifecycle

Recommended statuses:

```text
planned → drafted → in_review → changes_required → re_review → approved → released
                                                                  ↓
                                                             superseded
```

- Only `released` artifacts drive official state updates.
- Released content is changed through a new version, never overwritten without trace.
- A superseded artifact remains linkable for historical evidence.
- State records cite the artifact version actually used.
- Architecture/schema changes document migration effects in `CHANGELOG.md`.

## 15. Identifier and link policy

Use stable, human-readable IDs:

- phase `P00`–`P07`;
- unit `U00-01`;
- lesson `L0001`;
- activity `L0001-A01`;
- item `L0001-A01-Q01` or an assessment-bank ID;
- audio `L0001-AUD01` (filename may use lowercase);
- concept IDs namespaced by track, such as `G-TENSE-PRESENT-SIMPLE` or a documented shorter scheme;
- lexical and error IDs according to schema.

File paths may change during reorganization; stable IDs should not. Internal Markdown links should be relative. Manifests store IDs plus paths. Every reference must resolve during validation.

## 16. Duplication rules

Use references instead of copying authoritative content:

- roadmap phases reference constitutional principles rather than restating all of them;
- lesson manifests reference concept IDs rather than redefining concepts;
- state files record evidence/status rather than copying explanations;
- review reports reference item IDs and short excerpts rather than duplicating full artifacts;
- `COURSE_STATE.md` points to detailed state rather than aggregating every record;
- audio transcripts are canonical beside audio; lesson files link or reveal them according to mode instead of maintaining divergent copies.

A deliberate short summary is allowed for usability if it identifies the authoritative source. If two files would require synchronized edits to remain correct, ownership is probably unclear and should be redesigned.

## 17. Validation and release checklist

### Structural

- required files and manifests exist;
- IDs are unique and references resolve;
- phase/unit/lesson relationships are consistent;
- referenced prerequisites exist;
- audio/transcript/metadata sets are complete;
- status and version fields agree.

### Pedagogical

- objectives align with curriculum and phase;
- prerequisites are taught or explicitly handled;
- core/review/extension roles are clear;
- new load and difficulty dimensions are controlled;
- exercises progress toward independent transfer;
- spaced review and state instructions exist;
- remediation is possible.

### Content

- English is natural and accurate;
- Vietnamese explanations/translations are clear and contextual;
- no unexplained required terminology appears;
- every scored answer is defensible;
- wrong-option explanations and evidence are sufficient;
- TOEIC material is original, realistic, and phase-appropriate.

### Audio

- waveform matches canonical transcript;
- speaker/rate/accent metadata and lesson use agree;
- questions remain valid against final audio;
- transcript is hidden/revealed according to mode;
- files pass technical and listening QA.

### State safety

- updates cite actual evidence and artifact version;
- no exposure is mislabeled as mastery;
- review due dates/reasons are recorded;
- blocking prerequisites and next action are explicit;
- prior history remains recoverable.

## 18. Continuity without AI memory

A new AI process must be able to continue the course by reading, in order:

1. `COURSE_SPEC.md`;
2. `ROADMAP.md` and the active phase plan;
3. `LEARNING_SYSTEM.md`;
4. relevant sections of `KNOWLEDGE_DEPENDENCIES.md` and `specs/`;
5. `COURSE_STATE.md`;
6. the relevant detailed state files and review queue;
7. the active unit and recent released lesson manifests;
8. open review findings, if producing or fixing an artifact.

Conversation history may provide convenience, but it is never the source of truth. Any decision required by the next run must be committed to an owned repository artifact.

## 19. Architecture-change procedure

For a material change:

1. identify the authoritative file and reason;
2. analyze dependent specs, curriculum, released artifacts, identifiers, and state schemas;
3. revise the smallest authoritative source;
4. add migration or compatibility notes to `CHANGELOG.md`;
5. update affected dependents by reference rather than broad duplication;
6. run structural and pedagogical validation;
7. record the effective version/date.

Routine learner progress updates do not modify architecture. A single difficult lesson does not justify changing the roadmap without broader evidence.
