# Contributing to git-visualizer

Thank you for your interest in contributing! This guide covers everything you need to get started.

---

## Prerequisites

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 10.0 or later | https://dot.net |
| Node.js | 24 or later | https://nodejs.org |
| Git | Any recent version | https://git-scm.com |

---

## Local Development Setup

```bash
# 1. Fork and clone
git clone https://github.com/<your-username>/git-visualizer.git
cd git-visualizer

# 2. Install JavaScript dependencies
cd src/GitVisualizer/wwwroot
npm install
cd ../../..

# 3. Start the dev server with hot reload
dotnet watch run --project src/GitVisualizer
# Opens at http://localhost:5000/git-visualizer/
# Note: if the browser auto-opens to http://localhost:5000, navigate manually to /git-visualizer/
```

---

## Common Commands

| Task | Command |
|---|---|
| Build | `dotnet build git-visualizer.sln` |
| Run all tests | `dotnet test git-visualizer.sln` |
| Dev server (hot reload) | `dotnet watch run --project src/GitVisualizer` |
| Publish (production) | `dotnet publish src/GitVisualizer -c Release -o publish/` |

All commands should be run from the repository root unless noted otherwise.

---

## Code Style

### C# Conventions

- **Private fields:** `_camelCase` prefix (e.g., `_gitService`)
- **Async methods:** `Async` suffix (e.g., `InitializeAsync`)
- **Interfaces:** `I` prefix (e.g., `IGitSimulatorService`)
- **Blazor components:** `PascalCase.razor` (e.g., `TerminalPanel.razor`)
- **JS interop files:** `kebab-case-interop.js` (e.g., `git-interop.js`)
- **Comments:** Only where clarification is needed — avoid redundant comments

### Architecture Rules

- Components never own git state or call JS interop directly
- All state lives in `GitSimulatorService` (singleton)
- All JS interop goes through wrapper classes in `Interop/` only
- Service methods return `CommandResult` — never throw for expected errors

---

## Pull Request Process

1. **Fork** the repository and create a branch from `main`
2. **Make your changes** — follow the code style above
3. **Run tests:** `dotnet test git-visualizer.sln` — all tests must pass
4. **Open a PR** targeting `main`
5. Fill in the PR template (description + checklist)
6. CI must pass before merge; one reviewer approval is required

### Branch naming suggestions

- `feat/short-description` for new features
- `fix/short-description` for bug fixes
- `docs/short-description` for documentation only changes

---

## Reporting Issues

- **Bugs:** [Open a bug report](https://github.com/gulshankumark/git-visualizer/issues/new?template=bug_report.md)
- **Feature requests:** [Open a feature request](https://github.com/gulshankumark/git-visualizer/issues/new?template=feature_request.md)
- **Security vulnerabilities:** See [SECURITY.md](SECURITY.md) — do **not** open a public issue

---

## Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you agree to uphold it.
