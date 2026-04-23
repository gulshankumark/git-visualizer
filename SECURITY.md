# Security Policy

## Supported Versions

| Version | Supported |
|---|---|
| Latest (`main`) | ✅ |

---

## Reporting a Vulnerability

**Please do not open a public GitHub issue for security vulnerabilities.**

Report vulnerabilities via **[GitHub private vulnerability reporting](https://github.com/gulshankumark/git-visualizer/security/advisories/new)**.

Include in your report:
- A description of the vulnerability
- Steps to reproduce
- Potential impact
- Any suggested mitigations (optional)

You will receive a response within **7 days**. If the vulnerability is accepted, a fix will be developed and a security advisory published.

---

## Security Considerations

### XSS Surface — Lesson Content

The primary XSS surface in this application is **lesson content** (JSON/Markdown files in `wwwroot/lessons/`, a v2.0 feature). Community-contributed lesson files are rendered as HTML in the browser.

Mitigations in place:
- Blazor's built-in string encoding handles all normal string interpolation
- Any `MarkupString` usage must be sanitized before rendering
- Lesson JSON Schema validation will run in CI on every PR that touches `wwwroot/lessons/` (planned for v2.0 — not yet implemented)

If you find a case where unsanitized content reaches the DOM, please report it.

### No Server, No Accounts, No Off-Device Data

This application is a fully static site served by GitHub Pages:
- **No server-side code** — there is nothing to compromise server-side
- **No user accounts or authentication** — no credentials to steal
- **No data leaves the device** — all state is stored in the user's own browser (localStorage/IndexedDB)

The attack surface is limited to **client-side content injection** via lesson content.

### Dependency Licensing & Auditing

All dependencies must be MIT, Apache 2.0, or BSD licensed. Dependency licensing is reviewed manually. Dependabot configuration will be added when the project reaches beta.

---

## Scope

| In scope | Out of scope |
|---|---|
| XSS via lesson content rendering | Social engineering |
| Dependency vulnerabilities | GitHub Pages infrastructure |
| Client-side data leakage | Browser bugs |
