var __create = Object.create;
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __commonJS = (cb, mod) => function __require() {
  return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(
  // If the importer is in node compatibility mode or this is not an ESM
  // file that has been converted to a CommonJS file using a Babel-
  // compatible transform (i.e. "__esModule" has not been set), then set
  // "default" to the CommonJS "module.exports" for node compatibility.
  isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target,
  mod
));

// node_modules/@gitgraph/js/lib/gitgraph.umd.js
var require_gitgraph_umd = __commonJS({
  "node_modules/@gitgraph/js/lib/gitgraph.umd.js"(exports, module) {
    (function(global2, factory) {
      typeof exports === "object" && typeof module !== "undefined" ? factory(exports) : typeof define === "function" && define.amd ? define(["exports"], factory) : (global2 = global2 || self, factory(global2.GitgraphJS = {}));
    })(exports, function(exports2) {
      "use strict";
      var commonjsGlobal = typeof globalThis !== "undefined" ? globalThis : typeof window !== "undefined" ? window : typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : {};
      function commonjsRequire() {
        throw new Error("Dynamic requires are not currently supported by rollup-plugin-commonjs");
      }
      function unwrapExports(x) {
        return x && x.__esModule && Object.prototype.hasOwnProperty.call(x, "default") ? x["default"] : x;
      }
      function createCommonjsModule(fn, module2) {
        return module2 = { exports: {} }, fn(module2, module2.exports), module2.exports;
      }
      function getCjsExportFromNamespace(n) {
        return n && n["default"] || n;
      }
      var commonjsHelpers = /* @__PURE__ */ Object.freeze({
        commonjsGlobal,
        commonjsRequire,
        unwrapExports,
        createCommonjsModule,
        getCjsExportFromNamespace
      });
      var orientation = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        var Orientation;
        (function(Orientation2) {
          Orientation2["VerticalReverse"] = "vertical-reverse";
          Orientation2["Horizontal"] = "horizontal";
          Orientation2["HorizontalReverse"] = "horizontal-reverse";
        })(Orientation = exports3.Orientation || (exports3.Orientation = {}));
      });
      unwrapExports(orientation);
      var orientation_1 = orientation.Orientation;
      var utils = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        function booleanOptionOr(value, defaultValue) {
          return typeof value === "boolean" ? value : defaultValue;
        }
        exports3.booleanOptionOr = booleanOptionOr;
        function numberOptionOr(value, defaultValue) {
          return typeof value === "number" ? value : defaultValue;
        }
        exports3.numberOptionOr = numberOptionOr;
        function pick(obj, paths) {
          return Object.assign({}, paths.reduce((mem, key) => Object.assign({}, mem, { [key]: obj[key] }), {}));
        }
        exports3.pick = pick;
        function debug(commits, paths) {
          console.log(JSON.stringify(commits.map((commit2) => pick(commit2, paths)), null, 2));
        }
        exports3.debug = debug;
        function isUndefined(obj) {
          return obj === void 0;
        }
        exports3.isUndefined = isUndefined;
        function withoutUndefinedKeys(obj = {}) {
          return Object.keys(obj).reduce((mem, key) => isUndefined(obj[key]) ? mem : Object.assign({}, mem, { [key]: obj[key] }), {});
        }
        exports3.withoutUndefinedKeys = withoutUndefinedKeys;
        function arrowSvgPath(graph, parent, commit2) {
          const commitRadius = commit2.style.dot.size;
          const size = graph.template.arrow.size;
          const h = commitRadius + graph.template.arrow.offset;
          const delta = Math.PI / 7;
          const alpha = getAlpha(graph, parent, commit2);
          const x1 = h * Math.cos(alpha);
          const y1 = h * Math.sin(alpha);
          const x2 = (h + size) * Math.cos(alpha - delta);
          const y2 = (h + size) * Math.sin(alpha - delta);
          const x3 = (h + size / 2) * Math.cos(alpha);
          const y3 = (h + size / 2) * Math.sin(alpha);
          const x4 = (h + size) * Math.cos(alpha + delta);
          const y4 = (h + size) * Math.sin(alpha + delta);
          return `M${x1},${y1} L${x2},${y2} Q${x3},${y3} ${x4},${y4} L${x4},${y4}`;
        }
        exports3.arrowSvgPath = arrowSvgPath;
        function getAlpha(graph, parent, commit2) {
          const deltaX = parent.x - commit2.x;
          const deltaY = parent.y - commit2.y;
          const commitSpacing = graph.template.commit.spacing;
          let alphaY;
          let alphaX;
          switch (graph.orientation) {
            case orientation.Orientation.Horizontal:
              alphaY = deltaY;
              alphaX = -commitSpacing;
              break;
            case orientation.Orientation.HorizontalReverse:
              alphaY = deltaY;
              alphaX = commitSpacing;
              break;
            case orientation.Orientation.VerticalReverse:
              alphaY = -commitSpacing;
              alphaX = deltaX;
              break;
            default:
              alphaY = commitSpacing;
              alphaX = deltaX;
              break;
          }
          if (graph.isVertical) {
            if (Math.abs(deltaY) > commitSpacing)
              alphaX = 0;
          } else {
            if (Math.abs(deltaX) > commitSpacing)
              alphaY = 0;
          }
          if (graph.reverseArrow) {
            alphaY *= -1;
            alphaX *= -1;
          }
          return Math.atan2(alphaY, alphaX);
        }
      });
      unwrapExports(utils);
      var utils_1 = utils.booleanOptionOr;
      var utils_2 = utils.numberOptionOr;
      var utils_3 = utils.pick;
      var utils_4 = utils.debug;
      var utils_5 = utils.isUndefined;
      var utils_6 = utils.withoutUndefinedKeys;
      var utils_7 = utils.arrowSvgPath;
      var template = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        var MergeStyle;
        (function(MergeStyle2) {
          MergeStyle2["Bezier"] = "bezier";
          MergeStyle2["Straight"] = "straight";
        })(MergeStyle || (MergeStyle = {}));
        exports3.MergeStyle = MergeStyle;
        exports3.DEFAULT_FONT = "normal 12pt Calibri";
        class Template {
          constructor(options) {
            options.branch = options.branch || {};
            options.branch.label = options.branch.label || {};
            options.arrow = options.arrow || {};
            options.commit = options.commit || {};
            options.commit.dot = options.commit.dot || {};
            options.commit.message = options.commit.message || {};
            this.colors = options.colors || ["#000000"];
            this.branch = {
              color: options.branch.color,
              lineWidth: options.branch.lineWidth || 2,
              mergeStyle: options.branch.mergeStyle || MergeStyle.Bezier,
              spacing: utils.numberOptionOr(options.branch.spacing, 20),
              label: {
                display: utils.booleanOptionOr(options.branch.label.display, true),
                color: options.branch.label.color || options.commit.color,
                strokeColor: options.branch.label.strokeColor || options.commit.color,
                bgColor: options.branch.label.bgColor || "white",
                font: options.branch.label.font || options.commit.message.font || exports3.DEFAULT_FONT,
                borderRadius: utils.numberOptionOr(options.branch.label.borderRadius, 10)
              }
            };
            this.arrow = {
              size: options.arrow.size || null,
              color: options.arrow.color || null,
              offset: options.arrow.offset || 2
            };
            this.commit = {
              color: options.commit.color,
              spacing: utils.numberOptionOr(options.commit.spacing, 25),
              hasTooltipInCompactMode: utils.booleanOptionOr(options.commit.hasTooltipInCompactMode, true),
              dot: {
                color: options.commit.dot.color || options.commit.color,
                size: options.commit.dot.size || 3,
                strokeWidth: utils.numberOptionOr(options.commit.dot.strokeWidth, 0),
                strokeColor: options.commit.dot.strokeColor,
                font: options.commit.dot.font || options.commit.message.font || "normal 10pt Calibri"
              },
              message: {
                display: utils.booleanOptionOr(options.commit.message.display, true),
                displayAuthor: utils.booleanOptionOr(options.commit.message.displayAuthor, true),
                displayHash: utils.booleanOptionOr(options.commit.message.displayHash, true),
                color: options.commit.message.color || options.commit.color,
                font: options.commit.message.font || exports3.DEFAULT_FONT
              }
            };
            this.tag = options.tag || {};
          }
        }
        exports3.Template = Template;
        const blackArrowTemplate = new Template({
          colors: ["#6963FF", "#47E8D4", "#6BDB52", "#E84BA5", "#FFA657"],
          branch: {
            color: "#000000",
            lineWidth: 4,
            spacing: 50,
            mergeStyle: MergeStyle.Straight
          },
          commit: {
            spacing: 60,
            dot: {
              size: 16,
              strokeColor: "#000000",
              strokeWidth: 4
            },
            message: {
              color: "black"
            }
          },
          arrow: {
            size: 16,
            offset: -1.5
          }
        });
        exports3.blackArrowTemplate = blackArrowTemplate;
        const metroTemplate = new Template({
          colors: ["#979797", "#008fb5", "#f1c109"],
          branch: {
            lineWidth: 10,
            spacing: 50
          },
          commit: {
            spacing: 80,
            dot: {
              size: 14
            },
            message: {
              font: "normal 14pt Arial"
            }
          }
        });
        exports3.metroTemplate = metroTemplate;
        var TemplateName2;
        (function(TemplateName3) {
          TemplateName3["Metro"] = "metro";
          TemplateName3["BlackArrow"] = "blackarrow";
        })(TemplateName2 || (TemplateName2 = {}));
        exports3.TemplateName = TemplateName2;
        function templateExtend2(selectedTemplate, options) {
          const template2 = getTemplate(selectedTemplate);
          if (!options.branch)
            options.branch = {};
          if (!options.commit)
            options.commit = {};
          return {
            colors: options.colors || template2.colors,
            arrow: Object.assign({}, template2.arrow, options.arrow),
            branch: Object.assign({}, template2.branch, options.branch, { label: Object.assign({}, template2.branch.label, options.branch.label) }),
            commit: Object.assign({}, template2.commit, options.commit, { dot: Object.assign({}, template2.commit.dot, options.commit.dot), message: Object.assign({}, template2.commit.message, options.commit.message) }),
            tag: Object.assign({}, template2.tag, options.tag)
          };
        }
        exports3.templateExtend = templateExtend2;
        function getTemplate(template2) {
          if (!template2)
            return metroTemplate;
          if (typeof template2 === "string") {
            return {
              [TemplateName2.BlackArrow]: blackArrowTemplate,
              [TemplateName2.Metro]: metroTemplate
            }[template2];
          }
          return template2;
        }
        exports3.getTemplate = getTemplate;
      });
      unwrapExports(template);
      var template_1 = template.MergeStyle;
      var template_2 = template.DEFAULT_FONT;
      var template_3 = template.Template;
      var template_4 = template.blackArrowTemplate;
      var template_5 = template.metroTemplate;
      var template_6 = template.TemplateName;
      var template_7 = template.templateExtend;
      var template_8 = template.getTemplate;
      var tag = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class Tag {
          constructor(name, style, render, commitStyle) {
            this.name = name;
            this.tagStyle = style;
            this.commitStyle = commitStyle;
            this.render = render;
          }
          /**
           * Style
           */
          get style() {
            return {
              strokeColor: this.tagStyle.strokeColor || this.commitStyle.color,
              bgColor: this.tagStyle.bgColor || this.commitStyle.color,
              color: this.tagStyle.color || "white",
              font: this.tagStyle.font || this.commitStyle.message.font || template.DEFAULT_FONT,
              borderRadius: utils.numberOptionOr(this.tagStyle.borderRadius, 10),
              pointerWidth: utils.numberOptionOr(this.tagStyle.pointerWidth, 12)
            };
          }
        }
        exports3.Tag = Tag;
      });
      unwrapExports(tag);
      var tag_1 = tag.Tag;
      var commit = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        const getRandomHash = () => (Math.random().toString(16).substring(3) + Math.random().toString(16).substring(3) + Math.random().toString(16).substring(3) + Math.random().toString(16).substring(3)).substring(0, 40);
        class Commit {
          constructor(options) {
            this.refs = [];
            this.x = 0;
            this.y = 0;
            let name, email;
            try {
              [, name, email] = options.author.match(/(.*) <(.*)>/);
            } catch (e) {
              [name, email] = [options.author, ""];
            }
            this.author = { name, email, timestamp: Date.now() };
            this.committer = { name, email, timestamp: Date.now() };
            this.subject = options.subject;
            this.body = options.body || "";
            this.hash = options.hash || getRandomHash();
            this.hashAbbrev = this.hash.substring(0, 7);
            this.parents = options.parents ? options.parents : [];
            this.parentsAbbrev = this.parents.map((commit2) => commit2.substring(0, 7));
            this.style = Object.assign({}, options.style, { message: Object.assign({}, options.style.message), dot: Object.assign({}, options.style.dot) });
            this.dotText = options.dotText;
            this.onClick = () => options.onClick ? options.onClick(this) : void 0;
            this.onMessageClick = () => options.onMessageClick ? options.onMessageClick(this) : void 0;
            this.onMouseOver = () => options.onMouseOver ? options.onMouseOver(this) : void 0;
            this.onMouseOut = () => options.onMouseOut ? options.onMouseOut(this) : void 0;
            this.renderDot = options.renderDot;
            this.renderMessage = options.renderMessage;
            this.renderTooltip = options.renderTooltip;
          }
          /**
           * Message
           */
          get message() {
            let message = "";
            if (this.style.message.displayHash) {
              message += `${this.hashAbbrev} `;
            }
            message += this.subject;
            if (this.style.message.displayAuthor) {
              message += ` - ${this.author.name} <${this.author.email}>`;
            }
            return message;
          }
          /**
           * Branch that should be rendered
           */
          get branchToDisplay() {
            return this.branches ? this.branches[0] : "";
          }
          setRefs(refs2) {
            this.refs = refs2.getNames(this.hash);
            return this;
          }
          setTags(tags, getTagStyle, getTagRender) {
            this.tags = tags.getNames(this.hash).map((name) => new tag.Tag(name, getTagStyle(name), getTagRender(name), this.style));
            return this;
          }
          setBranches(branches) {
            this.branches = branches;
            return this;
          }
          setPosition({ x, y }) {
            this.x = x;
            this.y = y;
            return this;
          }
          withDefaultColor(color) {
            const newStyle = Object.assign({}, this.style, { dot: Object.assign({}, this.style.dot), message: Object.assign({}, this.style.message) });
            if (!newStyle.color)
              newStyle.color = color;
            if (!newStyle.dot.color)
              newStyle.dot.color = color;
            if (!newStyle.message.color)
              newStyle.message.color = color;
            const commit2 = this.cloneCommit();
            commit2.style = newStyle;
            return commit2;
          }
          /**
           * Ideally, we want Commit to be a [Value Object](https://martinfowler.com/bliki/ValueObject.html).
           * We started with a mutable class. So we'll refactor that little by little.
           * This private function is a helper to create a new Commit from existing one.
           */
          cloneCommit() {
            const commit2 = new Commit({
              author: `${this.author.name} <${this.author.email}>`,
              subject: this.subject,
              style: this.style,
              body: this.body,
              hash: this.hash,
              parents: this.parents,
              dotText: this.dotText,
              onClick: this.onClick,
              onMessageClick: this.onMessageClick,
              onMouseOver: this.onMouseOver,
              onMouseOut: this.onMouseOut,
              renderDot: this.renderDot,
              renderMessage: this.renderMessage,
              renderTooltip: this.renderTooltip
            });
            commit2.refs = this.refs;
            commit2.branches = this.branches;
            commit2.tags = this.tags;
            commit2.x = this.x;
            commit2.y = this.y;
            return commit2;
          }
        }
        exports3.Commit = Commit;
      });
      unwrapExports(commit);
      var commit_1 = commit.Commit;
      var branchUserApi = createCommonjsModule(function(module2, exports3) {
        var __rest = commonjsGlobal && commonjsGlobal.__rest || function(s, e) {
          var t = {};
          for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
            t[p] = s[p];
          if (s != null && typeof Object.getOwnPropertySymbols === "function")
            for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
              if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                t[p[i]] = s[p[i]];
            }
          return t;
        };
        Object.defineProperty(exports3, "__esModule", { value: true });
        class BranchUserApi {
          // tslint:enable:variable-name
          constructor(branch2, graph, onGraphUpdate) {
            this._branch = branch2;
            this.name = branch2.name;
            this._graph = graph;
            this._onGraphUpdate = onGraphUpdate;
          }
          branch(args) {
            if (this._branch.isDeleted() && !this._isReferenced()) {
              throw new Error(`Cannot branch from the deleted branch "${this.name}"`);
            }
            const options = typeof args === "string" ? { name: args } : args;
            options.from = this;
            return this._graph.createBranch(options).getUserApi();
          }
          commit(options) {
            if (this._branch.isDeleted() && !this._isReferenced()) {
              throw new Error(`Cannot commit on the deleted branch "${this.name}"`);
            }
            if (typeof options === "string")
              options = { subject: options };
            if (!options)
              options = {};
            this._commitWithParents(options, []);
            this._onGraphUpdate();
            return this;
          }
          /**
           * Delete the branch (as `git branch -d`)
           */
          delete() {
            if (this._graph.refs.getCommit("HEAD") === this._graph.refs.getCommit(this.name)) {
              throw new Error(`Cannot delete the checked out branch "${this.name}"`);
            }
            const branchCommits = (function* (graph, branch2) {
              const lookupCommit = (graph2, commitHash) => {
                return graph2.commits.find(({ hash }) => hash === commitHash);
              };
              let currentCommit = lookupCommit(graph, graph.refs.getCommit(branch2.name));
              while (currentCommit && currentCommit.hash !== branch2.parentCommitHash) {
                yield currentCommit;
                currentCommit = lookupCommit(graph, currentCommit.parents[0]);
              }
              return;
            })(this._graph, this._branch);
            [...branchCommits].forEach((commit2) => {
              commit2.refs = commit2.refs.filter((branchName) => branchName !== this.name);
            });
            this._graph.refs.delete(this.name);
            this._graph.branches.delete(this.name);
            this._branch = branch.createDeletedBranch(this._graph, this._branch.style, () => {
            });
            this._onGraphUpdate();
            return this;
          }
          merge(...args) {
            if (this._branch.isDeleted() && !this._isReferenced()) {
              throw new Error(`Cannot merge to the deleted branch "${this.name}"`);
            }
            let options = args[0];
            if (!isBranchMergeOptions(options)) {
              options = {
                branch: args[0],
                fastForward: false,
                commitOptions: { subject: args[1] }
              };
            }
            const { branch: branch2, fastForward, commitOptions } = options;
            const branchName = typeof branch2 === "string" ? branch2 : branch2.name;
            const branchLastCommitHash = this._graph.refs.getCommit(branchName);
            if (!branchLastCommitHash) {
              throw new Error(`The branch called "${branchName}" is unknown`);
            }
            let canFastForward = false;
            if (fastForward) {
              const lastCommitHash = this._graph.refs.getCommit(this._branch.name);
              if (lastCommitHash) {
                canFastForward = this._areCommitsConnected(lastCommitHash, branchLastCommitHash);
              }
            }
            if (fastForward && canFastForward) {
              this._fastForwardTo(branchLastCommitHash);
            } else {
              this._commitWithParents(Object.assign({}, commitOptions, { subject: commitOptions && commitOptions.subject || `Merge branch ${branchName}` }), [branchLastCommitHash]);
            }
            this._onGraphUpdate();
            return this;
          }
          tag(options) {
            if (this._branch.isDeleted() && !this._isReferenced()) {
              throw new Error(`Cannot tag on the deleted branch "${this.name}"`);
            }
            if (typeof options === "string") {
              this._graph.getUserApi().tag({ name: options, ref: this._branch.name });
            } else {
              this._graph.getUserApi().tag(Object.assign({}, options, { ref: this._branch.name }));
            }
            return this;
          }
          /**
           * Checkout onto this branch and update "HEAD" in refs
           */
          checkout() {
            if (this._branch.isDeleted() && !this._isReferenced()) {
              throw new Error(`Cannot checkout the deleted branch "${this.name}"`);
            }
            const target = this._branch;
            const headCommit = this._graph.refs.getCommit(target.name);
            this._graph.currentBranch = target;
            if (headCommit) {
              this._graph.refs.set("HEAD", headCommit);
            }
            return this;
          }
          // tslint:disable:variable-name - Prefix `_` = explicitly private for JS users
          _commitWithParents(options, parents) {
            const parentOnSameBranch = this._graph.refs.getCommit(this._branch.name);
            if (parentOnSameBranch) {
              parents.unshift(parentOnSameBranch);
            } else if (this._branch.parentCommitHash) {
              parents.unshift(this._branch.parentCommitHash);
            }
            const { tag: tag2 } = options, commitOptions = __rest(options, ["tag"]);
            const commit$1 = new commit.Commit(Object.assign({ hash: this._graph.generateCommitHash(), author: this._branch.commitDefaultOptions.author || this._graph.author, subject: this._branch.commitDefaultOptions.subject || this._graph.commitMessage }, commitOptions, { parents, style: this._getCommitStyle(options.style) }));
            if (parentOnSameBranch) {
              const parentRefs = this._graph.refs.getNames(parentOnSameBranch);
              parentRefs.forEach((ref) => this._graph.refs.set(ref, commit$1.hash));
            } else {
              this._graph.refs.set(this._branch.name, commit$1.hash);
            }
            this._graph.commits.push(commit$1);
            this.checkout();
            if (tag2)
              this.tag(tag2);
          }
          _areCommitsConnected(parentCommitHash, childCommitHash) {
            const childCommit = this._graph.commits.find(({ hash }) => childCommitHash === hash);
            if (!childCommit)
              return false;
            const isFirstCommitOfGraph = childCommit.parents.length === 0;
            if (isFirstCommitOfGraph)
              return false;
            if (childCommit.parents.includes(parentCommitHash)) {
              return true;
            }
            return childCommit.parents.some((directParentHash) => this._areCommitsConnected(parentCommitHash, directParentHash));
          }
          _fastForwardTo(commitHash) {
            this._graph.refs.set(this._branch.name, commitHash);
          }
          _getCommitStyle(style = {}) {
            return Object.assign({}, utils.withoutUndefinedKeys(this._graph.template.commit), utils.withoutUndefinedKeys(this._branch.commitDefaultOptions.style), style, { message: Object.assign({}, utils.withoutUndefinedKeys(this._graph.template.commit.message), utils.withoutUndefinedKeys(this._branch.commitDefaultOptions.style.message), style.message, utils.withoutUndefinedKeys({
              display: this._graph.shouldDisplayCommitMessage && void 0
            })), dot: Object.assign({}, utils.withoutUndefinedKeys(this._graph.template.commit.dot), utils.withoutUndefinedKeys(this._branch.commitDefaultOptions.style.dot), style.dot) });
          }
          _isReferenced() {
            return this._graph.branches.has(this.name) || this._graph.refs.hasName(this.name) || this._graph.commits.reduce((allNames, { refs: refs2 }) => [...allNames, ...refs2], []).includes(this.name);
          }
        }
        exports3.BranchUserApi = BranchUserApi;
        function isBranchMergeOptions(options) {
          return typeof options === "object" && !(options instanceof BranchUserApi);
        }
      });
      unwrapExports(branchUserApi);
      var branchUserApi_1 = branchUserApi.BranchUserApi;
      var branch = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        const DELETED_BRANCH_NAME = "";
        exports3.DELETED_BRANCH_NAME = DELETED_BRANCH_NAME;
        class Branch {
          constructor(options) {
            this.gitgraph = options.gitgraph;
            this.name = options.name;
            this.style = options.style;
            this.parentCommitHash = options.parentCommitHash;
            this.commitDefaultOptions = options.commitDefaultOptions || { style: {} };
            this.onGraphUpdate = options.onGraphUpdate;
            this.renderLabel = options.renderLabel;
          }
          /**
           * Return the API to manipulate Gitgraph branch as a user.
           */
          getUserApi() {
            return new branchUserApi.BranchUserApi(this, this.gitgraph, this.onGraphUpdate);
          }
          /**
           * Return true if branch was deleted.
           */
          isDeleted() {
            return this.name === DELETED_BRANCH_NAME;
          }
        }
        exports3.Branch = Branch;
        function createDeletedBranch(gitgraph2, style, onGraphUpdate) {
          return new Branch({
            name: DELETED_BRANCH_NAME,
            gitgraph: gitgraph2,
            style,
            onGraphUpdate
          });
        }
        exports3.createDeletedBranch = createDeletedBranch;
      });
      unwrapExports(branch);
      var branch_1 = branch.DELETED_BRANCH_NAME;
      var branch_2 = branch.Branch;
      var branch_3 = branch.createDeletedBranch;
      var mode = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        var Mode;
        (function(Mode2) {
          Mode2["Compact"] = "compact";
        })(Mode || (Mode = {}));
        exports3.Mode = Mode;
      });
      unwrapExports(mode);
      var mode_1 = mode.Mode;
      var regular = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class RegularGraphRows {
          constructor(commits) {
            this.rows = /* @__PURE__ */ new Map();
            this.maxRowCache = void 0;
            this.computeRowsFromCommits(commits);
          }
          getRowOf(commitHash) {
            return this.rows.get(commitHash) || 0;
          }
          getMaxRow() {
            if (this.maxRowCache === void 0) {
              this.maxRowCache = uniq(Array.from(this.rows.values())).length - 1;
            }
            return this.maxRowCache;
          }
          computeRowsFromCommits(commits) {
            commits.forEach((commit2, i) => {
              this.rows.set(commit2.hash, i);
            });
            this.maxRowCache = void 0;
          }
        }
        exports3.RegularGraphRows = RegularGraphRows;
        function uniq(array) {
          const set = /* @__PURE__ */ new Set();
          array.forEach((value) => set.add(value));
          return Array.from(set);
        }
      });
      unwrapExports(regular);
      var regular_1 = regular.RegularGraphRows;
      var compact = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class CompactGraphRows extends regular.RegularGraphRows {
          computeRowsFromCommits(commits) {
            commits.forEach((commit2, i) => {
              let newRow = i;
              const isFirstCommit = i === 0;
              if (!isFirstCommit) {
                const parentRow = this.getRowOf(commit2.parents[0]);
                const historyParent = commits[i - 1];
                newRow = Math.max(parentRow + 1, this.getRowOf(historyParent.hash));
                const isMergeCommit = commit2.parents.length > 1;
                if (isMergeCommit) {
                  const mergeTargetParentRow = this.getRowOf(commit2.parents[1]);
                  if (parentRow < mergeTargetParentRow)
                    newRow++;
                }
              }
              this.rows.set(commit2.hash, newRow);
            });
          }
        }
        exports3.CompactGraphRows = CompactGraphRows;
      });
      unwrapExports(compact);
      var compact_1 = compact.CompactGraphRows;
      var graphRows = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        exports3.GraphRows = regular.RegularGraphRows;
        function createGraphRows(mode$1, commits) {
          return mode$1 === mode.Mode.Compact ? new compact.CompactGraphRows(commits) : new regular.RegularGraphRows(commits);
        }
        exports3.createGraphRows = createGraphRows;
      });
      unwrapExports(graphRows);
      var graphRows_1 = graphRows.GraphRows;
      var graphRows_2 = graphRows.createGraphRows;
      var branchesOrder = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class BranchesOrder {
          constructor(commits, colors, compareFunction) {
            this.branches = /* @__PURE__ */ new Set();
            this.colors = colors;
            commits.forEach((commit2) => this.branches.add(commit2.branchToDisplay));
            if (compareFunction) {
              this.branches = new Set(Array.from(this.branches).sort(compareFunction));
            }
          }
          /**
           * Return the order of the given branch name.
           *
           * @param branchName Name of the branch
           */
          get(branchName) {
            return Array.from(this.branches).findIndex((branch2) => branch2 === branchName);
          }
          /**
           * Return the color of the given branch.
           *
           * @param branchName Name of the branch
           */
          getColorOf(branchName) {
            return this.colors[this.get(branchName) % this.colors.length];
          }
        }
        exports3.BranchesOrder = BranchesOrder;
      });
      unwrapExports(branchesOrder);
      var branchesOrder_1 = branchesOrder.BranchesOrder;
      var refs = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class Refs {
          constructor() {
            this.commitPerName = /* @__PURE__ */ new Map();
            this.namesPerCommit = /* @__PURE__ */ new Map();
          }
          /**
           * Set a new reference to a commit hash.
           *
           * @param name Name of the ref (ex: "master", "v1.0")
           * @param commitHash Commit hash
           */
          set(name, commitHash) {
            const prevCommitHash = this.commitPerName.get(name);
            if (prevCommitHash) {
              this.removeNameFrom(prevCommitHash, name);
            }
            this.addNameTo(commitHash, name);
            this.addCommitTo(name, commitHash);
            return this;
          }
          /**
           * Delete a reference
           *
           * @param name Name of the reference
           */
          delete(name) {
            if (this.hasName(name)) {
              this.removeNameFrom(this.getCommit(name), name);
              this.commitPerName.delete(name);
            }
            return this;
          }
          /**
           * Get the commit hash associated with the given reference name.
           *
           * @param name Name of the ref
           */
          getCommit(name) {
            return this.commitPerName.get(name);
          }
          /**
           * Get the list of reference names associated with given commit hash.
           *
           * @param commitHash Commit hash
           */
          getNames(commitHash) {
            return this.namesPerCommit.get(commitHash) || [];
          }
          /**
           * Get all reference names known.
           */
          getAllNames() {
            return Array.from(this.commitPerName.keys());
          }
          /**
           * Returns true if given commit hash is referenced.
           *
           * @param commitHash Commit hash
           */
          hasCommit(commitHash) {
            return this.namesPerCommit.has(commitHash);
          }
          /**
           * Returns true if given reference name exists.
           *
           * @param name Name of the ref
           */
          hasName(name) {
            return this.commitPerName.has(name);
          }
          removeNameFrom(commitHash, nameToRemove) {
            const names = this.namesPerCommit.get(commitHash) || [];
            this.namesPerCommit.set(commitHash, names.filter((name) => name !== nameToRemove));
          }
          addNameTo(commitHash, nameToAdd) {
            const prevNames = this.namesPerCommit.get(commitHash) || [];
            this.namesPerCommit.set(commitHash, [...prevNames, nameToAdd]);
          }
          addCommitTo(name, commitHashToAdd) {
            this.commitPerName.set(name, commitHashToAdd);
          }
        }
        exports3.Refs = Refs;
      });
      unwrapExports(refs);
      var refs_1 = refs.Refs;
      var branchesPaths = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class BranchesPathsCalculator {
          constructor(commits, branches, commitSpacing, isGraphVertical, isGraphReverse, createDeletedBranch) {
            this.branchesPaths = /* @__PURE__ */ new Map();
            this.commits = commits;
            this.branches = branches;
            this.commitSpacing = commitSpacing;
            this.isGraphVertical = isGraphVertical;
            this.isGraphReverse = isGraphReverse;
            this.createDeletedBranch = createDeletedBranch;
          }
          /**
           * Compute branches paths for graph.
           */
          execute() {
            this.fromCommits();
            this.withMergeCommits();
            return this.smoothBranchesPaths();
          }
          /**
           * Initialize branches paths from calculator's commits.
           */
          fromCommits() {
            this.commits.forEach((commit2) => {
              let branch2 = this.branches.get(commit2.branchToDisplay);
              if (!branch2) {
                branch2 = this.getDeletedBranchInPath() || this.createDeletedBranch();
              }
              const path = [];
              const existingBranchPath = this.branchesPaths.get(branch2);
              const firstParentCommit = this.commits.find(({ hash }) => hash === commit2.parents[0]);
              if (existingBranchPath) {
                path.push(...existingBranchPath);
              } else if (firstParentCommit) {
                path.push({ x: firstParentCommit.x, y: firstParentCommit.y });
              }
              path.push({ x: commit2.x, y: commit2.y });
              this.branchesPaths.set(branch2, path);
            });
          }
          /**
           * Insert merge commits points into `branchesPaths`.
           *
           * @example
           *     // Before
           *     [
           *       { x: 0, y: 640 },
           *       { x: 50, y: 560 }
           *     ]
           *
           *     // After
           *     [
           *       { x: 0, y: 640 },
           *       { x: 50, y: 560 },
           *       { x: 50, y: 560, mergeCommit: true }
           *     ]
           */
          withMergeCommits() {
            const mergeCommits = this.commits.filter(({ parents }) => parents.length > 1);
            mergeCommits.forEach((mergeCommit) => {
              const parentOnOriginBranch = this.commits.find(({ hash }) => {
                return hash === mergeCommit.parents[1];
              });
              if (!parentOnOriginBranch)
                return;
              const originBranchName = parentOnOriginBranch.branches ? parentOnOriginBranch.branches[0] : "";
              let branch2 = this.branches.get(originBranchName);
              if (!branch2) {
                branch2 = this.getDeletedBranchInPath();
                if (!branch2) {
                  return;
                }
              }
              const lastPoints = [...this.branchesPaths.get(branch2) || []];
              this.branchesPaths.set(branch2, [
                ...lastPoints,
                { x: mergeCommit.x, y: mergeCommit.y, mergeCommit: true }
              ]);
            });
          }
          /**
           * Retrieve deleted branch from calculator's branches paths.
           */
          getDeletedBranchInPath() {
            return Array.from(this.branchesPaths.keys()).find((branch2) => branch2.isDeleted());
          }
          /**
           * Smooth all paths by putting points on each row.
           */
          smoothBranchesPaths() {
            const branchesPaths2 = /* @__PURE__ */ new Map();
            this.branchesPaths.forEach((points, branch2) => {
              if (points.length <= 1) {
                branchesPaths2.set(branch2, [points]);
                return;
              }
              if (this.isGraphVertical) {
                points = points.sort((a, b) => a.y > b.y ? -1 : 1);
              } else {
                points = points.sort((a, b) => a.x > b.x ? 1 : -1);
              }
              if (this.isGraphReverse) {
                points = points.reverse();
              }
              const paths = points.reduce((mem, point, i) => {
                if (point.mergeCommit) {
                  mem[mem.length - 1].push(utils.pick(point, ["x", "y"]));
                  let j = i - 1;
                  let previousPoint = points[j];
                  while (j >= 0 && previousPoint.mergeCommit) {
                    j--;
                    previousPoint = points[j];
                  }
                  if (j >= 0) {
                    mem.push([previousPoint]);
                  }
                } else {
                  mem[mem.length - 1].push(point);
                }
                return mem;
              }, [[]]);
              if (this.isGraphReverse) {
                paths.forEach((path) => path.reverse());
              }
              if (this.isGraphVertical) {
                paths.forEach((subPath) => {
                  if (subPath.length <= 1)
                    return;
                  const firstPoint = subPath[0];
                  const lastPoint = subPath[subPath.length - 1];
                  const column = subPath[1].x;
                  const branchSize = Math.round(Math.abs(firstPoint.y - lastPoint.y) / this.commitSpacing) - 1;
                  const branchPoints = branchSize > 0 ? new Array(branchSize).fill(0).map((_, i) => ({
                    x: column,
                    y: subPath[0].y - this.commitSpacing * (i + 1)
                  })) : [];
                  const lastSubPaths = branchesPaths2.get(branch2) || [];
                  branchesPaths2.set(branch2, [
                    ...lastSubPaths,
                    [firstPoint, ...branchPoints, lastPoint]
                  ]);
                });
              } else {
                paths.forEach((subPath) => {
                  if (subPath.length <= 1)
                    return;
                  const firstPoint = subPath[0];
                  const lastPoint = subPath[subPath.length - 1];
                  const column = subPath[1].y;
                  const branchSize = Math.round(Math.abs(firstPoint.x - lastPoint.x) / this.commitSpacing) - 1;
                  const branchPoints = branchSize > 0 ? new Array(branchSize).fill(0).map((_, i) => ({
                    y: column,
                    x: subPath[0].x + this.commitSpacing * (i + 1)
                  })) : [];
                  const lastSubPaths = branchesPaths2.get(branch2) || [];
                  branchesPaths2.set(branch2, [
                    ...lastSubPaths,
                    [firstPoint, ...branchPoints, lastPoint]
                  ]);
                });
              }
            });
            return branchesPaths2;
          }
        }
        exports3.BranchesPathsCalculator = BranchesPathsCalculator;
        function toSvgPath(coordinates, isBezier, isVertical) {
          return coordinates.map((path) => "M" + path.map(({ x, y }, i, points) => {
            if (isBezier && points.length > 1 && (i === 1 || i === points.length - 1)) {
              const previous = points[i - 1];
              if (isVertical) {
                const middleY = (previous.y + y) / 2;
                return `C ${previous.x} ${middleY} ${x} ${middleY} ${x} ${y}`;
              } else {
                const middleX = (previous.x + x) / 2;
                return `C ${middleX} ${previous.y} ${middleX} ${y} ${x} ${y}`;
              }
            }
            return `L ${x} ${y}`;
          }).join(" ").slice(1)).join(" ");
        }
        exports3.toSvgPath = toSvgPath;
      });
      unwrapExports(branchesPaths);
      var branchesPaths_1 = branchesPaths.BranchesPathsCalculator;
      var branchesPaths_2 = branchesPaths.toSvgPath;
      var gitgraphUserApi = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class GitgraphUserApi {
          // tslint:enable:variable-name
          constructor(graph, onGraphUpdate) {
            this._graph = graph;
            this._onGraphUpdate = onGraphUpdate;
          }
          /**
           * Clear everything (as `rm -rf .git && git init`).
           */
          clear() {
            this._graph.refs = new refs.Refs();
            this._graph.tags = new refs.Refs();
            this._graph.commits = [];
            this._graph.branches = /* @__PURE__ */ new Map();
            this._graph.currentBranch = this._graph.createBranch("master");
            this._onGraphUpdate();
            return this;
          }
          commit(options) {
            this._graph.currentBranch.getUserApi().commit(options);
            return this;
          }
          branch(args) {
            return this._graph.createBranch(args).getUserApi();
          }
          tag(...args) {
            let name;
            let ref;
            let style;
            let render;
            if (typeof args[0] === "string") {
              name = args[0];
              ref = args[1];
            } else {
              name = args[0].name;
              ref = args[0].ref;
              style = args[0].style;
              render = args[0].render;
            }
            if (!ref) {
              const head = this._graph.refs.getCommit("HEAD");
              if (!head)
                return this;
              ref = head;
            }
            let commitHash;
            if (this._graph.refs.hasCommit(ref)) {
              commitHash = ref;
            }
            if (this._graph.refs.hasName(ref)) {
              commitHash = this._graph.refs.getCommit(ref);
            }
            if (!commitHash) {
              throw new Error(`The ref "${ref}" does not exist`);
            }
            this._graph.tags.set(name, commitHash);
            this._graph.tagStyles[name] = style;
            this._graph.tagRenders[name] = render;
            this._onGraphUpdate();
            return this;
          }
          /**
           * Import a JSON.
           *
           * Data can't be typed since it comes from a JSON.
           * We validate input format and throw early if something is invalid.
           *
           * @experimental
           * @param data JSON from `git2json` output
           */
          import(data) {
            const invalidData = new Error("Only `git2json` format is supported for imported data.");
            if (!Array.isArray(data)) {
              throw invalidData;
            }
            const areDataValid = data.every((options) => {
              return typeof options === "object" && typeof options.author === "object" && Array.isArray(options.refs);
            });
            if (!areDataValid) {
              throw invalidData;
            }
            const commitOptionsList = data.map((options) => Object.assign({}, options, { style: Object.assign({}, this._graph.template.commit, { message: Object.assign({}, this._graph.template.commit.message, { display: this._graph.shouldDisplayCommitMessage }) }), author: `${options.author.name} <${options.author.email}>` })).reverse();
            this.clear();
            this._graph.commits = commitOptionsList.map((options) => new commit.Commit(options));
            commitOptionsList.forEach(({ refs: refs2, hash }) => {
              if (!refs2)
                return;
              if (!hash)
                return;
              const TAG_PREFIX = "tag: ";
              const tags = refs2.map((ref) => ref.split(TAG_PREFIX)).map(([_, tag2]) => tag2).filter((tag2) => typeof tag2 === "string");
              tags.forEach((tag2) => this._graph.tags.set(tag2, hash));
              refs2.filter((ref) => !ref.startsWith(TAG_PREFIX)).forEach((ref) => this._graph.refs.set(ref, hash));
            });
            const branches = this._getBranches();
            this._graph.commits.map((commit2) => this._withBranches(branches, commit2)).reduce((mem, commit2) => {
              if (!commit2.branches)
                return mem;
              commit2.branches.forEach((branch2) => mem.add(branch2));
              return mem;
            }, /* @__PURE__ */ new Set()).forEach((branch2) => this.branch(branch2));
            this._onGraphUpdate();
            return this;
          }
          // tslint:disable:variable-name - Prefix `_` = explicitly private for JS users
          // TODO: get rid of these duplicated private methods.
          //
          // These belong to Gitgraph. It is duplicated because of `import()`.
          // `import()` should use regular user API instead.
          _withBranches(branches, commit2) {
            let commitBranches = Array.from((branches.get(commit2.hash) || /* @__PURE__ */ new Set()).values());
            if (commitBranches.length === 0) {
              commitBranches = [branch.DELETED_BRANCH_NAME];
            }
            return commit2.setBranches(commitBranches);
          }
          _getBranches() {
            const result = /* @__PURE__ */ new Map();
            const queue = [];
            const branches = this._graph.refs.getAllNames().filter((name) => name !== "HEAD");
            branches.forEach((branch2) => {
              const commitHash = this._graph.refs.getCommit(branch2);
              if (commitHash) {
                queue.push(commitHash);
              }
              while (queue.length > 0) {
                const currentHash = queue.pop();
                const current = this._graph.commits.find(({ hash }) => hash === currentHash);
                const prevBranches = result.get(currentHash) || /* @__PURE__ */ new Set();
                prevBranches.add(branch2);
                result.set(currentHash, prevBranches);
                if (current && current.parents && current.parents.length > 0) {
                  queue.push(current.parents[0]);
                }
              }
            });
            return result;
          }
        }
        exports3.GitgraphUserApi = GitgraphUserApi;
      });
      unwrapExports(gitgraphUserApi);
      var gitgraphUserApi_1 = gitgraphUserApi.GitgraphUserApi;
      var gitgraph = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        class GitgraphCore {
          constructor(options = {}) {
            this.refs = new refs.Refs();
            this.tags = new refs.Refs();
            this.tagStyles = {};
            this.tagRenders = {};
            this.commits = [];
            this.branches = /* @__PURE__ */ new Map();
            this.listeners = [];
            this.nextTimeoutId = null;
            this.template = template.getTemplate(options.template);
            this.currentBranch = this.createBranch("master");
            this.orientation = options.orientation;
            this.reverseArrow = utils.booleanOptionOr(options.reverseArrow, false);
            this.initCommitOffsetX = utils.numberOptionOr(options.initCommitOffsetX, 0);
            this.initCommitOffsetY = utils.numberOptionOr(options.initCommitOffsetY, 0);
            this.mode = options.mode;
            this.author = options.author || "Sergio Flores <saxo-guy@epic.com>";
            this.commitMessage = options.commitMessage || "He doesn't like George Michael! Boooo!";
            this.generateCommitHash = typeof options.generateCommitHash === "function" ? options.generateCommitHash : () => void 0;
            this.branchesOrderFunction = typeof options.compareBranchesOrder === "function" ? options.compareBranchesOrder : void 0;
            this.branchLabelOnEveryCommit = utils.booleanOptionOr(options.branchLabelOnEveryCommit, false);
          }
          get isHorizontal() {
            return this.orientation === orientation.Orientation.Horizontal || this.orientation === orientation.Orientation.HorizontalReverse;
          }
          get isVertical() {
            return !this.isHorizontal;
          }
          get isReverse() {
            return this.orientation === orientation.Orientation.HorizontalReverse || this.orientation === orientation.Orientation.VerticalReverse;
          }
          get shouldDisplayCommitMessage() {
            return !this.isHorizontal && this.mode !== mode.Mode.Compact;
          }
          /**
           * Return the API to manipulate Gitgraph as a user.
           * Rendering library should give that API to their consumer.
           */
          getUserApi() {
            return new gitgraphUserApi.GitgraphUserApi(this, () => this.next());
          }
          /**
           * Add a change listener.
           * It will be called any time the graph have changed (commit, merge…).
           *
           * @param listener A callback to be invoked on every change.
           * @returns A function to remove this change listener.
           */
          subscribe(listener) {
            this.listeners.push(listener);
            let isSubscribed = true;
            return () => {
              if (!isSubscribed)
                return;
              isSubscribed = false;
              const index = this.listeners.indexOf(listener);
              this.listeners.splice(index, 1);
            };
          }
          /**
           * Return all data required for rendering.
           * Rendering libraries will use this to implement their rendering strategy.
           */
          getRenderedData() {
            const commits = this.computeRenderedCommits();
            const branchesPaths2 = this.computeRenderedBranchesPaths(commits);
            const commitMessagesX = this.computeCommitMessagesX(branchesPaths2);
            this.computeBranchesColor(commits, branchesPaths2);
            return { commits, branchesPaths: branchesPaths2, commitMessagesX };
          }
          createBranch(args) {
            const defaultParentBranchName = "HEAD";
            let options = {
              gitgraph: this,
              name: "",
              parentCommitHash: this.refs.getCommit(defaultParentBranchName),
              style: this.template.branch,
              onGraphUpdate: () => this.next()
            };
            if (typeof args === "string") {
              options.name = args;
              options.parentCommitHash = this.refs.getCommit(defaultParentBranchName);
            } else {
              const parentBranchName = args.from ? args.from.name : defaultParentBranchName;
              const parentCommitHash = this.refs.getCommit(parentBranchName) || (this.refs.hasCommit(args.from) ? args.from : void 0);
              args.style = args.style || {};
              options = Object.assign({}, options, args, { parentCommitHash, style: Object.assign({}, options.style, args.style, { label: Object.assign({}, options.style.label, args.style.label) }) });
            }
            const branch$1 = new branch.Branch(options);
            this.branches.set(branch$1.name, branch$1);
            return branch$1;
          }
          /**
           * Return commits with data for rendering.
           */
          computeRenderedCommits() {
            const branches = this.getBranches();
            const reachableUnassociatedCommits = (() => {
              const unassociatedCommits = new Set(this.commits.reduce((commits, { hash }) => !branches.has(hash) ? [...commits, hash] : commits, []));
              const tipsOfMergedBranches = this.commits.reduce((tipsOfMergedBranches2, commit2) => commit2.parents.length > 1 ? [
                ...tipsOfMergedBranches2,
                ...commit2.parents.slice(1).map((parentHash) => this.commits.find(({ hash }) => parentHash === hash))
              ] : tipsOfMergedBranches2, []);
              const reachableCommits = /* @__PURE__ */ new Set();
              tipsOfMergedBranches.forEach((tip) => {
                let currentCommit = tip;
                while (currentCommit && unassociatedCommits.has(currentCommit.hash)) {
                  reachableCommits.add(currentCommit.hash);
                  currentCommit = currentCommit.parents.length > 0 ? this.commits.find(({ hash }) => currentCommit.parents[0] === hash) : void 0;
                }
              });
              return reachableCommits;
            })();
            const commitsToRender = this.commits.filter(({ hash }) => branches.has(hash) || reachableUnassociatedCommits.has(hash));
            const commitsWithBranches = commitsToRender.map((commit2) => this.withBranches(branches, commit2));
            const rows = graphRows.createGraphRows(this.mode, commitsToRender);
            const branchesOrder$1 = new branchesOrder.BranchesOrder(commitsWithBranches, this.template.colors, this.branchesOrderFunction);
            return commitsWithBranches.map((commit2) => commit2.setRefs(this.refs)).map((commit2) => this.withPosition(rows, branchesOrder$1, commit2)).map((commit2) => commit2.withDefaultColor(this.getBranchDefaultColor(branchesOrder$1, commit2.branchToDisplay))).map((commit2) => commit2.setTags(this.tags, (name) => Object.assign({}, this.tagStyles[name], this.template.tag), (name) => this.tagRenders[name]));
          }
          /**
           * Return branches paths with all data required for rendering.
           *
           * @param commits List of commits with rendering data computed
           */
          computeRenderedBranchesPaths(commits) {
            return new branchesPaths.BranchesPathsCalculator(commits, this.branches, this.template.commit.spacing, this.isVertical, this.isReverse, () => branch.createDeletedBranch(this, this.template.branch, () => this.next())).execute();
          }
          /**
           * Set branches colors based on branches paths.
           *
           * @param commits List of graph commits
           * @param branchesPaths Branches paths to be rendered
           */
          computeBranchesColor(commits, branchesPaths2) {
            const branchesOrder$1 = new branchesOrder.BranchesOrder(commits, this.template.colors, this.branchesOrderFunction);
            Array.from(branchesPaths2).forEach(([branch2]) => {
              branch2.computedColor = branch2.style.color || this.getBranchDefaultColor(branchesOrder$1, branch2.name);
            });
          }
          /**
           * Return commit messages X position for rendering.
           *
           * @param branchesPaths Branches paths to be rendered
           */
          computeCommitMessagesX(branchesPaths2) {
            const numberOfColumns = Array.from(branchesPaths2).length;
            return numberOfColumns * this.template.branch.spacing;
          }
          /**
           * Add `branches` property to commit.
           *
           * @param branches All branches mapped by commit hash
           * @param commit Commit
           */
          withBranches(branches, commit2) {
            let commitBranches = Array.from((branches.get(commit2.hash) || /* @__PURE__ */ new Set()).values());
            if (commitBranches.length === 0) {
              commitBranches = [branch.DELETED_BRANCH_NAME];
            }
            return commit2.setBranches(commitBranches);
          }
          /**
           * Get all branches from current commits.
           */
          getBranches() {
            const result = /* @__PURE__ */ new Map();
            const queue = [];
            const branches = this.refs.getAllNames().filter((name) => name !== "HEAD");
            branches.forEach((branch2) => {
              const commitHash = this.refs.getCommit(branch2);
              if (commitHash) {
                queue.push(commitHash);
              }
              while (queue.length > 0) {
                const currentHash = queue.pop();
                const current = this.commits.find(({ hash }) => hash === currentHash);
                const prevBranches = result.get(currentHash) || /* @__PURE__ */ new Set();
                prevBranches.add(branch2);
                result.set(currentHash, prevBranches);
                if (current && current.parents && current.parents.length > 0) {
                  queue.push(current.parents[0]);
                }
              }
            });
            return result;
          }
          /**
           * Add position to given commit.
           *
           * @param rows Graph rows
           * @param branchesOrder Computed order of branches
           * @param commit Commit to position
           */
          withPosition(rows, branchesOrder2, commit2) {
            const row = rows.getRowOf(commit2.hash);
            const maxRow = rows.getMaxRow();
            const order = branchesOrder2.get(commit2.branchToDisplay);
            switch (this.orientation) {
              default:
                return commit2.setPosition({
                  x: this.initCommitOffsetX + this.template.branch.spacing * order,
                  y: this.initCommitOffsetY + this.template.commit.spacing * (maxRow - row)
                });
              case orientation.Orientation.VerticalReverse:
                return commit2.setPosition({
                  x: this.initCommitOffsetX + this.template.branch.spacing * order,
                  y: this.initCommitOffsetY + this.template.commit.spacing * row
                });
              case orientation.Orientation.Horizontal:
                return commit2.setPosition({
                  x: this.initCommitOffsetX + this.template.commit.spacing * row,
                  y: this.initCommitOffsetY + this.template.branch.spacing * order
                });
              case orientation.Orientation.HorizontalReverse:
                return commit2.setPosition({
                  x: this.initCommitOffsetX + this.template.commit.spacing * (maxRow - row),
                  y: this.initCommitOffsetY + this.template.branch.spacing * order
                });
            }
          }
          /**
           * Return the default color for given branch.
           *
           * @param branchesOrder Computed order of branches
           * @param branchName Name of the branch
           */
          getBranchDefaultColor(branchesOrder2, branchName) {
            return branchesOrder2.getColorOf(branchName);
          }
          /**
           * Tell each listener something new happened.
           * E.g. a rendering library will know it needs to re-render the graph.
           */
          next() {
            if (this.nextTimeoutId) {
              window.clearTimeout(this.nextTimeoutId);
            }
            this.nextTimeoutId = window.setTimeout(() => {
              this.listeners.forEach((listener) => listener(this.getRenderedData()));
            }, 0);
          }
        }
        exports3.GitgraphCore = GitgraphCore;
      });
      unwrapExports(gitgraph);
      var gitgraph_1 = gitgraph.GitgraphCore;
      var lib = createCommonjsModule(function(module2, exports3) {
        Object.defineProperty(exports3, "__esModule", { value: true });
        exports3.GitgraphCore = gitgraph.GitgraphCore;
        exports3.Mode = mode.Mode;
        exports3.GitgraphUserApi = gitgraphUserApi.GitgraphUserApi;
        exports3.BranchUserApi = branchUserApi.BranchUserApi;
        exports3.Branch = branch.Branch;
        exports3.Commit = commit.Commit;
        exports3.Tag = tag.Tag;
        exports3.Refs = refs.Refs;
        exports3.MergeStyle = template.MergeStyle;
        exports3.TemplateName = template.TemplateName;
        exports3.templateExtend = template.templateExtend;
        exports3.Orientation = orientation.Orientation;
        exports3.toSvgPath = branchesPaths.toSvgPath;
        exports3.arrowSvgPath = utils.arrowSvgPath;
      });
      unwrapExports(lib);
      var lib_1 = lib.GitgraphCore;
      var lib_2 = lib.Mode;
      var lib_3 = lib.GitgraphUserApi;
      var lib_4 = lib.BranchUserApi;
      var lib_5 = lib.Branch;
      var lib_6 = lib.Commit;
      var lib_7 = lib.Tag;
      var lib_8 = lib.Refs;
      var lib_9 = lib.MergeStyle;
      var lib_10 = lib.TemplateName;
      var lib_11 = lib.templateExtend;
      var lib_12 = lib.Orientation;
      var lib_13 = lib.toSvgPath;
      var lib_14 = lib.arrowSvgPath;
      var SVG_NAMESPACE = "http://www.w3.org/2000/svg";
      function createSvg(options) {
        var svg = document.createElementNS(SVG_NAMESPACE, "svg");
        if (!options)
          return svg;
        if (options.children) {
          options.children.forEach(function(child) {
            return svg.appendChild(child);
          });
        }
        if (options.viewBox) {
          svg.setAttribute("viewBox", options.viewBox);
        }
        if (options.height) {
          svg.setAttribute("height", options.height.toString());
        }
        if (options.width) {
          svg.setAttribute("width", options.width.toString());
        }
        return svg;
      }
      function createG(options) {
        var g = document.createElementNS(SVG_NAMESPACE, "g");
        options.children.forEach(function(child) {
          return child && g.appendChild(child);
        });
        if (options.translate) {
          g.setAttribute("transform", "translate(" + options.translate.x + ", " + options.translate.y + ")");
        }
        if (options.fill) {
          g.setAttribute("fill", options.fill);
        }
        if (options.stroke) {
          g.setAttribute("stroke", options.stroke);
        }
        if (options.strokeWidth) {
          g.setAttribute("stroke-width", options.strokeWidth.toString());
        }
        if (options.onClick) {
          g.addEventListener("click", options.onClick);
        }
        if (options.onMouseOver) {
          g.addEventListener("mouseover", options.onMouseOver);
        }
        if (options.onMouseOut) {
          g.addEventListener("mouseout", options.onMouseOut);
        }
        return g;
      }
      function createText(options) {
        var text = document.createElementNS(SVG_NAMESPACE, "text");
        text.setAttribute("alignment-baseline", "central");
        text.setAttribute("dominant-baseline", "central");
        text.textContent = options.content;
        if (options.fill) {
          text.setAttribute("fill", options.fill);
        }
        if (options.font) {
          text.setAttribute("style", "font: " + options.font);
        }
        if (options.anchor) {
          text.setAttribute("text-anchor", options.anchor);
        }
        if (options.translate) {
          text.setAttribute("x", options.translate.x.toString());
          text.setAttribute("y", options.translate.y.toString());
        }
        if (options.onClick) {
          text.addEventListener("click", options.onClick);
        }
        return text;
      }
      function createCircle(options) {
        var circle = document.createElementNS(SVG_NAMESPACE, "circle");
        circle.setAttribute("cx", options.radius.toString());
        circle.setAttribute("cy", options.radius.toString());
        circle.setAttribute("r", options.radius.toString());
        if (options.id) {
          circle.setAttribute("id", options.id);
        }
        if (options.fill) {
          circle.setAttribute("fill", options.fill);
        }
        return circle;
      }
      function createRect(options) {
        var rect = document.createElementNS(SVG_NAMESPACE, "rect");
        rect.setAttribute("width", options.width.toString());
        rect.setAttribute("height", options.height.toString());
        if (options.borderRadius) {
          rect.setAttribute("rx", options.borderRadius.toString());
        }
        if (options.fill) {
          rect.setAttribute("fill", options.fill || "none");
        }
        if (options.stroke) {
          rect.setAttribute("stroke", options.stroke);
        }
        return rect;
      }
      function createPath(options) {
        var path = document.createElementNS(SVG_NAMESPACE, "path");
        path.setAttribute("d", options.d);
        if (options.fill) {
          path.setAttribute("fill", options.fill);
        }
        if (options.stroke) {
          path.setAttribute("stroke", options.stroke);
        }
        if (options.strokeWidth) {
          path.setAttribute("stroke-width", options.strokeWidth.toString());
        }
        if (options.translate) {
          path.setAttribute("transform", "translate(" + options.translate.x + ", " + options.translate.y + ")");
        }
        return path;
      }
      function createUse(href) {
        var use = document.createElementNS(SVG_NAMESPACE, "use");
        use.setAttribute("href", "#" + href);
        use.setAttributeNS("http://www.w3.org/1999/xlink", "xlink:href", "#" + href);
        return use;
      }
      function createClipPath() {
        return document.createElementNS(SVG_NAMESPACE, "clipPath");
      }
      function createDefs(children) {
        var defs = document.createElementNS(SVG_NAMESPACE, "defs");
        children.forEach(function(child) {
          return defs.appendChild(child);
        });
        return defs;
      }
      function createForeignObject(options) {
        var result = document.createElementNS(SVG_NAMESPACE, "foreignObject");
        result.setAttribute("width", options.width.toString());
        if (options.translate) {
          result.setAttribute("x", options.translate.x.toString());
          result.setAttribute("y", options.translate.y.toString());
        }
        var p = document.createElement("p");
        p.textContent = options.content;
        result.appendChild(p);
        return result;
      }
      var PADDING_X = 10;
      var PADDING_Y = 5;
      function createBranchLabel(branch2, commit2) {
        var rect = createRect({
          width: 0,
          height: 0,
          borderRadius: branch2.style.label.borderRadius,
          stroke: branch2.style.label.strokeColor || commit2.style.color,
          fill: branch2.style.label.bgColor
        });
        var text = createText({
          content: branch2.name,
          translate: {
            x: PADDING_X,
            y: 0
          },
          font: branch2.style.label.font,
          fill: branch2.style.label.color || commit2.style.color
        });
        var branchLabel = createG({ children: [rect] });
        var observer = new MutationObserver(function() {
          var _a = text.getBBox(), height = _a.height, width = _a.width;
          var boxWidth = width + 2 * PADDING_X;
          var boxHeight = height + 2 * PADDING_Y;
          rect.setAttribute("width", boxWidth.toString());
          rect.setAttribute("height", boxHeight.toString());
          text.setAttribute("y", (boxHeight / 2).toString());
        });
        observer.observe(branchLabel, {
          attributes: false,
          subtree: false,
          childList: true
        });
        branchLabel.appendChild(text);
        return branchLabel;
      }
      var PADDING_X$1 = 10;
      var PADDING_Y$1 = 5;
      function createTag(tag2) {
        var path = createPath({
          d: "",
          fill: tag2.style.bgColor,
          stroke: tag2.style.strokeColor
        });
        var text = createText({
          content: tag2.name,
          fill: tag2.style.color,
          font: tag2.style.font,
          translate: { x: 0, y: 0 }
        });
        var result = createG({ children: [path] });
        var offset = tag2.style.pointerWidth;
        var observer = new MutationObserver(function() {
          var _a = text.getBBox(), height = _a.height, width = _a.width;
          if (height === 0 || width === 0)
            return;
          var radius = tag2.style.borderRadius;
          var boxWidth = offset + width + 2 * PADDING_X$1;
          var boxHeight = height + 2 * PADDING_Y$1;
          var pathD = [
            "M 0,0",
            "L " + offset + "," + boxHeight / 2,
            "V " + boxHeight / 2,
            "Q " + offset + "," + boxHeight / 2 + " " + (offset + radius) + "," + boxHeight / 2,
            "H " + (boxWidth - radius),
            "Q " + boxWidth + "," + boxHeight / 2 + " " + boxWidth + "," + (boxHeight / 2 - radius),
            "V -" + (boxHeight / 2 - radius),
            "Q " + boxWidth + ",-" + boxHeight / 2 + " " + (boxWidth - radius) + ",-" + boxHeight / 2,
            "H " + (offset + radius),
            "Q " + offset + ",-" + boxHeight / 2 + " " + offset + ",-" + boxHeight / 2,
            "V -" + boxHeight / 2,
            "z"
          ].join(" ");
          path.setAttribute("d", pathD.toString());
          text.setAttribute("x", (offset + PADDING_X$1).toString());
        });
        observer.observe(result, {
          attributes: false,
          subtree: false,
          childList: true
        });
        result.appendChild(text);
        return result;
      }
      var PADDING = 10;
      var OFFSET = 10;
      function createTooltip(commit2) {
        var path = createPath({ d: "", fill: "#EEE" });
        var text = createText({
          translate: { x: OFFSET + PADDING, y: 0 },
          content: commit2.hashAbbrev + " - " + commit2.subject,
          fill: "#333"
        });
        var commitSize = commit2.style.dot.size * 2;
        var tooltip = createG({
          translate: { x: commitSize, y: commitSize / 2 },
          children: [path]
        });
        var observer = new MutationObserver(function() {
          var width = text.getBBox().width;
          var radius = 5;
          var boxHeight = 50;
          var boxWidth = OFFSET + width + 2 * PADDING;
          var pathD = [
            "M 0,0",
            "L " + OFFSET + "," + OFFSET,
            "V " + (boxHeight / 2 - radius),
            "Q " + OFFSET + "," + boxHeight / 2 + " " + (OFFSET + radius) + "," + boxHeight / 2,
            "H " + (boxWidth - radius),
            "Q " + boxWidth + "," + boxHeight / 2 + " " + boxWidth + "," + (boxHeight / 2 - radius),
            "V -" + (boxHeight / 2 - radius),
            "Q " + boxWidth + ",-" + boxHeight / 2 + " " + (boxWidth - radius) + ",-" + boxHeight / 2,
            "H " + (OFFSET + radius),
            "Q " + OFFSET + ",-" + boxHeight / 2 + " " + OFFSET + ",-" + (boxHeight / 2 - radius),
            "V -" + OFFSET,
            "z"
          ].join(" ");
          path.setAttribute("d", pathD.toString());
        });
        observer.observe(tooltip, {
          attributes: false,
          subtree: false,
          childList: true
        });
        tooltip.appendChild(text);
        return tooltip;
      }
      function createGitgraph2(graphContainer, options) {
        var commitsElements = {};
        var commitYWithOffsets = {};
        var shouldRecomputeOffsets = false;
        var lastData;
        var $commits;
        var commitMessagesX = 0;
        var $tooltip = null;
        var svg = createSvg();
        adaptSvgOnUpdate(Boolean(options && options.responsive));
        graphContainer.appendChild(svg);
        if (options && options.responsive) {
          graphContainer.setAttribute("style", "display:inline-block; position: relative; width:100%; padding-bottom:100%; vertical-align:middle; overflow:hidden;");
        }
        var gitgraph2 = new lib_1(options);
        gitgraph2.subscribe(function(data) {
          shouldRecomputeOffsets = true;
          render(data);
        });
        return gitgraph2.getUserApi();
        function render(data) {
          commitsElements = {};
          var commits = data.commits, branchesPaths2 = data.branchesPaths;
          commitMessagesX = data.commitMessagesX;
          lastData = data;
          $commits = renderCommits(commits);
          svg.innerHTML = "";
          svg.appendChild(createG({
            // Translate graph left => left-most branch label is not cropped (horizontal)
            // Translate graph down => top-most commit tooltip is not cropped
            translate: { x: PADDING_X, y: PADDING },
            children: [renderBranchesPaths(branchesPaths2), $commits]
          }));
        }
        function adaptSvgOnUpdate(adaptToContainer) {
          var observer = new MutationObserver(function() {
            if (shouldRecomputeOffsets) {
              shouldRecomputeOffsets = false;
              computeOffsets();
              render(lastData);
            } else {
              positionCommitsElements();
              adaptGraphDimensions(adaptToContainer);
            }
          });
          observer.observe(svg, {
            attributes: false,
            // Listen to subtree changes to react when we append the tooltip.
            subtree: true,
            childList: true
          });
          function computeOffsets() {
            var commits = Array.from($commits.children);
            var totalOffsetY = 0;
            var orientedCommits = gitgraph2.orientation === lib_12.VerticalReverse ? commits : commits.reverse();
            commitYWithOffsets = orientedCommits.reduce(function(newOffsets, commit2) {
              var commitY = parseInt(commit2.getAttribute("transform").split(",")[1].slice(0, -1), 10);
              var firstForeignObject = commit2.getElementsByTagName("foreignObject")[0];
              var customHtmlMessage = firstForeignObject && firstForeignObject.firstElementChild;
              newOffsets[commitY] = commitY + totalOffsetY;
              totalOffsetY += getMessageHeight(customHtmlMessage);
              return newOffsets;
            }, {});
          }
          function positionCommitsElements() {
            if (gitgraph2.isHorizontal) {
              return;
            }
            var padding = 10;
            Object.keys(commitsElements).forEach(function(commitHash) {
              var _a = commitsElements[commitHash], branchLabel = _a.branchLabel, tags = _a.tags, message = _a.message;
              var x = commitMessagesX;
              if (branchLabel) {
                moveElement(branchLabel, x);
                var branchLabelWidth = branchLabel.getBBox().width + 2 * PADDING_X;
                x += branchLabelWidth + padding;
              }
              tags.forEach(function(tag2) {
                moveElement(tag2, x);
                var offset = parseFloat(tag2.getAttribute("data-offset") || "0");
                var tagWidth = tag2.getBBox().width + 2 * PADDING_X$1 + offset;
                x += tagWidth + padding;
              });
              if (message) {
                moveElement(message, x);
              }
            });
          }
          function adaptGraphDimensions(adaptToContainer2) {
            var _a = svg.getBBox(), height = _a.height, width = _a.width;
            var horizontalCustomOffset = 50;
            var verticalCustomOffset = 20;
            var widthOffset = gitgraph2.isHorizontal ? horizontalCustomOffset : (
              // Add `TOOLTIP_PADDING` so we don't crop the tooltip text.
              // Add `BRANCH_LABEL_PADDING_X` so we don't cut branch label.
              PADDING_X + PADDING
            );
            var heightOffset = gitgraph2.isHorizontal ? horizontalCustomOffset : (
              // Add `TOOLTIP_PADDING` so we don't crop tooltip text
              // Add `BRANCH_LABEL_PADDING_Y` so we don't crop branch label.
              PADDING_Y + PADDING + verticalCustomOffset
            );
            if (adaptToContainer2) {
              svg.setAttribute("preserveAspectRatio", "xMinYMin meet");
              svg.setAttribute("viewBox", "0 0 " + (width + widthOffset) + " " + (height + heightOffset));
            } else {
              svg.setAttribute("width", (width + widthOffset).toString());
              svg.setAttribute("height", (height + heightOffset).toString());
            }
          }
        }
        function moveElement(target, x) {
          var transform = target.getAttribute("transform") || "translate(0, 0)";
          target.setAttribute("transform", transform.replace(/translate\(([\d\.]+),/, "translate(" + x + ","));
        }
        function renderBranchesPaths(branchesPaths2) {
          var offset = gitgraph2.template.commit.dot.size;
          var isBezier = gitgraph2.template.branch.mergeStyle === lib_9.Bezier;
          var paths = Array.from(branchesPaths2).map(function(_a) {
            var branch2 = _a[0], coordinates = _a[1];
            return createPath({
              d: lib_13(coordinates.map(function(coordinate) {
                return coordinate.map(getWithCommitOffset);
              }), isBezier, gitgraph2.isVertical),
              fill: "none",
              stroke: branch2.computedColor || "",
              strokeWidth: branch2.style.lineWidth,
              translate: {
                x: offset,
                y: offset
              }
            });
          });
          return createG({ children: paths });
        }
        function renderCommits(commits) {
          return createG({ children: commits.map(renderCommit) });
          function renderCommit(commit2) {
            var _a = getWithCommitOffset(commit2), x = _a.x, y = _a.y;
            return createG({
              translate: { x, y },
              children: [
                renderDot(commit2)
              ].concat(renderArrows(commit2), [
                createG({
                  translate: { x: -x, y: 0 },
                  children: [
                    renderMessage(commit2)
                  ].concat(renderBranchLabels(commit2), renderTags(commit2))
                })
              ])
            });
          }
          function renderArrows(commit2) {
            if (!gitgraph2.template.arrow.size) {
              return [null];
            }
            var commitRadius = commit2.style.dot.size;
            return commit2.parents.map(function(parentHash) {
              var parent = commits.find(function(_a) {
                var hash = _a.hash;
                return hash === parentHash;
              });
              if (!parent)
                return null;
              var origin = gitgraph2.reverseArrow ? {
                x: commitRadius + (parent.x - commit2.x),
                y: commitRadius + (parent.y - commit2.y)
              } : { x: commitRadius, y: commitRadius };
              var path = createPath({
                d: lib_14(gitgraph2, parent, commit2),
                fill: gitgraph2.template.arrow.color || ""
              });
              return createG({ translate: origin, children: [path] });
            });
          }
        }
        function renderMessage(commit2) {
          if (!commit2.style.message.display) {
            return null;
          }
          var message;
          if (commit2.renderMessage) {
            message = createG({ children: [] });
            adaptMessageBodyHeight(message);
            message.appendChild(commit2.renderMessage(commit2));
            setMessageRef(commit2, message);
            return message;
          }
          var text = createText({
            content: commit2.message,
            fill: commit2.style.message.color || "",
            font: commit2.style.message.font,
            onClick: commit2.onMessageClick
          });
          message = createG({
            translate: { x: 0, y: commit2.style.dot.size },
            children: [text]
          });
          if (commit2.body) {
            var body = createForeignObject({
              width: 600,
              translate: { x: 10, y: 0 },
              content: commit2.body
            });
            adaptMessageBodyHeight(message);
            message.appendChild(body);
          }
          setMessageRef(commit2, message);
          return message;
        }
        function adaptMessageBodyHeight(message) {
          var observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(_a) {
              var target = _a.target;
              return setChildrenForeignObjectHeight(target);
            });
          });
          observer.observe(message, {
            attributes: false,
            subtree: false,
            childList: true
          });
          function setChildrenForeignObjectHeight(node) {
            if (node.nodeName === "foreignObject") {
              var foreignObject = node.firstChild && node.firstChild.parentElement;
              if (!foreignObject)
                return;
              foreignObject.setAttribute("height", getMessageHeight(foreignObject.firstElementChild).toString());
            }
            node.childNodes.forEach(setChildrenForeignObjectHeight);
          }
        }
        function renderBranchLabels(commit2) {
          var branches = Array.from(gitgraph2.branches.values());
          return branches.map(function(branch2) {
            if (!branch2.style.label.display)
              return null;
            if (!gitgraph2.branchLabelOnEveryCommit) {
              var commitHash = gitgraph2.refs.getCommit(branch2.name);
              if (commit2.hash !== commitHash)
                return null;
            }
            if (commit2.branchToDisplay !== branch2.name)
              return null;
            var branchLabel = branch2.renderLabel ? branch2.renderLabel(branch2) : createBranchLabel(branch2, commit2);
            var branchLabelContainer;
            if (gitgraph2.isVertical) {
              branchLabelContainer = createG({
                children: [branchLabel]
              });
            } else {
              var commitDotSize = commit2.style.dot.size * 2;
              var horizontalMarginTop = 10;
              branchLabelContainer = createG({
                translate: { x: commit2.x, y: commitDotSize + horizontalMarginTop },
                children: [branchLabel]
              });
            }
            setBranchLabelRef(commit2, branchLabelContainer);
            return branchLabelContainer;
          });
        }
        function renderTags(commit2) {
          if (!commit2.tags)
            return [];
          if (gitgraph2.isHorizontal)
            return [];
          return commit2.tags.map(function(tag2) {
            var tagElement = tag2.render ? tag2.render(tag2.name, tag2.style) : createTag(tag2);
            var tagContainer = createG({
              translate: { x: 0, y: commit2.style.dot.size },
              children: [tagElement]
            });
            tagContainer.setAttribute("data-offset", tag2.style.pointerWidth.toString());
            setTagRef(commit2, tagContainer);
            return tagContainer;
          });
        }
        function renderDot(commit2) {
          if (commit2.renderDot) {
            return commit2.renderDot(commit2);
          }
          var circleId = commit2.hash;
          var circle = createCircle({
            id: circleId,
            radius: commit2.style.dot.size,
            fill: commit2.style.dot.color || ""
          });
          var clipPathId = "clip-" + commit2.hash;
          var circleClipPath = createClipPath();
          circleClipPath.setAttribute("id", clipPathId);
          circleClipPath.appendChild(createUse(circleId));
          var useCirclePath = createUse(circleId);
          useCirclePath.setAttribute("clip-path", "url(#" + clipPathId + ")");
          useCirclePath.setAttribute("stroke", commit2.style.dot.strokeColor || "");
          var strokeWidth = commit2.style.dot.strokeWidth ? commit2.style.dot.strokeWidth * 2 : 0;
          useCirclePath.setAttribute("stroke-width", strokeWidth.toString());
          var dotText = commit2.dotText ? createText({
            content: commit2.dotText,
            font: commit2.style.dot.font,
            anchor: "middle",
            translate: { x: commit2.style.dot.size, y: commit2.style.dot.size }
          }) : null;
          return createG({
            onClick: commit2.onClick,
            onMouseOver: function() {
              appendTooltipToGraph(commit2);
              commit2.onMouseOver();
            },
            onMouseOut: function() {
              if ($tooltip)
                $tooltip.remove();
              commit2.onMouseOut();
            },
            children: [createDefs([circle, circleClipPath]), useCirclePath, dotText]
          });
        }
        function appendTooltipToGraph(commit2) {
          if (!svg.firstChild)
            return;
          if (gitgraph2.isVertical && gitgraph2.mode !== lib_2.Compact)
            return;
          if (gitgraph2.isVertical && !commit2.style.hasTooltipInCompactMode)
            return;
          var tooltip = commit2.renderTooltip ? commit2.renderTooltip(commit2) : createTooltip(commit2);
          $tooltip = createG({
            translate: getWithCommitOffset(commit2),
            children: [tooltip]
          });
          svg.firstChild.appendChild($tooltip);
        }
        function getWithCommitOffset(_a) {
          var x = _a.x, y = _a.y;
          return { x, y: commitYWithOffsets[y] || y };
        }
        function setBranchLabelRef(commit2, branchLabels) {
          if (!commitsElements[commit2.hashAbbrev]) {
            initCommitElements(commit2);
          }
          commitsElements[commit2.hashAbbrev].branchLabel = branchLabels;
        }
        function setMessageRef(commit2, message) {
          if (!commitsElements[commit2.hashAbbrev]) {
            initCommitElements(commit2);
          }
          commitsElements[commit2.hashAbbrev].message = message;
        }
        function setTagRef(commit2, tag2) {
          if (!commitsElements[commit2.hashAbbrev]) {
            initCommitElements(commit2);
          }
          commitsElements[commit2.hashAbbrev].tags.push(tag2);
        }
        function initCommitElements(commit2) {
          commitsElements[commit2.hashAbbrev] = {
            branchLabel: null,
            tags: [],
            message: null
          };
        }
      }
      function getMessageHeight(message) {
        var messageHeight = 0;
        if (message) {
          var height = message.getBoundingClientRect().height;
          var marginTopInPx = window.getComputedStyle(message).marginTop || "0px";
          var marginTop = parseInt(marginTopInPx.replace("px", ""), 10);
          messageHeight = height + marginTop;
        }
        return messageHeight;
      }
      exports2.MergeStyle = lib_9;
      exports2.Mode = lib_2;
      exports2.Orientation = lib_12;
      exports2.TemplateName = lib_10;
      exports2.createGitgraph = createGitgraph2;
      exports2.templateExtend = lib_11;
      Object.defineProperty(exports2, "__esModule", { value: true });
    });
  }
});

// js/gitgraph-renderer.src.js
var import_js = __toESM(require_gitgraph_umd());
var _template = null;
function readCss(name) {
  return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}
async function initRenderer(_isDark) {
  const branchColors = [0, 1, 2, 3, 4, 5].map((i) => readCss(`--gitvis-graph-branch-${i}`));
  const nodeText = readCss("--gitvis-graph-node-text");
  const edge = readCss("--gitvis-graph-edge");
  _template = (0, import_js.templateExtend)(import_js.TemplateName.Metro, {
    colors: branchColors,
    branch: {
      lineWidth: 4,
      spacing: 30,
      mergeStyle: "straight",
      label: {
        font: "normal 13px var(--gitvis-font-ui, sans-serif)",
        bgColor: "transparent",
        strokeColor: "transparent"
      }
    },
    commit: {
      spacing: 60,
      dot: { size: 10, strokeWidth: 0 },
      message: {
        color: nodeText,
        displayHash: true,
        displayAuthor: false,
        font: "normal 13px var(--gitvis-font-mono, monospace)"
      }
    },
    arrow: { color: edge, size: 0, offset: 0 },
    tag: {
      color: nodeText,
      bgColor: "transparent",
      font: "bold 12px var(--gitvis-font-mono, monospace)",
      borderRadius: 4,
      pointerWidth: 6
    }
  });
}
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
async function renderGraph(containerId, payload) {
  const container = document.getElementById(containerId);
  if (!container || !payload || !payload.commits || payload.commits.length === 0) return;
  if (!_template) await initRenderer(false);
  container.innerHTML = "";
  const gitgraph = (0, import_js.createGitgraph)(container, {
    template: _template,
    orientation: "horizontal"
  });
  const branches = /* @__PURE__ */ new Map();
  for (const c of payload.commits) {
    const subject = (c.message || "").split("\n")[0];
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
            tag: c.isHead ? "HEAD" : void 0
          }
        });
        continue;
      }
    }
    branch.commit({
      hash: c.oid,
      subject,
      author: c.author,
      tag: c.isHead ? `HEAD \u2192 ${payload.headBranch}` : void 0,
      dotText: void 0,
      style: c.isHead ? { dot: { strokeWidth: 2, strokeColor: readCss("--gitvis-graph-node-text") } } : void 0
    });
  }
}
function getReducedMotion() {
  return window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}
function scrollToHead(containerId, reduceMotion = false) {
  const container = document.getElementById(containerId);
  if (!container) return;
  const head = container.querySelector(".commit-highlight");
  const behavior = reduceMotion ? "instant" : "smooth";
  if (head) {
    head.scrollIntoView({ behavior, block: "nearest", inline: "end" });
  } else {
    container.scrollLeft = container.scrollWidth;
  }
}
export {
  getReducedMotion,
  initRenderer,
  renderGraph,
  scrollToHead
};
