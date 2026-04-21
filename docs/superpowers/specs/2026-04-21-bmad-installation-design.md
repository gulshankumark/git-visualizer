# BMAD installation design

## Problem

This repository is a minimal project that does not yet include BMAD. The goal is to add BMAD using the upstream installation flow so the setup is shared in version control and aligned with BMAD's documented install process.

## Agreed scope

- Install BMAD into the repository root.
- Use the Copilot CLI / Others integration path.
- Commit the generated BMAD setup to the repository.
- Include additional modules only if they are clearly useful and generic for this repository.

## Recommended approach

Use the official BMAD installer from the repository root instead of manually creating BMAD files. This keeps the installation aligned with upstream expectations and reduces the chance of drift or missing generated assets.

## Alternatives considered

1. Interactive installer in the repo root. This is the recommended approach because it follows the documented path and lets the installer place the right files for the selected tool integration.
2. Non-interactive installer. This could be more repeatable, but only if the CLI flags cover the exact choices needed for this repo.
3. Manual file setup. This offers control but is the least reliable because it bypasses the install workflow BMAD expects.

## Design

### Installation strategy

Run the BMAD installer from the repository root and target the current directory so generated assets live inside the project. Select Copilot CLI / Others as the AI integration and keep the installation minimal unless the installer presents extra modules that are broadly useful rather than project-specific.

### Expected repository changes

The installer is expected to add BMAD-managed directories such as `_bmad` and `_bmad-output`, plus any prompt or tool-integration files required for the selected AI tool. Those generated files should be committed so the BMAD setup is shared with the repository instead of remaining local-only.

### Error handling

If the installer fails, use the failure output to correct the setup rather than hand-editing around the failure. If the installer succeeds but leaves an obvious mismatch with the agreed integration choice, make only the smallest follow-up change needed to reconcile the result.

### Verification

Confirm that the BMAD help entrypoint is available after installation and that the generated directory structure matches the installation choices. Verify that any extra installed modules are generic and useful for this repository rather than unnecessary add-ons.
