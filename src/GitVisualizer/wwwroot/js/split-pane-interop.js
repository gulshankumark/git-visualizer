// split-pane-interop.js — minimal JS for container measurements and theme
export function getContainerWidth(element) {
    return element.getBoundingClientRect().width;
}

export function setDocumentTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
}
