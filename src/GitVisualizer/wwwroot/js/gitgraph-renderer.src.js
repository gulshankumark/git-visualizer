// gitgraph-renderer.src.js — source for esbuild bundle (gitgraph-renderer.js)
// Stage 1 renderer: backed by @gitgraph/js (archived but stable, zero deps).
// The C#/JS contract — initRenderer / renderGraph(containerId, payload) / scrollToHead —
// is renderer-agnostic so Stage 2 (custom SVG) can replace this file in isolation.
import { createGitgraph, templateExtend, TemplateName } from '@gitgraph/js';

let _template = null;

function readCss(name) {
    return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}

export async function initRenderer(_isDark) {
    const branchColors = [0, 1, 2, 3, 4, 5].map((i) => readCss(`--gitvis-graph-branch-${i}`));
    const nodeText     = readCss('--gitvis-graph-node-text');
    const edge         = readCss('--gitvis-graph-edge');

    _template = templateExtend(TemplateName.Metro, {
        colors: branchColors,
        branch: {
            lineWidth: 4,
            spacing: 30,
            mergeStyle: 'straight',
            label: {
                font: 'normal 13px var(--gitvis-font-ui, sans-serif)',
                bgColor: 'transparent',
                strokeColor: 'transparent',
            },
        },
        commit: {
            spacing: 60,
            dot: { size: 10, strokeWidth: 0 },
            message: {
                color: nodeText,
                displayHash: true,
                displayAuthor: false,
                font: 'normal 13px var(--gitvis-font-mono, monospace)',
            },
        },
        arrow: { color: edge, size: 0, offset: 0 },
        tag: {
            color: nodeText,
            bgColor: 'transparent',
            font: 'bold 12px var(--gitvis-font-mono, monospace)',
            borderRadius: 4,
            pointerWidth: 6,
        },
    });
}

// Resolve a Branch object for the named branch, creating it lazily off its
// fork point when first encountered.
function getOrCreateBranch(branches, gitgraph, payload, branchName, forkParentOid) {
    if (branches.has(branchName)) return branches.get(branchName);

    let created;
    if (!forkParentOid) {
        created = gitgraph.branch(branchName);
    } else {
        const parentBranchName = payload.commitBranch?.[forkParentOid] ?? payload.trunkBranch;
        const parentBranch = branches.get(parentBranchName);
        created = parentBranch ? parentBranch.branch(branchName) : gitgraph.branch(branchName);
    }
    branches.set(branchName, created);
    return created;
}

export async function renderGraph(containerId, payload) {
    const container = document.getElementById(containerId);
    if (!container || !payload || !payload.commits || payload.commits.length === 0) return;
    if (!_template) await initRenderer(false);

    container.innerHTML = '';

    const gitgraph = createGitgraph(container, {
        template: _template,
        orientation: 'horizontal',
    });

    const branches = new Map();

    for (const c of payload.commits) {
        const subject = (c.message || '').split('\n')[0];
        const forkParent = c.parents && c.parents.length > 0 ? c.parents[0] : null;
        const branch = getOrCreateBranch(branches, gitgraph, payload, c.branch, forkParent);

        if (c.isMerge && c.parents.length >= 2) {
            const sourceBranchName = payload.commitBranch?.[c.parents[1]];
            const sourceBranch = sourceBranchName ? branches.get(sourceBranchName) : null;
            if (sourceBranch) {
                branch.merge({
                    branch: sourceBranch,
                    commitOptions: {
                        hash: c.oid,
                        subject,
                        author: c.author,
                        tag: c.isHead ? 'HEAD' : undefined,
                    },
                });
                continue;
            }
        }

        branch.commit({
            hash: c.oid,
            subject,
            author: c.author,
            tag: c.isHead ? `HEAD → ${payload.headBranch}` : undefined,
            dotText: undefined,
            style: c.isHead ? { dot: { strokeWidth: 2, strokeColor: readCss('--gitvis-graph-node-text') } } : undefined,
        });
    }
}

export function scrollToHead(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;
    const head = container.querySelector('.commit-highlight');
    if (head) {
        head.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'end' });
    } else {
        container.scrollLeft = container.scrollWidth;
    }
}
