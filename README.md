# git-visualizer

Learn Git by doing — type commands and watch the commit graph update in real time.

[![Build & Deploy](https://github.com/gulshankumark/git-visualizer/actions/workflows/deploy.yml/badge.svg)](https://github.com/gulshankumark/git-visualizer/actions/workflows/deploy.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

<!-- TODO: add demo GIF -->
![git-visualizer demo placeholder](https://placehold.co/800x450?text=demo+coming+soon)

**[🚀 Try it live](https://gulshankumark.github.io/git-visualizer/)**

---

## What is this?

**git-visualizer** is a browser-based interactive Git sandbox. Type `git init`, `git commit`, `git branch`, and more — and see the commit graph update instantly. No installation, no account, nothing to break. Just open it and learn.

---

## Quick Start

### Prerequisites

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 10.0 or later | https://dot.net |
| Node.js | 24 or later | https://nodejs.org |

### Run locally

```bash
# 1. Clone the repository
git clone https://github.com/gulshankumark/git-visualizer.git
cd git-visualizer

# 2. Install JavaScript dependencies
cd src/GitVisualizer/wwwroot
npm install
cd ../../..

# 3. Start the development server
dotnet watch run --project src/GitVisualizer
```

Open your browser at **http://localhost:5000/git-visualizer/**

> **Note:** `dotnet watch run` may auto-open your browser to the root (`http://localhost:5000`). If so, navigate manually to **http://localhost:5000/git-visualizer/**.

---

## Browser Support

| Browser | Support |
|---|---|
| Chrome | Last 2 major versions ✅ |
| Edge | Last 2 major versions ✅ |
| Firefox | Last 2 major versions ✅ |
| Safari | Last 2 major versions ✅ |

Requires WebAssembly support (available in all modern browsers).

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for build prerequisites, coding standards, and PR process.

---

## License

[MIT](LICENSE) © 2026 gulshankumark
