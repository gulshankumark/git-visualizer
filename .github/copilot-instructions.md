# Copilot Instructions — git-visualizer

A project for learning Git through command visualizations. The application code does not yet exist; the repository is currently in the planning/design phase using the BMAD method.

## Repository Layout

| Path | Purpose |
|---|---|
| `_bmad/` | BMAD installation config — **do not edit manually** |
| `_bmad-output/` | Generated BMAD artifacts (gitignored); subdirs `planning-artifacts/` and `implementation-artifacts/` |
| `.github/skills/` | Copilot BMAD skills (one directory per skill) |
| `docs/` | Project knowledge — BMAD reads this for context; put new reference docs here |

## BMAD Workflow

This repository uses BMAD v6.3.0 with GitHub Copilot integration. **Always start a BMAD task in a fresh context window** and use the `bmad-help` skill to orient yourself before picking a skill.

### Phases and key skills

| Phase | Required skills (in order) |
|---|---|
| 1 — Analysis | `bmad-domain-research`, `bmad-technical-research`, `bmad-prfaq` / `bmad-product-brief` |
| 2 — Planning | `bmad-create-prd` → `bmad-validate-prd` → `bmad-create-ux-design` |
| 3 — Solutioning | `bmad-create-architecture` → `bmad-create-epics-and-stories` → `bmad-check-implementation-readiness` |
| 4 — Implementation | `bmad-sprint-planning` → `bmad-create-story` → `bmad-dev-story` → `bmad-code-review` |

**Anytime skills** (no phase dependency): `bmad-quick-dev`, `bmad-code-review`, `bmad-checkpoint-preview`, `bmad-correct-course`, `bmad-party-mode`, `bmad-help`.

### Artifact locations

- Planning artifacts (PRD, architecture, epics) → `_bmad-output/planning-artifacts/`
- Implementation artifacts (stories, sprint plans, test suites) → `_bmad-output/implementation-artifacts/`
- Persistent project knowledge → `docs/`

### Skill menu codes

Each skill has a short code for quick invocation (e.g., `BH` = bmad-help, `CP` = Create PRD, `CA` = Create Architecture, `DS` = Dev Story, `QQ` = Quick Dev). See `_bmad/_config/bmad-help.csv` for the full catalog.

## Key Conventions

- **Git commits** must include the trailer: `Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>`
- **`_bmad-output/` is gitignored** — generated artifacts are not committed; only `.gitkeep` placeholders are tracked.
- **`docs/`** is the BMAD `project_knowledge` path. Add reference material here so BMAD skills can read it for context.
- **`.worktrees/`** is gitignored — use git worktrees for feature isolation; they live outside the tracked tree.
- The `.gitignore` uses the Visual Studio template, signalling a planned .NET/C# application with possible Node.js tooling.
