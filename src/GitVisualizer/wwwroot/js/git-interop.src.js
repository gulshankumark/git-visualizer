// git-interop.src.js — source for esbuild bundle (git-interop.js)
// Uses isomorphic-git 1.37.5 + lightning-fs 4.6.2
// Import from explicit ESM entry to avoid CJS crypto dependency
// (isomorphic-git exports map only exposes CJS; use direct file path)
import git from '../node_modules/isomorphic-git/index.js';
import LightningFS from '@isomorphic-git/lightning-fs';

const fs = new LightningFS('gitfs');
const dir = '/';
const author = { name: 'Learner', email: 'learner@git-visualizer' };

export async function gitInit() {
  try {
    await git.init({ fs, dir });
    await git.setConfig({ fs, dir, path: 'user.name', value: 'Learner' });
    await git.setConfig({ fs, dir, path: 'user.email', value: 'learner@git-visualizer' });
    await fs.promises.writeFile('/README.md', '# My Git Repository\n', { encoding: 'utf8' });
    return { success: true, message: 'Initialized empty Git repository in /' };
  } catch (e) {
    throw new Error(`git init failed: ${e.message}`);
  }
}

export async function gitAdd(filepath = '.') {
  try {
    if (filepath === '.') {
      const files = await git.statusMatrix({ fs, dir });
      await Promise.all(
        files
          .filter(([, head, workdir]) => workdir !== head)
          .map(([name]) => git.add({ fs, dir, filepath: name }))
      );
    } else {
      await git.add({ fs, dir, filepath });
    }
    return { success: true, message: 'Changes staged' };
  } catch (e) {
    throw new Error(`git add failed: ${e.message}`);
  }
}

export async function gitCommit(message) {
  try {
    const sha = await git.commit({ fs, dir, message, author });
    return { success: true, message: `[${sha.slice(0, 7)}] ${message}` };
  } catch (e) {
    if (e.message && e.message.includes('nothing to commit')) {
      throw new Error("Nothing to commit. Run 'git add .' first.");
    }
    throw new Error(`git commit failed: ${e.message}`);
  }
}

export async function gitBranch(name) {
  try {
    await git.branch({ fs, dir, ref: name });
    return { success: true, message: `Created branch '${name}'` };
  } catch (e) {
    throw new Error(`git branch failed: ${e.message}`);
  }
}

export async function gitCheckout(ref, createBranch = false) {
  try {
    if (createBranch) await git.branch({ fs, dir, ref });
    await git.checkout({ fs, dir, ref });
    return { success: true, message: `Switched to branch '${ref}'` };
  } catch (e) {
    throw new Error(`git checkout failed: ${e.message}`);
  }
}

export async function gitMerge(branch) {
  try {
    const result = await git.merge({ fs, dir, theirs: branch, author, committer: author });
    const msg = result.alreadyMerged ? 'Already up to date.'
              : result.fastForward   ? 'Fast-forward merge complete.'
                                     : 'Merge commit created.';
    return { success: true, message: msg, fastForward: result.fastForward ?? false };
  } catch (e) {
    throw new Error(`git merge failed: ${e.message}`);
  }
}

export async function gitLog(depth = 20) {
  try {
    const commits = await git.log({ fs, dir, depth });
    const lines = commits.map(c => {
      const date = new Date(c.commit.author.timestamp * 1000).toLocaleString();
      return `commit ${c.oid}\nAuthor: ${c.commit.author.name} <${c.commit.author.email}>\nDate:   ${date}\n\n    ${c.commit.message.trim()}`;
    });
    return { success: true, message: lines.join('\n\n') || '(no commits yet)', commits };
  } catch (e) {
    throw new Error(`git log failed: ${e.message}`);
  }
}
