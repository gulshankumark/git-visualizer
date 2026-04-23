# BMAD Installation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Install BMAD into this repository with GitHub Copilot integration, commit the generated BMAD assets, and document the setup in the README.

**Architecture:** Use the upstream BMAD CLI in non-interactive mode so installation is deterministic and repeatable. Install the core `bmm` module because this minimal repository does not yet justify extra BMAD modules, and place the BMAD-managed assets in `.\_bmad\`, `.\_bmad-output\`, and `.\.github\skills\` while `README.md` gets a short usage note that points contributors to the installed BMAD entrypoint.

**Tech Stack:** Node.js, npm/npx, BMAD CLI (`bmad-method`), GitHub Copilot repository skills, Git

---

### Task 1: Install BMAD assets

**Files:**
- Create: `.\_bmad\`
- Create: `.\_bmad-output\`
- Create: `.\.github\skills\`

- [ ] **Step 1: Run the BMAD installer with explicit repo settings**

```powershell
npx --yes bmad-method install --directory . --modules bmm --tools github-copilot --output-folder _bmad-output --yes
```

- [ ] **Step 2: Verify the installer created the BMAD directories**

Run: `Get-ChildItem -Name . ; Test-Path .github\skills`
Expected: the root listing includes `_bmad` and `_bmad-output`, and `Test-Path` returns `True` for `.github\skills`.

- [ ] **Step 3: Inspect the generated changes before editing docs**

Run: `git --no-pager status --short`
Expected: new tracked paths appear under `_bmad\` and `.github\skills\`, and `_bmad-output\` exists on disk even if it is empty.

- [ ] **Step 4: Commit the generated BMAD assets**

```powershell
git add -- _bmad _bmad-output .github\skills
git commit -m "chore: add BMAD scaffolding" -m "Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

### Task 2: Document the BMAD setup

**Files:**
- Modify: `.\README.md`

- [ ] **Step 1: Replace the README content with a BMAD-aware project summary**

```markdown
# git-visualizer

Learn Git from visualizations of Git commands.

## BMAD

This repository includes BMAD with GitHub Copilot integration.

- Core BMAD files live in `.\_bmad\`
- Generated BMAD artifacts live in `.\_bmad-output\`
- Copilot BMAD skills live in `.\.github\skills\`

Use the `bmad-help` BMAD entrypoint from a compatible GitHub Copilot environment to see the available workflows and next steps.
```

- [ ] **Step 2: Review the documentation diff**

Run: `git --no-pager diff -- README.md`
Expected: the diff only adds the BMAD section and preserves the existing project description.

- [ ] **Step 3: Commit the README update**

```powershell
git add -- README.md
git commit -m "docs: document BMAD setup" -m "Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

### Task 3: Validate the installed BMAD entrypoints

**Files:**
- Verify: `.\_bmad\`
- Verify: `.\_bmad-output\`
- Verify: `.\.github\skills\`
- Verify: `.\README.md`

- [ ] **Step 1: Confirm the GitHub Copilot BMAD help entrypoint was generated**

Run: `Get-ChildItem -Name .github\skills | Select-String "bmad-help"`
Expected: one match containing `bmad-help`.

- [ ] **Step 2: Confirm the repository now exposes all expected BMAD roots**

Run: `Get-ChildItem -Name _bmad ; Test-Path _bmad-output`
Expected: `_bmad` lists BMAD-managed content and `Test-Path` returns `True` for `_bmad-output`.

- [ ] **Step 3: Review the final working tree**

Run: `git --no-pager status --short`
Expected: no unexpected untracked files remain outside the intended BMAD paths and `README.md`.
